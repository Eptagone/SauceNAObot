using SauceNAO.App.Resources;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Exceptions.ApiKeys;
using SauceNAO.Core.Exceptions.Sauce;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.App.Exceptions;

/// <summary>
/// Handles exceptions related to API keys
/// </summary>
sealed class ApiKeyExceptionHandler(
    IContextProvider contextProvider,
    IBetterStringLocalizer<ApiKeyExceptionHandler> localizer,
    ITelegramBotClient client
) : IMessageExceptionHandler
{
    public async Task<bool> TryHandleAsync(
        MessageException exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is ApiKeyException ae)
        {
            await this.HandleAsync(ae, cancellationToken);
            return true;
        }
        return false;
    }

    private async Task HandleAsync(ApiKeyException exception, CancellationToken cancellationToken)
    {
        await client.SendChatActionAsync(
            exception.ReceivedMessage.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );
        var text = exception.GetType().Name switch
        {
            nameof(ApiKeyNameAlreadyExistsException) => localizer["NameAlreadyExists"],
            nameof(ApiKeyAlreadyExistsException) => localizer["AlreadyExists"],
            nameof(ApiKeyNotPremiumException) => localizer["NotPremium"],
            nameof(ApiKeyValidationException) => exception.InnerException?.GetType().Name switch
            {
                nameof(InvalidApiKeyException) => localizer["InvalidKey"],
                nameof(SearchLimitReachedException) => localizer["SearchLimitReached"],
                _ => localizer["CannotValidateKey"],
            },
            _ => localizer["UnknownError"],
        };
        await contextProvider.LoadAsync(exception.ReceivedMessage, cancellationToken);
        await client.SendMessageAsync(
            exception.ReceivedMessage.Chat.Id,
            text,
            replyParameters: new()
            {
                AllowSendingWithoutReply = true,
                MessageId = exception.ReceivedMessage.MessageId,
            },
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken
        );
    }
}
