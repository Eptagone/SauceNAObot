// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities.SauceAggregate;

namespace SauceNAO.Core.Repositories;

/// <summary>
/// Represents a repository for API keys used to access the SauceNAO API.
/// </summary>
public interface IApiKeyRespository : IRepository<SauceApiKey>
{
    /// <summary>
    /// Get the public API keys that are available for use.
    /// </summary>
    IEnumerable<SauceApiKey> GetPublicKeys();
}
