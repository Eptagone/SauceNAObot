// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Runtime.InteropServices;

namespace SauceNAO.Core.Data
{
    public interface IRepository<TEntity>
        where TEntity : class, new()
    {
        TEntity Insert(TEntity entity);
        TEntity Update(TEntity entity);
        void Delete(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity, [Optional] CancellationToken cancellationToken);
        Task<TEntity> UpdateAsync(TEntity entity, [Optional] CancellationToken cancellationToken);
        Task DeleteAsync(TEntity entity, [Optional] CancellationToken cancellationToken);
    }
}
