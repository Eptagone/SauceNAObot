using SauceNAO.Application.Resources;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Application.Features.ApiKey;

/// <summary>
/// Handles exceptions related to API keys.
/// </summary>
class ApiKeyExceptionHandler(
    ITelegramBotClient client,
    IBetterStringLocalizer<ApiKeyExceptionHandler> localizer,
    IBotHelper helper
) : IExceptionHandler
{
    private string AddNotPremiumMessage => localizer["ApiKeyAddNotPremium"];
    private string MissingApiKey => localizer["ApiKeyCouldNotBeFound"];
    private string NameNotAvailable => localizer["ApiKeyNameNotAvailable"];
    private string ApiKeyAlreadyExistsMessage => localizer["ApiKeyAlreadyExists"];
    private string CouldNotBeAddedMessage => localizer["ApiKeyCouldNotBeAdded"];

    public async Task<bool> TryHandleAsync(Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ApiKeyException e)
        {
            return false;
        }
        var (_, _, languageCode) = await helper.UpsertDataFromMessageAsync(
            e.UserMessage,
            cancellationToken
        );

        if (!string.IsNullOrEmpty(languageCode))
        {
            localizer.ChangeCulture(languageCode);
        }

        var errorMessage = e.ErrorType switch
        {
            ApiKeyErrorType.MissingApiKey => this.MissingApiKey,
            ApiKeyErrorType.NameNotAvailable => this.NameNotAvailable,
            ApiKeyErrorType.ApiKeyAlreadyExists => this.ApiKeyAlreadyExistsMessage,
            ApiKeyErrorType.NotPremium => this.AddNotPremiumMessage,
            _ => this.CouldNotBeAddedMessage,
        };
        var args = new SendMessageArgs(e.UserMessage.Chat.Id, errorMessage)
        {
            ReplyParameters = new ReplyParameters()
            {
                MessageId = e.UserMessage.MessageId,
                AllowSendingWithoutReply = true,
            },
        };

        if (e.ErrorType == ApiKeyErrorType.MissingApiKey)
        {
            args.LinkPreviewOptions = new LinkPreviewOptions { IsDisabled = true };
        }
        else if (e.ErrorType == ApiKeyErrorType.NotPremium)
        {
            args.ReplyMarkup = new ReplyKeyboardRemove();
        }

        await client.SendMessageAsync(args, cancellationToken: cancellationToken);

        return true;
    }
}
