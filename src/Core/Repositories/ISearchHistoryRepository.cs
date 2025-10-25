using SauceNAO.Core.Entities.UserAggregate;

namespace SauceNAO.Core.Repositories;

/// <summary>
/// Defines a helper to query records from a user's search history.
/// </summary>
public interface ISearchHistoryRepository : IAsyncRepository<SearchRecord>
{
    /// <summary>
    /// Retrieves a list of search records for a specific user.
    /// </summary>
    /// <param name="userId">Unique identifier for the user.</param>
    /// <param name="limit">Maximum number of records to retrieve.</param>
    /// <param name="offset">Number of records to skip.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A list of search records for the user.</returns>
    Task<IEnumerable<SearchRecord>> GetByUserIdAsync(
        long userId,
        string? query = null,
        int limit = 10,
        int offset = 0,
        CancellationToken cancellationToken = default
    );
}
