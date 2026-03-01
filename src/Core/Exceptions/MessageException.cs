// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Base class for all exceptions thrown by the bot when processing a message.
/// </summary>
/// <param name="receivedMessage">The telegram message that triggered the exception</param>
/// <param name="innerException">The inner exception, if any</param>
public abstract class MessageException(Message receivedMessage, Exception? innerException = null)
    : Exception(null, innerException)
{
    /// <summary>
    /// The message sent by the user that triggered the exception
    /// </summary>
    public Message ReceivedMessage { get; set; } = receivedMessage;

    /// <summary>
    /// If the bot already replied to the user, this is the message that was sent
    /// </summary>
    public Message? SentMessage { get; set; }
}
