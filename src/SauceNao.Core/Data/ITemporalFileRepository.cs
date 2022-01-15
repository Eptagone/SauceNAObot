// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities;
using System.Runtime.InteropServices;

namespace SauceNAO.Core.Data
{
    public interface ITemporalFileRepository : IRepository<CachedTelegramFile>
    {
        /// <summary>Get cached Telegram file.</summary>
        /// <param name="fileUniqueId">File unique Id.</param>
        /// <returns>The file.</returns>
        CachedTelegramFile? GetFile(string fileUniqueId);
        /// <summary>Get cached Telegram file.</summary>
        /// <param name="fileUniqueId">File unique Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The file.</returns>
        Task<CachedTelegramFile?> GetFileAsync(string fileUniqueId, [Optional] CancellationToken cancellationToken);
    }
}
