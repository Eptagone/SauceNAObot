using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.ApiKeys;

/// <summary>
/// Thrown when the bot is unable to validate the api key.
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
/// <param name="innerException">The inner exception, if any</param>
public sealed class ApiKeyValidationException(
    Message receivedMessage,
    Exception? innerException = null
) : ApiKeyException(receivedMessage, innerException) { }
