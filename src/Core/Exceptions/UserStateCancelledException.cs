using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Thrown when the user has cancelled the current user state
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
public sealed class UserStateCancelledException(Message receivedMessage)
    : UserStateException(receivedMessage) { }
