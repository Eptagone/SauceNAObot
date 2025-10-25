// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SauceNAO.Core;
using SauceNAO.Core.Configuration;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
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
        var defaultCommands = BotCommandDirectory
            .GetAll()
            .Where(cmd => cmd.Scope == BotCommandScopeValue.Default);
        await this.RegisterCommandsAsync(
            defaultCommands,
            new BotCommandScopeAllPrivateChats(),
            cancellationToken
        );

        // Registrer private chat commands.
        var privateChatCommands = BotCommandDirectory
            .GetAll()
            .Where(cmd => cmd.Scope == BotCommandScopeValue.PrivateChat);
        if (privateChatCommands.Where(cmd => !defaultCommands.Contains(cmd)).Any())
        {
            await this.RegisterCommandsAsync(
                privateChatCommands,
                new BotCommandScopeAllPrivateChats(),
                cancellationToken
            );
        }

        // Register group commands.
        var groupCommands = BotCommandDirectory
            .GetAll()
            .Where(cmd => cmd.Scope == BotCommandScopeValue.Group);

        if (groupCommands.Where(cmd => !defaultCommands.Contains(cmd)).Any())
        {
            await this.RegisterCommandsAsync(
                groupCommands,
                new BotCommandScopeAllGroupChats(),
                cancellationToken
            );
        }

        // Register group admin commands.
        var groupAdminCommands = BotCommandDirectory
            .GetAll()
            .Where(cmd => cmd.Scope == BotCommandScopeValue.GroupAdmin);
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
        IEnumerable<BotCommandDetails> commands,
        BotCommandScope scope,
        CancellationToken cancellationToken
    )
    {
        var botCommands = commands.Select(cmd => new BotCommand(cmd.Name, cmd.Description));
        await client
            .DeleteMyCommandsAsync(scope, cancellationToken: cancellationToken)
            .ContinueWith(
                (_) =>
                    client.SetMyCommandsAsync(
                        botCommands,
                        scope,
                        cancellationToken: cancellationToken
                    )
            );
    }

    /// <inheritdoc />
    Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
