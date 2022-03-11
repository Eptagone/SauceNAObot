// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Extensions;
using SauceNAO.Infrastructure;
using SauceNAO.Infrastructure.Data;
using SauceNAO.Webhook.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure database context
var connectionString = builder.Configuration.GetConnectionString("SauceNAO");
builder.Services.AddDbContext<SauceNaoContext>(options => options.UseSqlite(connectionString));
// builder.Services.AddDbContext<SauceNaoContext>(options => options.UseSqlServer(connectionString));

// Configure cache context
var cacheConnection = $"Data Source={Path.GetTempFileName()}"; // Get connection string for cache
builder.Services.AddDbContext<CacheDbContext>(options => options.UseSqlite(cacheConnection));

// Add temp repository
builder.Services.AddScoped<TemporalFileRepository>();

// Add bot service.
builder.Services.AddSauceBot<BotDb>(builder.Configuration);

// Add Data Cleaner service
builder.Services.AddHostedService<CleanerService>();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Create database if not exists
using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<SauceNaoContext>();
#if DEBUG
    context.Database.EnsureDeleted(); // Delete database
    context.Database.EnsureCreated(); // Create database without migrations
#else
    context.Database.EnsureCreated(); // Create database without migrations
    // context.Database.Migrate(); // Create database using migrations
#endif
}

// Create cache file
using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<CacheDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
