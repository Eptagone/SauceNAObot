// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities;

namespace SauceNAO.Core.Data;

/// <summary>
/// Defines methods for managing media targets.
/// </summary>
public interface IMediaFileRepository : IRepository<MediaFile>
{
    /// <summary>
    /// Retrieves a previously searched media by its unique identifier.
    /// </summary>
    /// <param name="fileUniqueId">Unique identifier for the file.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The media target if found, otherwise null.</returns>
    Task<MediaFile?> GetByFileUniqueIdAsync(
        string fileUniqueId,
        CancellationToken cancellationToken
    );
}
