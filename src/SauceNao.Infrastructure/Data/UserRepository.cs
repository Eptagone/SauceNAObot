// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;
using System.Runtime.InteropServices;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Infrastructure.Data
{
    public sealed class UserRepository : RepositoryBase<SauceNaoContext, UserData>, IUserRepository
    {
        public UserRepository(SauceNaoContext context) : base(context)
        {
        }

        public IQueryable<UserData> GetAllUsers()
        {
            return Context.Users.AsNoTracking().Include(u => u.UserSauces).ThenInclude(u => u.Sauce).AsQueryable();
        }

        public UserData GetUser(ITelegramUser telegramUser)
        {
            var user = Context.Users.AsNoTracking()
                .Include(u => u.UserSauces)
                .ThenInclude(u => u.Sauce)
                .FirstOrDefault(u => u.Id == telegramUser.Id);
            if (user == default)
            {
                user = new UserData(telegramUser);
                Insert(user);
            }
            else
            {
                if (user.Update(telegramUser))
                {
                    Update(user);
                }
            }
            return user;
        }
        public async Task<UserData> GetUserAsync(ITelegramUser telegramUser, [Optional] CancellationToken cancellationToken)
        {
            var user = await Context.Users.AsNoTracking()
                .Include(u => u.UserSauces)
                .ThenInclude(u => u.Sauce)
                .FirstOrDefaultAsync(u => u.Id == telegramUser.Id, cancellationToken)
                .ConfigureAwait(false);
            if (user == default)
            {
                user = new UserData(telegramUser);
                await InsertAsync(user, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (user.Update(telegramUser))
                {
                    await UpdateAsync(user, cancellationToken).ConfigureAwait(false);
                }
            }
            return user;
        }
    }
}
