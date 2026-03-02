// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities.UserAggregate;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Data;

/// <summary>
/// Provides a helper to manage user data.
/// </summary>
public interface IUserRepository : IRepository<UserEntity>
{
    /// <summary>
    /// Retrieves the updated user preferences from the database or creates a profile for the user if it doesn't exist.
    /// If the user already exists, the existing data is updated with the provided information.
    /// </summary>
    /// <param name="user">The Telegram user.</param>
    /// param name="startDm">Whether the user started a direct message conversation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated Telegram user data.</returns>
    Task<UserEntity> UpsertAsync(User user, bool isDm, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the list of language codes used by users.
    /// </summary>
    /// <returns>A dictionary of language codes and their number of occurrences.</returns>
    Task<IReadOnlyDictionary<string, int>> GetLanguageCodesAsync(
        CancellationToken cancellationToken
    );
}
