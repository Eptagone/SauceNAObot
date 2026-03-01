// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities.UserAggregate;

namespace SauceNAO.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository for API keys used to access the SauceNAO API.
/// </summary>
sealed class ApiKeyRepository(SnaoDbContext context)
    : RepositoryBase<ApiKey>(context),
        IApiKeyRespository
{
    public async Task<IEnumerable<ApiKey>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default
    ) => await context.ApiKeys.Where(k => k.Owner.UserId == userId).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<ApiKey>> GetPublicKeysAsync(
        CancellationToken cancellationToken = default
    ) => await context.ApiKeys.Where(k => k.IsPublic).ToListAsync(cancellationToken);

    public async Task<ApiKey> InsertAsync(
        ApiKey entity,
        UserEntity user,
        CancellationToken cancellationToken = default
    )
    {
        context.Attach(user);
        user.ApiKeys.Add(entity);
        var value = await this.InsertAsync(entity, cancellationToken);
        context.Entry(entity).State = EntityState.Detached;
        return value;
    }

    public override Task<ApiKey> InsertAsync(
        ApiKey entity,
        [Optional] CancellationToken cancellationToken
    )
    {
        context.Attach(entity.Owner);
        var value = base.InsertAsync(entity, cancellationToken);
        context.Entry(entity).State = EntityState.Detached;
        return value;
    }
}
