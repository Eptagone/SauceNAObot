// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Thrown when an unknown exception occurs while processing a message
/// </summary>
/// <param name="receivedMessage">The telegram message that triggered the exception</param>
/// <param name="innerException">The inner exception, if any</param>
public sealed class UnknownMessageException(Message receivedMessage, Exception innerException)
    : MessageException(receivedMessage, innerException) { }
