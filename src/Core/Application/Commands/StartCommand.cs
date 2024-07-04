// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

/// <summary>
/// Represents a command to start the bot.
/// </summary>
[TelegramBotCommand("start", "Starts the bot.")]
sealed class StartCommand(ITelegramBotClient client) : BotCommandBase
{
    private string AboutMessage => this.Context.Localizer["About"];

    /// <inheritdoc />
    protected override Task InvokeAsync(
        Message message,
        CancellationToken cancellationToken = default
    )
    {
        return client.SendMessageAsync(
            message.Chat.Id,
            this.AboutMessage,
            parseMode: FormatStyles.HTML,
            replyParameters: new ReplyParameters
            {
                MessageId = message.MessageId,
                AllowSendingWithoutReply = true
            },
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true, },
            cancellationToken: cancellationToken
        );
    }
}
