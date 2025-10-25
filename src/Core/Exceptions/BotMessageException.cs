using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Represents an exception that is related to a message received by the bot.
/// </summary>
/// <param name="message">The message received by the bot.</param>
public abstract class BotMessageException(Message message) : Exception
{
    /// <summary>
    /// The message received by the bot.
    /// </summary>
    public Message ReceivedMessage => message;

    /// <summary>
    /// The error key associated with the exception.
    /// Used to retrieve the error message and display it to the user.
    /// </summary>
    public abstract string DisplayErrorKey { get; }
}
