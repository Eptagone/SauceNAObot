using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.ApiKey;

/// <summary>
/// Represents an exception related to API keys.
/// </summary>
/// <param name="message">The message received by the bot.</param>
/// <param name="errorType">The type of API key error.</param>
/// <param name="languageCode">The language code of the user.</param>
sealed class ApiKeyException(Message message, ApiKeyErrorType errorType, string? languageCode)
    : Exception
{
    /// <summary>
    /// The message received by the bot.
    /// </summary>
    public Message UserMessage { get; } = message;

    /// <summary>
    /// The type of API key error.
    /// </summary>
    public ApiKeyErrorType ErrorType { get; } = errorType;
}
