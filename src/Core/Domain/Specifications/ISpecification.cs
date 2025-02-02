// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain.Specifications;

/// <summary>
/// Encapsulates query logic for <typeparamref name="T"/>.
/// </summary>
public interface ISpecification<T>
{
    /// <summary>
    /// Applies the specification to the given <paramref name="entities"/>.
    /// </summary>
    /// <param name="entities">A queryable collection of <typeparamref name="T"/> to be filtered.</param>
    /// <returns>A queryable collection of <typeparamref name="T"/> with the specification applied.</returns>
    IQueryable<T> Evaluate(IQueryable<T> entities);

    /// <summary>
    /// Checks if <paramref name="entity"/> satisfies the specification.
    /// </summary>
    /// <param name="entity">The entity to be validated.</param>
    /// <returns>True if <paramref name="entity"/> satisfies the specification, otherwise false.</returns>
    bool IsSatisfiedBy(T entity);
}
