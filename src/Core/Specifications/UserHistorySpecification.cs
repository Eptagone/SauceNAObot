using System.Linq.Expressions;
using SauceNAO.Core.Entities.UserAggregate;

namespace SauceNAO.Core.Specifications;

/// <summary>
/// Represents a specification to filter records from a user's search history.
/// </summary>
/// <param name="userId">Unique identifier of the user.</param>
/// <param name="query">Search query.</param>
public class UserHistorySpecification(long userId, string? query) : SpecificationBase<SearchRecord>
{
    private readonly string[] keywords = string.IsNullOrEmpty(query) ? [] : query.Split(' ');

    protected override Expression<Func<SearchRecord, bool>> Expression =>
        string.IsNullOrEmpty(query)
            ? record => record.User.UserId == userId
            : record =>
                record.User.UserId == userId
                && this.keywords.Any(k =>
                    record.Media.Sauces.Any(s => s.Similarity >= record.Similarity)
                    && record.Media.Sauces.Any(s =>
                        (
                            !string.IsNullOrEmpty(s.Title)
                            && s.Title.Contains(k, StringComparison.InvariantCultureIgnoreCase)
                                == true
                        )
                        || (
                            !string.IsNullOrEmpty(s.Author)
                            && s.Author.Contains(k, StringComparison.InvariantCultureIgnoreCase)
                                == true
                        )
                        || (
                            !string.IsNullOrEmpty(s.Characters)
                            && s.Characters.Contains(k, StringComparison.InvariantCultureIgnoreCase)
                                == true
                        )
                        || (
                            !string.IsNullOrEmpty(s.Material)
                            && s.Material.Contains(k, StringComparison.InvariantCultureIgnoreCase)
                                == true
                        )
                    )
                );
}
