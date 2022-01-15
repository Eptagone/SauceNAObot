// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using System.Runtime.InteropServices;

namespace SauceNAO.Infrastructure.Data
{
    public abstract class RepositoryBase<TContext, TEntity> : IRepository<TEntity>
        where TContext : DbContext
        where TEntity : class, new()
    {
        protected TContext Context { get; }
        public RepositoryBase(TContext context)
        {
            Context = context;
        }

        public TEntity Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            Context.Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return entity;
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            Context.Entry(entity).State = EntityState.Deleted;
            Context.SaveChanges();
        }

        public async Task<TEntity> InsertAsync(TEntity entity, [Optional] CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await Context.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, [Optional] CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task DeleteAsync(TEntity entity, [Optional] CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            Context.Entry(entity).State = EntityState.Deleted;
            await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
