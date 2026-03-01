using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.ApiKeys;

/// <summary>
/// Base class for all exceptions thrown by the bot when processing the api key command and related services
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
/// <param name="innerException">The inner exception, if any</param>
public abstract class ApiKeyException(Message receivedMessage, Exception? innerException = null)
    : MessageException(receivedMessage, innerException) { }
