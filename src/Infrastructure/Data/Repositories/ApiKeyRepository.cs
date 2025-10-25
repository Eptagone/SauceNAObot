// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities.SauceAggregate;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Specifications;

namespace SauceNAO.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository for API keys used to access the SauceNAO API.
/// </summary>
class ApiKeyRepository(ApplicationDbContext context)
    : AsyncRepositoryBase<ApplicationDbContext, SauceApiKey>(context),
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
