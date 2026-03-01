// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;

namespace SauceNAO.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a base class for repositories.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
abstract class RepositoryBase<TEntity>(DbContext context) : IRepository<TEntity>
    where TEntity : class
{
    /// <inheritdoc/>
    public virtual async Task<TEntity> InsertAsync(
        TEntity entity,
        [Optional] CancellationToken cancellationToken
    )
    {
        if (entity != null)
        {
            await context.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        throw new ArgumentNullException(nameof(entity));
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity> UpdateAsync(
        TEntity entity,
        [Optional] CancellationToken cancellationToken
    )
    {
        if (entity != null)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync(cancellationToken);
            context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        throw new ArgumentNullException(nameof(entity));
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(
        TEntity entity,
        [Optional] CancellationToken cancellationToken
    )
    {
        if (entity != null)
        {
            context.Entry(entity).State = EntityState.Deleted;
            await context.SaveChangesAsync(cancellationToken);
            context.Entry(entity).State = EntityState.Detached;
        }
        else
        {
            throw new ArgumentNullException(nameof(entity));
        }
    }

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().CountAsync(cancellationToken);
}
