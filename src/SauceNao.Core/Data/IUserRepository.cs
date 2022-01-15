// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities;
using System.Runtime.InteropServices;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Data
{
    public interface IUserRepository : IRepository<UserData>
    {
        /// <summary>Get all users.</summary>
        /// <returns>An <see cref="IQueryable"/> object of <see cref="UserData"/>.</returns>
        IQueryable<UserData> GetAllUsers();

        /// <summary>Get user data.</summary>
        /// <param name="telegramUser">Telegram user.</param>
        /// <param name="isPrivate">Is private.</param>
        UserData GetUser(ITelegramUser telegramUser);
        /// <summary>Get user data.</summary>
        /// <param name="telegramUser">Telegram user.</param>
        /// <param name="isPrivate">Is private.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<UserData> GetUserAsync(ITelegramUser telegramUser, [Optional] CancellationToken cancellationToken);
    }
}
