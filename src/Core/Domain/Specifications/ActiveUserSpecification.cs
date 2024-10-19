// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Linq.Expressions;
using SauceNAO.Domain.Entities.UserAggregate;

namespace SauceNAO.Domain.Specifications;

/// <summary>
/// Represents a specification to filter active users.
/// </summary>
public class ActiveUserSpecification : SpecificationBase<TelegramUser>
{
    /// <inheritdoc/>
    protected override Expression<Func<TelegramUser, bool>> Expression =>
        user => user.SearchHistory.Where(s => s.SearchedAt > DateTimeOffset.Now.AddDays(-7)).Any();
}
