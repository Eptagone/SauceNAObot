// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain.Services;

/// <summary>
/// Define methods to interact with the SauceNAO API.
/// </summary>
public interface ISauceNaoService
{
    /// <summary>
    /// Start a new search using the url provided returns a pantry where the sauces can be prepared if there are enough ingredients.
    /// </summary>
    /// <param name="url">Image url.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>A collection of sauces found for the media.</returns>
    Task<Pantry?> SearchByUrlAsync(
        string url,
        string apiKey,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Check if the given api key belongs to a premium user.
    /// </summary>
    /// <param name="apikey">The api key to check.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>True if the api key belongs to a premium user, false otherwise. If an error occurs, null is returned.</returns>
    Task<bool?> IsPremiumUserAsync(
        string apikey,
        CancellationToken cancellationToken = default
    );
}
