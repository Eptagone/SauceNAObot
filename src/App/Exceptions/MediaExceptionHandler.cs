// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.App.Resources;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Exceptions.Media;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.UpdatingMessages;

namespace SauceNAO.App.Exceptions;

/// <summary>
/// Handler message exceptions related to media
/// </summary>
sealed class MediaExceptionHandler(
    IContextProvider contextProvider,
    IBetterStringLocalizer<MediaExceptionHandler> localizer,
    ITelegramBotClient client
) : IMessageExceptionHandler
{
    public async Task<bool> TryHandleAsync(
        MessageException exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is MessageMediaException mme)
        {
            await this.HandleAsync(mme, cancellationToken);
            return true;
        }
        if (exception is MissingApplicationUrlException maue)
        {
            await this.HandleAsync(maue, cancellationToken);
            return true;
        }

        return false;
    }

    private async Task HandleAsync(MessageException exception, CancellationToken cancellationToken)
    {
        var errorKey = exception.GetType().Name switch
        {
            nameof(InvalidPhotoException) => "InvalidPhoto",
            nameof(UnsupportedFormatException) => "UnsupportedFormat",
            nameof(TooBigFileException) => "TooBigFile",
            nameof(MissingApplicationUrlException) => "MissingApplicationUrl",
            _ => "Unknown Error",
        };

        var message = exception.ReceivedMessage;
        await contextProvider.LoadAsync(message, cancellationToken);

        if (exception.SentMessage is null)
        {
            await client.SendMessageAsync(
                message.Chat.Id,
                localizer[errorKey],
                replyParameters: new()
                {
                    ChatId = message.Chat.Id,
                    MessageId = message.MessageId,
                    AllowSendingWithoutReply = true,
                },
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await client.EditMessageTextAsync(
                exception.SentMessage.Chat.Id,
                exception.SentMessage.MessageId,
                localizer[errorKey],
                parseMode: FormatStyles.HTML,
                cancellationToken: cancellationToken
            );
        }
    }
}
