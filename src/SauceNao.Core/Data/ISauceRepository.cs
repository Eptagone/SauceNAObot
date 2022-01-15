// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities;
using System.Runtime.InteropServices;

namespace SauceNAO.Core.Data
{
    public interface ISauceRepository : IRepository<SuccessfulSauce>
    {
        /// <summary>Get all sauces.</summary>
        /// <returns>An <see cref="IQueryable"/> object of <see cref="SuccessfulSauce"/>.</returns>
        IQueryable<SuccessfulSauce> GetAllSauces();

        /// <summary>Delete all sauces from list.</summary>
        /// <param name="sauces">Sauce to remove.</param>
        void DeleteRange(IEnumerable<SuccessfulSauce> sauces);
        /// <summary>Delete all sauces from list.</summary>
        /// <param name="sauces">Sauce to remove.</param>
        /// <param name="cancellationToken">The cancellattion token.</param>
        Task DeleteRangeAsync(IEnumerable<SuccessfulSauce> sauces, [Optional] CancellationToken cancellationToken);
    }
}
