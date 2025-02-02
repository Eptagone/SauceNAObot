// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

[TelegramBotCommand("creator", "Shows the bot creator.", ["botcreador"])]
[BotCommandVisibility(BotCommandVisibility.Hidden)]
class CreatorCommand(ITelegramBotClient client) : BotCommandBase
{
    private string CreatorMessage => this.Context.Localizer["Creator"];

    /// <inheritdoc />
    protected override Task InvokeAsync(
        Message message,
        CancellationToken cancellationToken = default
    )
    {
        return client.SendMessageAsync(
            message.Chat.Id,
            this.CreatorMessage,
            parseMode: FormatStyles.HTML,
            replyParameters: new ReplyParameters
            {
                MessageId = message.MessageId,
                AllowSendingWithoutReply = true,
            },
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
            cancellationToken: cancellationToken
        );
    }
}
