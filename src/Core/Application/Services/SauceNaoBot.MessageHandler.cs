// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using SauceNAO.Application.Commands;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Services;

partial class SauceNaoBot : SimpleTelegramBotBase, ISauceNaoBot
{
    /// <inheritdoc />
    protected override Task OnMessageAsync(
        Message message,
        CancellationToken cancellationToken = default
    )
    {
        if (message.From is null)
        {
            return Task.CompletedTask;
        }

        if (
            message.From.Id == TelegramConstants.TelegramId
            || message.From.Id == TelegramConstants.GroupAnonymousBotId
        )
        {
            return Task.CompletedTask;
        }

        // If there are pending states, continue with it.
        var continueStateTask = this.stateManager.ContinueStateAsync(
            message,
            this.Context,
            cancellationToken
        );
        if (continueStateTask is not null)
        {
            return continueStateTask;
        }

        // Get the text or caption from message
        var text = message.Text ?? message.Caption;

        // If message is a command, continue with the base method.
        if (text?.StartsWith('/') == true)
        {
            return base.OnMessageAsync(message, cancellationToken);
        }

        // If chat is private and message was not sent via this bot, try to search.
        if (message.Chat.Type == ChatTypes.Private)
        {
            if (this.Context.User?.PrivateChatStarted == false)
            {
                this.Context.User.PrivateChatStarted = true;
            }
            if (message.ViaBot?.Id != this.Id)
            {
                // Start a new search
                return this.SearchInMessageAsync(message, cancellationToken);
            }
        }

        if (!string.IsNullOrEmpty(text))
        {
            // If the text or caption is equal to "sauce" or contains the bot's username, search the sauce in the message.
            if (
                text.Equals("sauce", StringComparison.InvariantCultureIgnoreCase)
                || text.Contains($"@{this.Username}")
            )
            {
                // Start a new search
                return this.SearchInMessageAsync(message, cancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs a search in the given message.
    /// </summary>
    /// <param name="message">The message to search in.</param>
    private Task SearchInMessageAsync(Message message, CancellationToken cancellationToken)
    {
        // Get the sauce command.
        var sauceCommand = this.serviceProvider.GetRequiredKeyedService<ITelegramBotCommand>(
            "sauce"
        );

        // Call the sauce command.
        sauceCommand.Context = this.Context;
        return sauceCommand.InvokeAsync(message, [], cancellationToken);
    }
}
