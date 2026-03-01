using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Thrown when the user attempts to use a command that is not allowed in the current user state
/// </summary>
/// <param name="receivedMessage"></param>
public sealed class UserStateDisallowedCommandException(Message receivedMessage)
    : UserStateException(receivedMessage) { }
