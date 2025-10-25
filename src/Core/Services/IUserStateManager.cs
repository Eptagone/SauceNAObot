// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Models;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Define methods to manage user states.
/// </summary>
public interface IUserStateManager
{
    /// <summary>
    /// Get the state if it exists.
    /// </summary>
    /// <param name="chatId">The chat identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The user state. <c>null</c> if not found.</returns>
    Task<UserState?> GetAsync(long chatId, long? userId, CancellationToken cancellationToken);

    /// <summary>
    /// Create or update the state of the user.
    /// </summary>
    /// <param name="state">The new user state.</param>
    Task CreateOrUpdateAsync(UserState state, CancellationToken cancellationToken);

    /// <summary>
    /// Remove the state of the user.
    /// </summary>
    /// <param name="state">The user state to remove.</param>
    Task RemoveAsync(UserState state, CancellationToken cancellationToken);
}
