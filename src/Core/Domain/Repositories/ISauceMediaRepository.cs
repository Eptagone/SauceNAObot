// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Entities.SauceAggregate;

namespace SauceNAO.Domain.Repositories;

/// <summary>
/// Represents the sauce media repository.
/// </summary>
public interface ISauceMediaRepository : IRepository<SauceMedia>
{
    /// <summary>
    /// Find all sauces by their unique identifier for the file.
    /// </summary>
    /// <param name="fileUniqueId">Unique identifier for the file.</param>
    /// <returns>A <see cref="SauceMedia"/> object or null if not found.</returns>
    SauceMedia? GetByFileUniqueId(string fileUniqueId);
}
