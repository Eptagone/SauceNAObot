// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Handles user state
/// </summary>
public interface IUserStateHandler
{
    /// <summary>
    /// Tries to continue the user state based on the message
    /// </summary>
    /// <param name="message">The message sent by the user</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the state was canceled, false otherwise</returns>
    Task<bool> TryContinueAsync(Message message, CancellationToken cancellationToken);
}
