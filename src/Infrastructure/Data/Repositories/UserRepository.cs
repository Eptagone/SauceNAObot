// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Domain.Entities.UserAggregate;
using SauceNAO.Domain.Repositories;
using SauceNAO.Domain.Specifications;

namespace SauceNAO.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository for the user entity.
/// </summary>
/// <param name="context">The database context.</param>
class UserRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, TelegramUser>(context),
        IUserRepository
{
    /// <inheritdoc/>
    public TelegramUser? GetByUserId(long userId)
    {
        var users = this.Context.Users.Include(user => user.ApiKeys);
        var spec = new UserSpecification(userId);
        return spec.Evaluate(users).SingleOrDefault();
    }
}
