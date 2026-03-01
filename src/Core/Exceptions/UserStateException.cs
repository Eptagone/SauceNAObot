using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Base class for all exceptions thrown by the bot when processing a user state
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
/// <param name="innerException">The inner exception, if any</param>
public abstract class UserStateException(Message receivedMessage, Exception? innerException = null)
    : MessageException(receivedMessage, innerException) { }
