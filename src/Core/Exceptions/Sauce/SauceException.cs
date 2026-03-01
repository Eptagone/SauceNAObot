// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.Sauce;

/// <summary>
/// Thrown when an error occurs while looking for the image sauce of a message.
/// </summary>
/// <param name="receivedMessage">The telegram message that triggered the exception</param>
/// <param name="innerException">The inner exception, if any</param>
public class SauceException(Message receivedMessage, Exception? innerException = null)
    : MessageException(receivedMessage, innerException) { }
