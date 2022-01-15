// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Data;

namespace SauceNAO.Core
{
    public interface IBotCache
    {
        ITemporalFileRepository Files { get; }
    }
}
