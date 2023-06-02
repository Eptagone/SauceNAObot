// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;

namespace SauceNAO.Infrastructure.Data;

public sealed class TemporalFileRepository : RepositoryBase<CacheDbContext, CachedTelegramFile>, ITemporalFileRepository
{
	public TemporalFileRepository(CacheDbContext context) : base(context)
	{
	}

	public CachedTelegramFile? GetFile(string fileUniqueId)
	{
		if (fileUniqueId == null)
		{
			throw new ArgumentNullException(nameof(fileUniqueId));
		}
		return this.Context.Files.AsNoTracking().FirstOrDefault(f => f.FileUniqueId == fileUniqueId);
	}

	public async Task<CachedTelegramFile?> GetFileAsync(string fileUniqueId, CancellationToken cancellationToken)
	{
		if (fileUniqueId == null)
		{
			throw new ArgumentNullException(nameof(fileUniqueId));
		}
		return await this.Context.Files.AsNoTracking().FirstOrDefaultAsync(f => f.FileUniqueId == fileUniqueId, cancellationToken).ConfigureAwait(false);
	}
}
