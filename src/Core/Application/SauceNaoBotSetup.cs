// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SauceNAO.Application.Commands;
using SauceNAO.Domain;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Application;

/// <summary>
/// Represents a background service that sets up the SauceNAO bot application.
/// </summary>
/// <param name="logger">The logger.</param>
/// <param name="client">The Telegram Bot API client.</param>
class SauceNaoBotSetup(
    ILogger<SauceNaoBotSetup> logger,
    ITelegramBotClient client,
    IOptions<GeneralOptions> options,
    IOptions<TelegramBotOptions> botOptions
) : IHostedService
{
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogSettingUpBotCommands();

        // Delete all commands.
        await client.DeleteMyCommandsAsync(cancellationToken: cancellationToken);
        await client.DeleteMyCommandsAsync(
            scope: new BotCommandScopeAllPrivateChats(),
            cancellationToken: cancellationToken
        );
        await client.DeleteMyCommandsAsync(
            scope: new BotCommandScopeAllGroupChats(),
            cancellationToken: cancellationToken
        );
        await client.DeleteMyCommandsAsync(
            scope: new BotCommandScopeAllChatAdministrators(),
            cancellationToken: cancellationToken
        );

        // Register the default commands.
        var defaultCommands = CommandDirectory.GetDefaultCommands();
        await this.RegisterCommandsAsync(
            defaultCommands,
            new BotCommandScopeAllPrivateChats(),
            cancellationToken
        );

        // Registrer private chat commands.
        var privateChatCommands = CommandDirectory.GetPrivateChatCommands();
        if (privateChatCommands.Where(cmd => !defaultCommands.Contains(cmd)).Any())
        {
            await this.RegisterCommandsAsync(
                privateChatCommands,
                new BotCommandScopeAllPrivateChats(),
                cancellationToken
            );
        }

        // Register group commands.
        var groupCommands = CommandDirectory.GetGroupCommands();
        if (groupCommands.Where(cmd => !defaultCommands.Contains(cmd)).Any())
        {
            await this.RegisterCommandsAsync(
                groupCommands,
                new BotCommandScopeAllGroupChats(),
                cancellationToken
            );
        }

        // Register group admin commands.
        var groupAdminCommands = CommandDirectory.GetGroupAdminCommands();
        if (groupAdminCommands.Where(cmd => !defaultCommands.Contains(cmd)).Any())
        {
            await this.RegisterCommandsAsync(
                groupAdminCommands,
                new BotCommandScopeAllGroupChats(),
                cancellationToken
            );
        }

        logger.LogCommandsRegistered();

        // Delete the previous webhook if it is configured.
        await client.DeleteWebhookAsync(cancellationToken: cancellationToken);

        var webhookUrl = botOptions.Value.WebhookUrl ?? options.Value.ApplicationURL;
        var secretToken = botOptions.Value.SecretToken;

        // Setup the webhook if it is configured.
        if (!string.IsNullOrEmpty(webhookUrl) && !string.IsNullOrEmpty(secretToken))
        {
            var url = new Uri(new Uri(webhookUrl), "/webhook");

            logger.LogWebhookSetUp();
            await client.SetWebhookAsync(
                url.ToString(),
                secretToken: secretToken,
                cancellationToken: cancellationToken
            );
            logger.LogWebhookSetUpSuccessful(webhookUrl);
        }

        logger.LogSetupCompleted();
    }

    // Register the given extended bot commands.
    private async Task RegisterCommandsAsync(
        IEnumerable<IExtendedBotCommand> commands,
        BotCommandScope scope,
        CancellationToken cancellationToken
    )
    {
        var languageCodes = commands.SelectMany(command => command.LanguageCodes).Distinct();

        if (languageCodes.Count() > 1)
        {
            foreach (var languageCode in languageCodes)
            {
                var botCommands = commands.Select(cmd => new BotCommand(
                    cmd.GetTranslatedName(languageCode) ?? cmd.Command,
                    cmd.GetTranslatedDescription(languageCode) ?? cmd.Description
                ));

                await client.SetMyCommandsAsync(
                    botCommands,
                    scope,
                    languageCode,
                    cancellationToken
                );
            }
        }
        else
        {
            var botCommands = commands.Select(cmd => new BotCommand(cmd.Command, cmd.Description));

            await client.SetMyCommandsAsync(
                botCommands,
                scope,
                cancellationToken: cancellationToken
            );
        }
    }

    /// <inheritdoc />
    Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
