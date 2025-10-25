using SauceNAO.Core.Entities.UserAggregate;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Specifications;

namespace SauceNAO.Infrastructure.Data.Repositories;

class SearchHistoryRepository(ApplicationDbContext context)
    : AsyncRepositoryBase<SearchRecord>(context),
        ISearchHistoryRepository
{
    public Task<IEnumerable<SearchRecord>> GetByUserIdAsync(
        long userId,
        string? query = null,
        int limit = 10,
        int offset = 0,
        CancellationToken cancellationToken = default
    )
    {
        var spec = new UserHistorySpecification(userId, query);
        var records = spec.Evaluate(context.SearchRecords)
            .OrderByDescending(s => s.SearchedAt)
            .Skip(offset)
            .Take(limit);
        return Task.FromResult(records.AsEnumerable());
    }
}
