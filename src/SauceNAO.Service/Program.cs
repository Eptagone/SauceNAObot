// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core;
using SauceNAO.Core.Extensions;
using SauceNAO.Infrastructure;
using SauceNAO.Infrastructure.Data;
using SauceNAO.Service;

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices((context, services) =>
	{
		var configuration = context.Configuration;

		// Configure database context
		var connectionString = configuration.GetConnectionString("Default");

		if (connectionString == null)
		{
			throw new NullReferenceException("The connection string cannot be null");
		}

		switch (configuration["DbProvider"])
		{
			case "SQLite":
			case "sqlite":
			case "lite":
			case "Lite":
			default:
				// Use SQLite Server. Default.
				services.AddDbContext<SauceNaoContext>(options => options.UseSqlite(connectionString));
				break;
			case "SqlServer":
			case "sqlserver":
			case "mssql":
			case "sql":
				// Use SQL Server.
				services.AddDbContext<SauceNaoContext>(options => options.UseSqlServer(connectionString));
				break;
		}


		// Configure cache context
		var cacheConnection = $"Data Source={Path.GetTempFileName()}"; // Get connection string for cache
		services.AddDbContext<CacheDbContext>(options => options.UseSqlite(cacheConnection));

		// Add bot service.
		services.AddSauceBot<BotDb>();

		// Add Data Cleaner service
		//services.AddHostedService<CleanerService>();

		// Add long polling service
		services.AddHostedService<Worker>();
	})
	.Build();

// Create database if not exists
using (var scope = host.Services.CreateScope())
{
	using var context = scope.ServiceProvider.GetRequiredService<SauceNaoContext>();
#if DEBUG
	// context.Database.EnsureDeleted(); // Delete database
	context.Database.EnsureCreated(); // Create database without migrations
#else
    // context.Database.EnsureCreated(); // Create database without migrations
    context.Database.Migrate(); // Create database using migrations
#endif
}

using (var scope = host.Services.CreateScope())
{
	using var context = scope.ServiceProvider.GetRequiredService<CacheDbContext>();
	// Create cache file
	context.Database.EnsureCreated();
	// Initialize bot
	_ = scope.ServiceProvider.GetRequiredService<SnaoBotProperties>();
}

await host.RunAsync();
