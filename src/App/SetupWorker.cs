// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Reflection;
using Castle.Core.Internal;
using Microsoft.Extensions.Options;
using SauceNAO.Core.Configuration;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.App;

sealed class SetupWorker(
    IServiceProvider serviceProvider,
    IOptions<AppConfiguration> appOptions,
    IOptions<BotConfiguration> botOptions
) : BackgroundService
{
    private readonly AppConfiguration appConfiguration = appOptions.Value;
    private readonly BotConfiguration botConfiguration = botOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        await SetupCommandsAsync(client, stoppingToken);
        await this.ConfigureUpdateStrategyAsync(client, stoppingToken);
    }

    private static async Task SetupCommandsAsync(
        ITelegramBotClient client,
        CancellationToken stoppingToken
    )
    {
        var definitions = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(ICommandHandler)))
            .Aggregate(
                new List<SetMyCommandsArgs>(),
                (acc, t) =>
                {
                    var attr = t.GetAttribute<BotCommandAttribute>();
                    var command = new BotCommand(attr.Command, attr.Description);
                    var visibility = t.GetAttribute<BotCommandVisibilityAttribute?>()?.Visibility;
                    // Ignore hidden commands
                    if (visibility?.HasFlag(BotCommandVisibility.Hidden) == true)
                    {
                        return acc;
                    }
                    // Get scopes
                    var scopes = new List<BotCommandScope?>();
                    foreach (var v in Enum.GetValues<BotCommandVisibility>())
                    {
                        if (visibility?.HasFlag(v) == true)
                        {
                            BotCommandScope? scope = v switch
                            {
                                BotCommandVisibility.PrivateChats =>
                                    new BotCommandScopeAllPrivateChats(),
                                BotCommandVisibility.Members => new BotCommandScopeAllGroupChats(),
                                BotCommandVisibility.Administrators =>
                                    new BotCommandScopeAllChatAdministrators(),
                                _ => null,
                            };
                            scopes.Add(scope);
                        }
                    }
                    if (scopes.Count == 0)
                    {
                        scopes.Add(null);
                    }

                    var translations = t.GetAttributes<LocalizedBotCommandAttribute>()
                        .Select(a => new
                        {
                            a.LanguageCode,
                            Command = new BotCommand(a.Command ?? attr.Command, a.Description),
                        });

                    // For each scope, define the command and translations
                    foreach (var scope in scopes)
                    {
                        var item = acc.FirstOrDefault(a => a.Scope?.Type == scope?.Type);
                        if (item is null)
                        {
                            item = new SetMyCommandsArgs([command]) { Scope = scope };
                            acc.Add(item);
                        }
                        else
                        {
                            item.Commands = item.Commands.Append(command);
                        }

                        foreach (var tc in translations)
                        {
                            var tItem = acc.FirstOrDefault(a =>
                                a.Scope?.Type == scope?.Type && a.LanguageCode == tc.LanguageCode
                            );
                            if (tItem is null)
                            {
                                tItem = new SetMyCommandsArgs([tc.Command])
                                {
                                    Scope = scope,
                                    LanguageCode = tc.LanguageCode,
                                };
                                acc.Add(tItem);
                            }
                            else
                            {
                                tItem.Commands = tItem.Commands.Append(tc.Command);
                            }
                        }
                    }

                    return acc;
                },
                acc =>
                {
                    // Merge default commands with the rest of the commands
                    foreach (var item in acc.Where(a => a.Scope is not null))
                    {
                        var @default = acc.FirstOrDefault(d =>
                            d.Scope is null && d.LanguageCode == item.LanguageCode
                        );
                        if (@default?.Commands is not null)
                        {
                            item.Commands = item.Commands.Concat(@default.Commands);
                        }
                    }
                    return acc;
                }
            );

        foreach (var definition in definitions)
        {
            await client.DeleteMyCommandsAsync(
                definition.Scope,
                definition.LanguageCode,
                stoppingToken
            );
            await client.SetMyCommandsAsync(
                definition.Commands,
                definition.Scope,
                definition.LanguageCode,
                stoppingToken
            );
        }
    }

    private async Task ConfigureUpdateStrategyAsync(
        ITelegramBotClient client,
        CancellationToken stoppingToken
    )
    {
        // Delete the previous webhook if it is configured.
        await client.DeleteWebhookAsync(cancellationToken: stoppingToken);

        var webhookUrl = this.botConfiguration.WebhookUrl ?? this.appConfiguration.ApplicationUrl;

        if (
            !string.IsNullOrEmpty(webhookUrl)
            && !string.IsNullOrEmpty(this.botConfiguration.SecretToken)
        )
        {
            var url = new Uri(new Uri(webhookUrl), "webhook");
            await client.SetWebhookAsync(
                url.AbsoluteUri,
                secretToken: this.botConfiguration.SecretToken,
                cancellationToken: stoppingToken
            );
        }
    }
}
