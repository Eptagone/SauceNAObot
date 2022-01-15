// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;
using System.Runtime.InteropServices;

namespace SauceNAO.Infrastructure.Data
{
    public sealed class SauceRepository : RepositoryBase<SauceNaoContext, SuccessfulSauce>, ISauceRepository
    {
        public SauceRepository(SauceNaoContext context) : base(context)
        {
        }

        public IQueryable<SuccessfulSauce> GetAllSauces()
        {
            return Context.SuccessfulSauces.AsNoTracking().Include(s => s.UserSauces);
        }

        public void DeleteRange(IEnumerable<SuccessfulSauce> sauces)
        {
            Context.RemoveRange(sauces);
            Context.SaveChanges();
        }

        public async Task DeleteRangeAsync(IEnumerable<SuccessfulSauce> sauces, [Optional] CancellationToken cancellationToken)
        {
            Context.RemoveRange(sauces);
            await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
