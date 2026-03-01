using SauceNAO.Core.Entities.UserAggregate;

namespace SauceNAO.Core.Data;

/// <summary>
/// Defines a helper to query records from a user's search history.
/// </summary>
public interface ISearchHistoryRepository : IRepository<SearchRecord>
{
    /// <summary>
    /// Retrieves the first search record for a specific user with a specific media and similarity.
    /// </summary>
    /// <param name="userId">Unique identifier for the user.</param>
    /// <param name="mediaId">Unique identifier for the media.</param>
    /// <param name="similarity">Similarity of the search.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns></returns>
    Task<SearchRecord?> FirstWithMediaAndSimilarityAsync(
        long userId,
        int mediaId,
        float similarity,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a list of search records for a specific user.
    /// </summary>
    /// <param name="userId">Unique identifier for the user.</param>
    /// <param name="limit">Maximum number of records to retrieve.</param>
    /// <param name="offset">Number of records to skip.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A list of search records for the user.</returns>
    Task<IEnumerable<SearchRecord>> SearchAsync(
        long userId,
        string? searchTerm = null,
        int limit = 10,
        int offset = 0,
        CancellationToken cancellationToken = default
    );
}
