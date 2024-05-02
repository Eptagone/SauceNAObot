// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Entities.UserAggregate;

namespace SauceNAO.Domain.Repositories;

/// <summary>
/// Represents the user repository.
/// </summary>
public interface IUserRepository : IRepository<TelegramUser>
{
    /// <summary>
    /// Find a user by their unique identifier.
    /// </summary>
    /// <param name="userId">Unique identifier for the telegram user.</param>
    /// <returns><see cref="TelegramUser"/> or <see langword="null"/> if not found.</returns>
    TelegramUser? GetByUserId(long userId);
}
