// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Linq.Expressions;

namespace SauceNAO.Domain.Specifications;

/// <summary>
/// Encapsulates query logic for <typeparamref name="T"/>.
/// </summary>
public abstract class SpecificationBase<T> : ISpecification<T>
{
    /// <inheritdoc/>
    protected abstract Expression<Func<T, bool>> Expression { get; }

    /// <inheritdoc/>
    public virtual IQueryable<T> Evaluate(IQueryable<T> entities)
    {
        return entities.Where(this.Expression);
    }

    /// <inheritdoc/>
    public bool IsSatisfiedBy(T entity)
    {
        return this.Expression.Compile().Invoke(entity);
    }
}
