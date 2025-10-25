using SauceNAO.Application.Resources;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Services;

/// <summary>
/// Detect pending user states and invoke the corresponding handler for them.
/// </summary>
class UserStateMessageHandler(
    IUserStateManager manager,
    IBotHelper helper,
    IUserStateHandler[] stateHandlers,
    ITelegramBotClient client,
    IBetterStringLocalizer<UserStateMessageHandler> localizer,
    ILogger<UserStateMessageHandler> logger
) : IMessageHandler
{
    public async Task<bool> HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var state = await manager.GetAsync(message.Chat.Id, message.From?.Id, cancellationToken);
        if (state is null)
        {
            return false;
        }

        // If the user wants to cancel the current state, remove the state.
        if (message.Text?.StartsWith("/cancel", StringComparison.OrdinalIgnoreCase) is true)
        {
            await client.SendChatActionAsync(
                message.Chat.Id,
                ChatActions.Typing,
                cancellationToken: cancellationToken
            );
            await manager.RemoveAsync(state, cancellationToken);
            var (_, _, languageCode) = helper
                .UpsertDataFromMessageAsync(message, cancellationToken)
                .Result;
            if (!string.IsNullOrEmpty(languageCode))
            {
                localizer.ChangeCulture(languageCode);
            }

            // Send the message.
            await client.SendMessageAsync(
                message.Chat.Id,
                localizer["CancelMessage"],
                replyParameters: new ReplyParameters
                {
                    MessageId = message.MessageId,
                    AllowSendingWithoutReply = true,
                },
                cancellationToken: cancellationToken
            );
            return true;
        }

        var handler = stateHandlers.FirstOrDefault(h => h.CanHandleState(state));
        if (handler is null)
        {
            logger.LogFailedToHandleUserState(state.Scope, message.Chat.Id, message.From?.Id);
        }
        else
        {
            await handler.ContinueAsync(message, state, cancellationToken);
        }

        return true;
    }
}

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 20,
        Level = LogLevel.Critical,
        Message = "Failed to handle state {Scope} for chat {ChatId} and user {UserId}."
    )]
    internal static partial void LogFailedToHandleUserState(
        this ILogger<UserStateMessageHandler> logger,
        string scope,
        long chatId,
        long? userId
    );
}
