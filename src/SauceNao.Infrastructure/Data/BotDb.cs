// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core;
using SauceNAO.Core.Data;

namespace SauceNAO.Infrastructure.Data
{
    public sealed class BotDb : IBotDb
    {
        public BotDb(SauceNaoContext context)
        {
            Users = new UserRepository(context);
            Groups = new GroupRepository(context);
            Sauces = new SauceRepository(context);
        }

        public IUserRepository Users { get; }
        public IGroupRepository Groups { get; }
        public ISauceRepository Sauces { get; }
    }
}
