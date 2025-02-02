// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Application;
using SauceNAO.Domain;
using SauceNAO.Infrastructure;
using SauceNAO.WebApp;
using SauceNAO.WebApp.Components;
using Vite.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSauceNaoInfrastructure(builder.Configuration);
builder.Services.AddSauceNAOCore();

builder.Services.AddControllers();

// Check if the bot needs to use long polling.
var botSection = builder.Configuration.GetRequiredSection(TelegramBotOptions.SectionName);
var webhookUrl =
    botSection.GetValue<string?>(nameof(TelegramBotOptions.WebhookUrl))
    ?? builder
        .Configuration.GetRequiredSection(GeneralOptions.SectionName)
        .GetValue<string?>(nameof(GeneralOptions.ApplicationURL));
var secretToken = botSection.GetValue<string?>(nameof(TelegramBotOptions.SecretToken));
var useLongPolling = string.IsNullOrEmpty(webhookUrl) || string.IsNullOrEmpty(secretToken);

// If there are not enough parameters to set up a webhook, use long polling instead.
if (useLongPolling)
{
    builder.Services.AddHostedService<LongPollingWorker>();
}

builder.Services.AddRazorComponents();
builder.Services.AddViteServices(options =>
{
    options.Base = "/dist/";
    options.Server.AutoRun = true;
});

var app = builder.Build();

app.UseStaticFiles();

app.UseAntiforgery();
app.MapControllers();
app.MapRazorComponents<App>();

if (app.Environment.IsDevelopment())
{
    app.UseWebSockets();
    app.UseViteDevelopmentServer(true);
}

app.Run();
