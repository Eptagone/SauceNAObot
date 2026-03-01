// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.App.Resources;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.App.Exceptions;

/// <summary>
/// Handles user state exceptions
/// </summary>
sealed class UserStateExceptionHandler(
    IContextProvider contextProvider,
    ITelegramBotClient client,
    IBetterStringLocalizer<UserStateExceptionHandler> localizer
) : IMessageExceptionHandler
{
    public async Task<bool> TryHandleAsync(
        MessageException exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is UserStateException use)
        {
            await this.HandleAsync(use, cancellationToken);
            return true;
        }

        return false;
    }

    public async Task HandleAsync(UserStateException exception, CancellationToken cancellationToken)
    {
        await contextProvider.LoadAsync(exception.ReceivedMessage, cancellationToken);
        if (exception is UserStateCancelledException)
        {
            await client.SendMessageAsync(
                exception.ReceivedMessage.Chat.Id,
                localizer["CancelMessage"],
                replyParameters: new()
                {
                    AllowSendingWithoutReply = true,
                    MessageId = exception.ReceivedMessage.MessageId,
                },
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await client.SendMessageAsync(
                exception.ReceivedMessage.Chat.Id,
                localizer["CommandsDisabled"],
                parseMode: FormatStyles.HTML,
                replyParameters: new()
                {
                    AllowSendingWithoutReply = true,
                    MessageId = exception.ReceivedMessage.MessageId,
                },
                cancellationToken: cancellationToken
            );
        }
    }
}
