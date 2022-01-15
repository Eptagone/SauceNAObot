// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;
using System.Runtime.InteropServices;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Infrastructure.Data
{
    public sealed class GroupRepository : RepositoryBase<SauceNaoContext, TelegramGroup>, IGroupRepository
    {
        public GroupRepository(SauceNaoContext context) : base(context)
        {
        }

        public IQueryable<TelegramGroup> GetAllGroups()
        {
            return Context.Groups.AsNoTracking().Include(c => c.AntiCheats).AsQueryable();
        }

        public TelegramGroup GetGroup(ITelegramChat telegramChat)
        {
            if (telegramChat.Type is ChatType.Private or ChatType.Channel)
            {
                throw new ArgumentException("Type of chat mush be group");
            }
            var group = Context.Groups.AsNoTracking()
                .Where(c => c.Id == telegramChat.Id)
                .Include(c => c.AntiCheats)
                .SingleOrDefault();
            if (group == default)
            {
                group = new TelegramGroup(telegramChat);
                Insert(group);
            }
            else
            {
                if (group.UpdateInfo(telegramChat))
                {
                    Update(group);
                }
            }
            return group;
        }
        public async Task<TelegramGroup> GetGroupAsync(ITelegramChat telegramChat, [Optional] CancellationToken cancellationToken)
        {
            if (telegramChat.Type is ChatType.Private or ChatType.Channel)
            {
                throw new ArgumentException("Type of chat mush be group");
            }
            var group = await Context.Groups.AsNoTracking()
                .Where(c => c.Id == telegramChat.Id)
                .Include(c => c.AntiCheats)
                .SingleOrDefaultAsync(cancellationToken);
            if (group == default)
            {
                group = new TelegramGroup(telegramChat);
                await InsertAsync(group, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (group.UpdateInfo(telegramChat))
                {
                    await UpdateAsync(group, cancellationToken).ConfigureAwait(false);
                }
            }
            return group;
        }
    }
}
