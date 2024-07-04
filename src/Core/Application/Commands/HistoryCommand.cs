// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Repositories;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

[TelegramBotCommand("history", "Your sauce history", ["historial"])]
[BotCommandVisibility(BotCommandVisibility.PrivateChat)]
class HistoryCommand(ITelegramBotClient client, IUserRepository userRepository) : BotCommandBase
{
    private string HistoryMessage => this.Context.Localizer["HistoryMessage"];
    private string HistoryButtonLabel => this.Context.Localizer["HistoryButtonLabel"];
    private string HistoryErasedMessage => this.Context.Localizer["HistoryErased"];

    /// <inheritdoc />
    protected override Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken = default
    )
    {
        var action = args.FirstOrDefault();

        switch (action)
        {
            case "clear":
            case "clean":
            case "limpiar":
            case "borrar":
            case "vaciar":
                var actionTask = client.SendChatActionAsync(
                    message.Chat.Id,
                    ChatActions.Typing,
                    cancellationToken: cancellationToken
                );

                this.User!.SearchHistory.Clear();
                userRepository.Update(this.User);

                actionTask.Wait(cancellationToken);
                return client.SendMessageAsync(
                    message.Chat.Id,
                    this.HistoryErasedMessage,
                    parseMode: FormatStyles.HTML,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true
                    },
                    cancellationToken: cancellationToken
                );

            default:
                return client.SendMessageAsync(
                    message.Chat.Id,
                    this.HistoryMessage,
                    parseMode: FormatStyles.HTML,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true
                    },
                    replyMarkup: new InlineKeyboardMarkup(
                        new InlineKeyboardBuilder().AppendSwitchInlineQueryCurrentChat(
                            this.HistoryButtonLabel,
                            string.Empty
                        )
                    ),
                    cancellationToken: cancellationToken
                );
        }
    }
}
