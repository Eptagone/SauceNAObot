using System.Collections.ObjectModel;
using SauceNAO.Core.Entities.UserAggregate;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Repositories;

/// <summary>
/// Provides a helper to manage user data.
/// </summary>
public interface IUserRepository : IAsyncRepository<TelegramUser>
{
    /// <summary>
    /// Retrieves the updated user preferences from the database or creates a profile for the user if it doesn't exist.
    /// If the user already exists, the existing data is updated with the provided information.
    /// </summary>
    /// <param name="user">The Telegram user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated Telegram user data.</returns>
    Task<TelegramUser> UpsertAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the list of language codes used by users.
    /// </summary>
    /// <returns>A dictionary of language codes and their number of occurrences.</returns>
    Task<IReadOnlyDictionary<string, int>> GetLanguageCodesAsync(
        CancellationToken cancellationToken
    );
}
