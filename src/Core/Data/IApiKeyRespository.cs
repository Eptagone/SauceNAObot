// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities.UserAggregate;

namespace SauceNAO.Core.Data;

/// <summary>
/// Represents a repository for API keys used to access the SauceNAO API.
/// </summary>
public interface IApiKeyRespository : IRepository<ApiKey>
{
    /// <summary>
    /// Get the API keys for a specific user.
    /// </summary>
    /// <param name="userId">Unique identifier for the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns></returns>
    Task<IEnumerable<ApiKey>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get the public API keys that are available for use.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    Task<IEnumerable<ApiKey>> GetPublicKeysAsync(CancellationToken cancellationToken = default);
}
