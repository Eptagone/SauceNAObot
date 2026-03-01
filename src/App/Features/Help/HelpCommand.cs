// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Options;
using SauceNAO.App.Resources;
using SauceNAO.Core.Configuration;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Features.Help;

[BotCommand("help", "How to use", ["ayuda"]), LocalizedBotCommand("es", "Como usar")]
sealed class HelpCommand(
    IContextProvider contextProvider,
    IOptions<AppConfiguration> options,
    ITelegramBotClient client,
    IBetterStringLocalizer<HelpCommand> localizer
) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        await contextProvider.LoadAsync(message, cancellationToken);

        var builder = new InlineKeyboardBuilder();
        if (!string.IsNullOrEmpty(options.Value.SupportChatInvitationLink))
        {
            builder.AppendUrl(localizer["SupportChat"], options.Value.SupportChatInvitationLink);
        }
        var replyMarkup = builder.Any() ? new InlineKeyboardMarkup(builder) : null;
        await client.SendMessageAsync(
            message.Chat.Id,
            localizer["Help"],
            parseMode: FormatStyles.HTML,
            replyParameters: new()
            {
                AllowSendingWithoutReply = true,
                MessageId = message.MessageId,
            },
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
        );
    }
}
