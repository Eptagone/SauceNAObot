// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities;
using System.Runtime.InteropServices;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Data
{
    public interface IGroupRepository : IRepository<TelegramGroup>
    {
        /// <summary>Get all groups.</summary>
        /// <returns>An <see cref="IQueryable"/> object of <see cref="TelegramGroup"/>.</returns>
        IQueryable<TelegramGroup> GetAllGroups();

        /// <summary>Get group data.</summary>
        /// <param name="telegramChat">Telegram group.</param>
        TelegramGroup GetGroup(ITelegramChat telegramChat);
        /// <summary>Get group data.</summary>
        /// <param name="telegramChat">Telegram group.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TelegramGroup> GetGroupAsync(ITelegramChat telegramChat, [Optional] CancellationToken cancellationToken);
    }
}
