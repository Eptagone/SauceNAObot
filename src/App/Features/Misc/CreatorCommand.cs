// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.App.Resources;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Features.Misc;

[
    BotCommand("creator", "Shows the creator of the bot"),
    BotCommandVisibility(BotCommandVisibility.Hidden)
]
sealed class CreatorCommand(
    IContextProvider contextProvider,
    ITelegramBotClient client,
    IBetterStringLocalizer<CreatorCommand> localizer
) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        await contextProvider.LoadAsync(message, cancellationToken);
        await client.SendMessageAsync(
            message.Chat.Id,
            localizer["Creator"],
            parseMode: FormatStyles.HTML,
            replyParameters: new()
            {
                AllowSendingWithoutReply = true,
                MessageId = message.MessageId,
            },
            cancellationToken: cancellationToken
        );
    }
}
