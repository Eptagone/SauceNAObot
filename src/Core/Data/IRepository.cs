// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Data;

/// <summary>
/// Defines a generic asynchronous repository for managing entities of type TEntity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Asynchronously inserts the specified entity.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the inserted entity.</returns>
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes the specified entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously gets the number of entities in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of entities in the repository.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
