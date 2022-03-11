// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Infrastructure;
using SauceNAO.Infrastructure.Data;
using SauceNAO.Webhook.Services;
using SauceNAO.Core.Extensions;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure database context
var connectionString = builder.Configuration.GetConnectionString("SauceNAO");
builder.Services.AddDbContext<SauceNaoContext>(options => options.UseSqlServer(connectionString));

// Configure cache context
var cacheConnection = $"Data Source={Path.GetTempFileName()}"; // Get connection string for cache
builder.Services.AddDbContext<CacheDbContext>(options => options.UseSqlite(cacheConnection));

// Add bot service.
builder.Services.AddSauceBot<BotDb>(builder.Configuration);

// Add Data Cleaner service
builder.Services.AddHostedService<CleanerService>();

var app = builder.Build();

// Create database if not exists
using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<SauceNaoContext>();
#if DEBUG
    context.Database.EnsureDeleted(); // Delete database
    context.Database.EnsureCreated(); // Create database without migrations
#else
    context.Database.Migrate(); // Create database using migrations
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
