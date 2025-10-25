using SauceNAO.Application.Resources;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.UpdatingMessages;

namespace SauceNAO.Application.Features.Search;

class SauceExceptionHandler(
    ITelegramBotClient client,
    IBetterStringLocalizer<SauceExceptionHandler> localizer
) : IExceptionHandler
{
    public async Task<bool> TryHandleAsync(Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not SauceException e)
        {
            return false;
        }

        var replyToMessageId = e.ReceivedMessage.MessageId;
        int? editMessageId = null;

        if (e is UnsupportedFormatException ue)
        {
            replyToMessageId = ue.ReceivedMessage.MessageId;
        }
        else if (e is TooBigFileException tbf)
        {
            replyToMessageId = tbf.ReceivedMessage.MessageId;
            editMessageId = tbf.SentMessage?.MessageId;
        }

        if (editMessageId.HasValue)
        {
            await client.EditMessageTextAsync(
                e.ReceivedMessage.Chat.Id,
                editMessageId.Value,
                localizer[e.DisplayErrorKey],
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await client.SendMessageAsync(
                e.ReceivedMessage.Chat.Id,
                localizer[e.DisplayErrorKey],
                replyParameters: new ReplyParameters()
                {
                    MessageId = replyToMessageId,
                    AllowSendingWithoutReply = true,
                },
                cancellationToken: cancellationToken
            );
        }

        return true;
    }
}
