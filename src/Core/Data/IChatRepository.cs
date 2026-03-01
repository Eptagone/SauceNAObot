using SauceNAO.Core.Entities;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Data;

/// <summary>
/// Provides a helper to manage group data.
/// </summary>
public interface IChatRepostory : IRepository<ChatEntity>
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
