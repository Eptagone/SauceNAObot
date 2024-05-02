// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Linq.Expressions;
using SauceNAO.Domain.Entities.SauceAggregate;

namespace SauceNAO.Domain.Specifications;

/// <summary>
/// Represents a specification to filter seached medias by their unique identifier and similarity.
/// </summary>
/// <param name="fileUniqueId">The unique identifier of the file.</param>
public class SearchedMediaSpecification(string fileUniqueId) : SpecificationBase<SauceMedia>
{
    /// <inheritdoc/>
    protected override Expression<Func<SauceMedia, bool>> Expression { get; } =
        media => media.FileUniqueId == fileUniqueId;
}
