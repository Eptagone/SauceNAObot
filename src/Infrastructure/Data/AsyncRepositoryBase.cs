// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Services;

namespace SauceNAO.Infrastructure.Data;

/// <summary>
/// Represents a base class for repositories.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public abstract class AsyncRepositoryBase<TEntity>(DbContext context) : IAsyncRepository<TEntity>
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
