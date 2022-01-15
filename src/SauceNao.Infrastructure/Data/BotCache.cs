// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core;
using SauceNAO.Core.Data;

namespace SauceNAO.Infrastructure.Data
{
    public sealed class BotCache : IBotCache
    {
        public BotCache(CacheContext context)
        {
            Files = new TemporalFileRepository(context);
        }

        public ITemporalFileRepository Files { get; }
    }
}
