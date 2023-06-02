// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Extensions;
using SauceNAO.Core.Services;
using SauceNAO.Infrastructure;
using SauceNAO.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure database context
var connectionString = builder.Configuration.GetConnectionString("Default")!;

switch (builder.Configuration["DbProvider"])
{
	case "SQLite":
	case "sqlite":
	case "lite":
	case "Lite":
	default:
		// Use SQLite Server. Default.
		builder.Services.AddDbContext<SauceNaoContext>(options => options.UseSqlite(connectionString));
		break;
	case "SqlServer":
	case "sqlserver":
	case "mssql":
	case "sql":
		// Use SQL Server.
		builder.Services.AddDbContext<SauceNaoContext>(options => options.UseSqlServer(connectionString));
		break;
}

// Configure cache context
var cacheConnection = $"Data Source={Path.GetTempFileName()}"; // Get connection string for cache
builder.Services.AddDbContext<CacheDbContext>(options => options.UseSqlite(cacheConnection));

// Add temp repository
builder.Services.AddScoped<TemporalFileRepository>();

// Add bot service.
builder.Services.AddSauceBot<BotDb>();

// Add long polling service. If a webhook is used, this service will be automatically terminated.
builder.Services.AddHostedService<PollingService>();

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Create database if not exists
using (var scope = app.Services.CreateScope())
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

using (var scope = app.Services.CreateScope())
{
	using var context = scope.ServiceProvider.GetRequiredService<CacheDbContext>();
	// Create cache file
	context.Database.EnsureCreated();
	// Initialize bot
	_ = scope.ServiceProvider.GetRequiredService<SnaoBotProperties>();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
