// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Entities.SauceAggregate;
using SauceNAO.Domain.Repositories;
using SauceNAO.Domain.Specifications;

namespace SauceNAO.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository for API keys used to access the SauceNAO API.
/// </summary>
class ApiKeyRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, SauceApiKey>(context),
        IApiKeyRespository
{
    /// <inheritdoc/>
    public IEnumerable<SauceApiKey> GetPublicKeys()
    {
        var apiKeys = this.Context.ApiKeys;
        var spec = new PublicApiKeySpecification();
        return spec.Evaluate(apiKeys);
    }
}
