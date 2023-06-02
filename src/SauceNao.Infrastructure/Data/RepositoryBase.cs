// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using System.Runtime.InteropServices;

namespace SauceNAO.Infrastructure.Data;

public abstract class RepositoryBase<TContext, TEntity> : IRepository<TEntity>
	where TContext : DbContext
	where TEntity : class, new()
{
	protected TContext Context { get; }
	public RepositoryBase(TContext context)
	{
		this.Context = context;
	}

	public virtual TEntity Insert(TEntity entity)
	{
		if (entity == null)
		{
			throw new ArgumentNullException(nameof(entity));
		}
		this.Context.Add(entity);
		this.Context.SaveChanges();
		return entity;
	}

	public virtual TEntity Update(TEntity entity)
	{
		if (entity == null)
		{
			throw new ArgumentNullException(nameof(entity));
		}
		this.Context.Entry(entity).State = EntityState.Modified;
		this.Context.SaveChanges();
		return entity;
	}

	public virtual void Delete(TEntity entity)
	{
		if (entity == null)
		{
			throw new ArgumentNullException(nameof(entity));
		}
		this.Context.Entry(entity).State = EntityState.Deleted;
		this.Context.SaveChanges();
	}

	public virtual async Task<TEntity> InsertAsync(TEntity entity, [Optional] CancellationToken cancellationToken)
	{
		if (entity == null)
		{
			throw new ArgumentNullException(nameof(entity));
		}
		await this.Context.AddAsync(entity, cancellationToken);
		await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return entity;
	}

	public virtual async Task<TEntity> UpdateAsync(TEntity entity, [Optional] CancellationToken cancellationToken)
	{
		if (entity == null)
		{
			throw new ArgumentNullException(nameof(entity));
		}
		this.Context.Entry(entity).State = EntityState.Modified;
		await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return entity;
	}

	public virtual async Task DeleteAsync(TEntity entity, [Optional] CancellationToken cancellationToken)
	{
		if (entity == null)
		{
			throw new ArgumentNullException(nameof(entity));
		}
		this.Context.Entry(entity).State = EntityState.Deleted;
		await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}
}
