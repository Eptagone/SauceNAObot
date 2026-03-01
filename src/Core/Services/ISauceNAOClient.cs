// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Exceptions.Sauce;
using SauceNAO.Core.Models;

namespace SauceNAO.Core.Services;

/// <summary>
/// Define methods to interact with the SauceNAO API.
/// </summary>
public interface ISauceNAOClient
{
    /// <summary>
    /// Start a new search using the provided url.
    /// </summary>
    /// <param name="url">Image url.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="SauceNAOException">Thrown when an error occurs while processing the request.</exception>
    /// <returns>A collection of <see cref="Sauce"/> objects.</returns>
    Task<IEnumerable<Sauce>> SearchByUrlAsync(
        string url,
        string apiKey,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Check if the given api key belongs to a premium user.
    /// </summary>
    /// <param name="apikey">The api key to check.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="SauceNAOException">Thrown when an error occurs while processing the request.</exception>
    /// <returns>True if the api key belongs to a premium user, otherwise false.</returns>
    Task<bool> IsPremiumAsync(string apikey, CancellationToken cancellationToken = default);
}
