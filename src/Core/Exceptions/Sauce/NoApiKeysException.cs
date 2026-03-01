// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.Sauce;

/// <summary>
/// Thrown when the user does not have any api keys to perform the search
/// </summary>
/// <param name="receivedMessage">The telegram message that triggered the exception</param>
/// <param name="innerException">The inner exception, if any</param>
public sealed class NoApiKeysException(Message receivedMessage, Exception? innerException = null)
    : SauceException(receivedMessage, innerException) { }
