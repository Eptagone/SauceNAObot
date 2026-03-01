// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.ApiKeys;

/// <summary>
/// Thrown when the user attempts to Add a non-premium api key
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
public sealed class ApiKeyNotPremiumException(Message receivedMessage)
    : ApiKeyException(receivedMessage) { }
