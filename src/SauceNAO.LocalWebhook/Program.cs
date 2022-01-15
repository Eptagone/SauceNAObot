// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Ngrok.AgentAPI;
using SauceNAO.Core;
using SauceNAO.Infrastructure;
using SauceNAO.Infrastructure.Data;
using SauceNAO.Webhook.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Database services
var connectionString = builder.Configuration.GetConnectionString("SauceNAO");
//builder.Services.AddDbContext<SauceNaoContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDbContext<SauceNaoContext>(options => options.UseSqlServer(connectionString));

var cacheConnection = $"Data Source={Path.GetTempFileName()}";
builder.Services.AddDbContext<CacheContext>(options => options.UseSqlite(cacheConnection));

builder.Services.AddScoped<IBotDb, BotDb>(); // Bot data class
builder.Services.AddScoped<IBotCache, BotCache>(); // Bot cache class

// Ensure start ngrok tunnel
string appUrl;
{
    var agent = new NgrokAgentClient();
    var ngrok = builder.Configuration.GetSection("Ngrok");
    var tunnelName = ngrok["TunnelName"] ?? "SnaoTunnel";
    var tunnel = agent.ListTunnels().Tunnels.FirstOrDefault(t => t.Name == tunnelName);

    if (tunnel == null)
    {
        var port = ngrok["Port"];
        var hostheader = string.Format("localhost:{0}", port);
        var address = string.Format("https://{0}", hostheader);

        var tunnelConfig = new TunnelConfiguration(tunnelName, "http", address)
        {
            HostHeader = hostheader
        };
        tunnel = agent.StartTunnel(tunnelConfig);
    }
    appUrl = tunnel.PublicUrl;
}

// Add Telegram Bot Configurtaion
builder.Services.AddSingleton<SnaoBotProperties>(services =>
{
    var telegram = builder.Configuration.GetSection("Telegram");
    var snao = builder.Configuration.GetSection("SauceNAO");

    var botToken = telegram["BotToken"];
    var apikey = snao["ApiKey"];
    var ffmpegExec = builder.Configuration["FFmpegExec"];
    var supportChatLink = telegram["SupportChatLink"];

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

app.MapControllers();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();
