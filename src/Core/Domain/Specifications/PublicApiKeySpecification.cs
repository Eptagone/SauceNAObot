// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Linq.Expressions;
using SauceNAO.Domain.Entities.SauceAggregate;

namespace SauceNAO.Domain.Specifications;

/// <summary>
/// Represents a specification to filter public API keys.
/// </summary>
public class PublicApiKeySpecification : SpecificationBase<SauceApiKey>
{
    /// <inheritdoc/>
    protected override Expression<Func<SauceApiKey, bool>> Expression => apiKey => apiKey.IsPublic;
}
