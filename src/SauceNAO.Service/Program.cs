// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Infrastructure;
using SauceNAO.Infrastructure.Data;
using SauceNAO.Service;
using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Configure database context
        var connectionString = configuration.GetConnectionString("SauceNAO");
        services.AddDbContext<SauceNaoContext>(options => options.UseSqlServer(connectionString));

        // Configure cache context
        var cacheConnection = $"Data Source={Path.GetTempFileName()}"; // Get connection string for cache
        services.AddDbContext<CacheDbContext>(options => options.UseSqlite(cacheConnection));

        // Add bot service.
        services.AddSauceBot<BotDb>(configuration);

        // Add Data Cleaner service
        services.AddHostedService<CleanerService>();

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
    context.Database.Migrate(); // Create database using migrations
#endif
}

// Create cache file
using (var scope = host.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<CacheDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();
