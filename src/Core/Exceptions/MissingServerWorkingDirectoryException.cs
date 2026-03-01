using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Thrown when the bot is unable to process a request because the working directory for TBAS was not set
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
public sealed class MissingServerWorkingDirectoryException(Message receivedMessage)
    : MessageException(receivedMessage) { }
