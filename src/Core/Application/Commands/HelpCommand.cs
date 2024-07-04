// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Options;
using SauceNAO.Domain;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

/// <summary>
/// Represents a command that shows how to use the bot.
/// </summary>
/// <param name="configuration">The configuration object.</param>
[TelegramBotCommand("help", "How to use the bot.", ["ayuda"])]
class HelpCommand(ITelegramBotClient client, IOptions<GeneralOptions> options) : BotCommandBase
{
    private readonly string? SupportChatLink = options.Value.SupportChatInvitationLink;
    private string SupportChatText => this.Context.Localizer["SupportChat"];
    private string HelpMessage => this.Context.Localizer["About"];

    /// <inheritdoc />
    protected override Task InvokeAsync(
        Message message,
        CancellationToken cancellationToken = default
    )
    {
        var keyboard = string.IsNullOrEmpty(this.SupportChatLink)
            ? null
            : new InlineKeyboardMarkup(
                new InlineKeyboardBuilder().AppendUrl(this.SupportChatText, this.SupportChatLink)
            );

        return client.SendMessageAsync(
            message.Chat.Id,
            this.HelpMessage,
            parseMode: FormatStyles.HTML,
            replyParameters: new ReplyParameters
            {
                MessageId = message.MessageId,
                AllowSendingWithoutReply = true
            },
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true, },
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }
}
