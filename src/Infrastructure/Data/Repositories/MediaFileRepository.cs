// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;

namespace SauceNAO.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository for the sauce media.
/// </summary>
/// <param name="context">The database context.</param>
sealed class MediaFileRepository(SnaoDbContext context)
    : RepositoryBase<MediaFile>(context),
        IMediaFileRepository
{
    /// <inheritdoc/>
    public Task<MediaFile?> GetByFileUniqueIdAsync(
        string fileUniqueId,
        CancellationToken cancellationToken
    )
    {
        return context
            .MediaFiles.AsNoTrackingWithIdentityResolution()
            .SingleOrDefaultAsync(s => s.FileUniqueId == fileUniqueId, cancellationToken);
    }

    public override async Task<MediaFile> UpdateAsync(
        MediaFile entity,
        [Optional] CancellationToken cancellationToken
    )
    {
        // Clear search history for the given media before updating
        await context
            .SearchRecords.Where(s => s.Media.Id == entity.Id)
            .ExecuteDeleteAsync(cancellationToken);
        // Update
        return await base.UpdateAsync(entity, cancellationToken);
    }
}
