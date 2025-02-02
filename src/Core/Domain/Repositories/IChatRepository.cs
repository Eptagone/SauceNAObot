// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Entities.ChatAggregate;

namespace SauceNAO.Domain.Repositories;

/// <summary>
/// Represents a repository for chats.
/// </summary>
public interface IChatRepository : IRepository<TelegramChat>
{
    /// <summary>
    /// Find a chat by their unique identifier.
    /// </summary>
    /// <param name="chatId">Unique identifier for the telegram chat.</param>
    /// <returns><see cref="TelegramChat"/> or <see langword="null"/> if not found.</returns>
    TelegramChat? GetByChatId(long chatId);
}
