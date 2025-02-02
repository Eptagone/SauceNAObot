// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using SauceNAO.Domain.Repositories;

namespace SauceNAO.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a base class for repositories.
/// </summary>
/// <typeparam name="TContext">The type of the context.</typeparam>
public abstract class RepositoryBase<TContext, TEntity>(TContext context) : IRepository<TEntity>
    where TContext : DbContext
    where TEntity : class
{
    /// <summary>
    /// The context instance.
    /// </summary>
    protected TContext Context { get; } = context;

    /// <inheritdoc/>
    public virtual TEntity Insert(TEntity entity)
    {
        if (entity != null)
        {
            this.Context.Add(entity);
            this.Context.SaveChanges();
            return entity;
        }
        throw new ArgumentNullException(nameof(entity));
    }

    /// <inheritdoc/>
    public virtual TEntity Update(TEntity entity)
    {
        if (entity != null)
        {
            this.Context.Entry(entity).State = EntityState.Modified;
            this.Context.SaveChanges();
            return entity;
        }
        throw new ArgumentNullException(nameof(entity));
    }

    /// <inheritdoc/>
    public virtual void Delete(TEntity entity)
    {
        if (entity != null)
        {
            this.Context.Entry(entity).State = EntityState.Deleted;
            this.Context.SaveChanges();
        }
        throw new ArgumentNullException(nameof(entity));
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity> InsertAsync(
        TEntity entity,
        [Optional] CancellationToken cancellationToken
    )
    {
        if (entity != null)
        {
            await this.Context.AddAsync(entity, cancellationToken);
            await this.Context.SaveChangesAsync(cancellationToken);
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
            this.Context.Entry(entity).State = EntityState.Modified;
            await this.Context.SaveChangesAsync(cancellationToken);
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
            this.Context.Entry(entity).State = EntityState.Deleted;
            await this.Context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new ArgumentNullException(nameof(entity));
        }
    }

    /// <inheritdoc/>
    public int Count() => this.Context.Set<TEntity>().Count();

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        this.Context.Set<TEntity>().CountAsync(cancellationToken);
}
