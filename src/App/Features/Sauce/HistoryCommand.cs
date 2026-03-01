// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.App.Resources;
using SauceNAO.Core.Data;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Features.Sauce;

[BotCommand("history", "Manage your found sauces")]
[BotCommandVisibility(BotCommandVisibility.PrivateChats)]
[LocalizedBotCommand("es", "historial", "Administrar tus salsas encontradas")]
class HistoryCommand(
    IContextProvider contextProvider,
    ISearchHistoryRepository historyRepository,
    IBetterStringLocalizer<HistoryCommand> localizer,
    ITelegramBotClient client
) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        if (message.Chat.Type == ChatTypes.Private)
        {
            await contextProvider.LoadAsync(message, cancellationToken);

            var action = args.FirstOrDefault();
            switch (action)
            {
                case "clear":
                case "clean":
                case "limpiar":
                case "borrar":
                case "vaciar":
                    await client.SendChatActionAsync(
                        message.Chat.Id,
                        ChatActions.Typing,
                        cancellationToken: cancellationToken
                    );
                    await historyRepository.ClearUserHistoryAsync(
                        message.From!.Id,
                        cancellationToken
                    );

                    await client.SendMessageAsync(
                        message.Chat.Id,
                        localizer["HistoryCleared"],
                        replyParameters: new()
                        {
                            AllowSendingWithoutReply = true,
                            MessageId = message.MessageId,
                        },
                        cancellationToken: cancellationToken
                    );
                    break;

                default:
                    await client.SendMessageAsync(
                        message.Chat.Id,
                        localizer["HistoryHelp"],
                        parseMode: FormatStyles.HTML,
                        replyParameters: new ReplyParameters
                        {
                            MessageId = message.MessageId,
                            AllowSendingWithoutReply = true,
                        },
                        replyMarkup: new InlineKeyboardMarkup(
                            new InlineKeyboardBuilder().AppendSwitchInlineQueryCurrentChat(
                                localizer["HistoryHelpButton"],
                                string.Empty
                            )
                        ),
                        cancellationToken: cancellationToken
                    );
                    break;
            }
        }
    }
}
