// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core;
using SauceNAO.Infrastructure;
using SauceNAO.Infrastructure.Data;
using SauceNAO.Webhook.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Database services
builder.Services.AddDbContext<SauceNaoContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SauceNAO")));

var cacheConnection = $"Data Source={Path.GetTempFileName()}";
builder.Services.AddDbContext<CacheContext>(options => options.UseSqlite(cacheConnection));

builder.Services.AddScoped<IBotDb, BotDb>(); // Bot data class
builder.Services.AddScoped<IBotCache, BotCache>(); // Bot cache class

// Add Telegram Bot Configurtaion
builder.Services.AddSingleton<SnaoBotProperties>(services =>
{
    var telegram = builder.Configuration.GetSection("Telegram");
    var snao = builder.Configuration.GetSection("SauceNAO");

    var botToken = telegram["BotToken"];
    var apikey = snao["ApiKey"];
    var ffmpegExec = builder.Configuration["FFmpegExec"];
    var supportChatLink = telegram["SupportChatLink"];
    var appUrl = builder.Configuration["AplicationUrl"];

    var filesUrl = $"{appUrl}/temp/{{0}}";

    var botConfiguration = new SnaoBotProperties(botToken, apikey, filesUrl, ffmpegExec, supportChatLink);

    var accessToken = builder.Configuration["AccessToken"];
    var webhook = $"{appUrl}/bot/{accessToken}";

    botConfiguration.SetBotCommands();
    botConfiguration.SetWebhook(webhook);

    return botConfiguration;
});

builder.Services.AddScoped<SauceNaoBot>();

// Add Data Cleaner service
builder.Services.AddHostedService<CleanerService>();

var app = builder.Build();

// Create database if not exists
using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<SauceNaoContext>();
    // context.Database.Migrate(); // Create database using migrations
    context.Database.EnsureCreated(); // Create database without migrations
}

// Create cache file
using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<CacheContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
