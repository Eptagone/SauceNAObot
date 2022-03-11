// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core;
using SauceNAO.Core.Data;

namespace SauceNAO.Infrastructure.Data
{
    public sealed class BotDb : ISauceDatabase
    {
        public BotDb(SauceNaoContext context, CacheDbContext cacheContext)
        {
            Users = new UserRepository(context);
            Groups = new GroupRepository(context);
            Sauces = new SauceRepository(context);

            Files = new TemporalFileRepository(cacheContext);

        }

        public IUserRepository Users { get; }
        public IGroupRepository Groups { get; }
        public ISauceRepository Sauces { get; }

        public ITemporalFileRepository Files { get; }
    }
}
