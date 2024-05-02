// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Application.Models;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Services;

/// <summary>
/// Define methods to manage user states.
/// </summary>
public interface IUserStateManager
{
    /// <summary>
    /// Continue the state of the user if any.
    /// </summary>
    /// <param name="message">The message sent by the user.</param>
    /// <param name="context">The bot context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if a pending state was processed, otherwise false.</returns>
    Task? ContinueStateAsync(
        Message message,
        ISauceNaoContext context,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Create or update the state of the user.
    /// </summary>
    /// <param name="state">The new user state.</param>
    void CreateOrUpdateState(UserState state);

    /// <summary>
    /// Remove the state of the user.
    /// </summary>
    /// <param name="state">The user state to remove.</param>
    void RemoveState(UserState state);
}
