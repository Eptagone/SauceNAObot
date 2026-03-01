using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Thrown when the bot is unable to process a request because the specified language code is invalid
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
/// <param name="languageCode">The language code that caused the exception</param>
public sealed class InvalidLanguageCodeException(Message receivedMessage, string languageCode)
    : MessageException(receivedMessage)
{
    public string LanguageCode = languageCode;
}
