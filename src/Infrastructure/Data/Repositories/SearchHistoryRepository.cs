using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities.UserAggregate;

namespace SauceNAO.Infrastructure.Data.Repositories;

sealed class SearchHistoryRepository(SnaoDbContext context)
    : RepositoryBase<SearchRecord>(context),
        ISearchHistoryRepository
{
    private const string LIKE_WILDCARD_CHARS = "_%|*+?[](){}.";

    public Task<SearchRecord?> FirstWithMediaAndSimilarityAsync(
        long userId,
        int mediaId,
        float similarity,
        CancellationToken cancellationToken = default
    )
    {
        return context
            .SearchRecords.AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.User.Id == userId && s.Media.Id == mediaId && s.Similarity == similarity,
                cancellationToken
            );
    }

    public async Task<IEnumerable<SearchRecord>> SearchAsync(
        long userId,
        string? searchTerm = null,
        int limit = 10,
        int offset = 0,
        CancellationToken cancellationToken = default
    )
    {
        var query = context
            .SearchRecords.AsNoTracking()
            .Include(s => s.Media)
            .Where(s => s.User.UserId == userId);
        if (!string.IsNullOrEmpty(searchTerm))
        {
            string cleanSearchTerm = searchTerm;
            foreach (var c in LIKE_WILDCARD_CHARS)
            {
                cleanSearchTerm = cleanSearchTerm.Replace(c.ToString(), $"\\{c}");
            }
            cleanSearchTerm = string.Join(
                ' ',
                cleanSearchTerm.Split(' ').Where(s => !string.IsNullOrEmpty(s))
            );
            cleanSearchTerm = "%" + cleanSearchTerm.Replace(" ", "% %") + "%";

            query = query.Where(record =>
                record.Media.Sauces.Any(s => s.Similarity >= record.Similarity)
                && record.Media.Sauces.Any(s =>
                    EF.Functions.ILike(
                        s.Title + " " + s.Author + " " + s.Characters + " " + s.Material,
                        cleanSearchTerm
                    )
                )
            );
        }

        return await query
            .OrderByDescending(s => s.UpdatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public override async Task<SearchRecord> InsertAsync(
        SearchRecord entity,
        [Optional] CancellationToken cancellationToken
    )
    {
        context.Attach(entity.User);
        context.Attach(entity.Media);
        var value = await base.InsertAsync(entity, cancellationToken);
        context.Entry(entity).State = EntityState.Detached;
        context.Entry(entity.Media).State = EntityState.Detached;
        return value;
    }
}
