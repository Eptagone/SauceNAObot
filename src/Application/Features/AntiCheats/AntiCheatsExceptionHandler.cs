using SauceNAO.Application.Resources;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.AntiCheats;

class AntiCheatsExceptionHandler(
    IBotHelper helper,
    ITelegramBotClient client,
    IBetterStringLocalizer<AntiCheatsExceptionHandler> localizer
) : IExceptionHandler
{
    public async Task<bool> TryHandleAsync(Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not AntiCheatsException e)
        {
            return false;
        }

        var (_, _, languageCode) = await helper.UpsertDataFromMessageAsync(
            e.ReceivedMessage,
            cancellationToken
        );

        if (!string.IsNullOrEmpty(languageCode))
        {
            localizer.ChangeCulture(languageCode);
        }

        var errorMessage = exception switch
        {
            AlreadyAddedAntiCheatsException aa => localizer[
                aa.DisplayErrorKey,
                aa.Target.FirstName
            ],
            AlreadyRemovedAntiCheatsException ar => localizer[
                ar.DisplayErrorKey,
                ar.Target.FirstName
            ],
            _ => localizer[e.DisplayErrorKey],
        };

        await client.SendMessageAsync(
            e.ReceivedMessage.Chat.Id,
            errorMessage,
            replyParameters: new ReplyParameters
            {
                MessageId = e.ReceivedMessage.MessageId,
                AllowSendingWithoutReply = true,
            },
            cancellationToken: cancellationToken
        );

        return true;
    }
}
