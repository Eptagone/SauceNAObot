// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Domain.Entities.SauceAggregate;
using SauceNAO.Domain.Repositories;
using SauceNAO.Domain.Specifications;

namespace SauceNAO.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository for the sauce media.
/// </summary>
/// <param name="context">The database context.</param>
class SauceMediaRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, SauceMedia>(context),
        ISauceMediaRepository
{
    /// <inheritdoc/>
    public SauceMedia? GetByFileUniqueId(string fileUniqueId)
    {
        var medias = this.Context.SearchedMedias.Include(s => s.Sauces);
        var spec = new SearchedMediaSpecification(fileUniqueId);
        return spec.Evaluate(medias).FirstOrDefault();
    }
}
