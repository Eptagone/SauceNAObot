// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Data;

/// <summary>
/// Provides a helper to manage group data.
/// </summary>
public interface IChatRepository : IRepository<ChatEntity>
{
    /// <summary>
    /// Retrieves the updated group preferences from the database or creates a entry if it doesn't exist.
    /// If the group already exists, the existing data is updated with the provided information.
    /// </summary>
    /// <param name="message">The message containing the group data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated Telegram group data.</returns>
    Task<ChatEntity> UpsertFromMessageAsync(Message message, CancellationToken cancellationToken);
}
