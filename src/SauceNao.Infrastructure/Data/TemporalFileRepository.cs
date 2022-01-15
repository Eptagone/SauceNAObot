using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;

namespace SauceNAO.Infrastructure.Data
{
    public sealed class TemporalFileRepository : RepositoryBase<CacheContext, CachedTelegramFile>, ITemporalFileRepository
    {
        public TemporalFileRepository(CacheContext context) : base(context)
        {
        }

        public CachedTelegramFile? GetFile(string fileUniqueId)
        {
            if (fileUniqueId == null)
            {
                throw new ArgumentNullException(nameof(fileUniqueId));
            }
            return Context.Files.AsNoTracking().FirstOrDefault(f => f.FileUniqueId == fileUniqueId);
        }

        public async Task<CachedTelegramFile?> GetFileAsync(string fileUniqueId, CancellationToken cancellationToken)
        {
            if (fileUniqueId == null)
            {
                throw new ArgumentNullException(nameof(fileUniqueId));
            }
            return await Context.Files.AsNoTracking().FirstOrDefaultAsync(f => f.FileUniqueId == fileUniqueId, cancellationToken).ConfigureAwait(false);
        }
    }
}
