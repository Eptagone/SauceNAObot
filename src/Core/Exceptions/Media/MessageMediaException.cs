// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.Media;

/// <summary>
/// Base class for all exceptions thrown by the bot when processing a message with media
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
public abstract class MessageMediaException(Message receivedMessage)
    : MessageException(receivedMessage)
{
    /// <summary>
    /// The message whose media caused the exception.
    /// </summary>
    public Message MediaMessage { get; set; } = receivedMessage;
}
