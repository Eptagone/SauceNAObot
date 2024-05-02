// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Infrastructure.API.Types;

namespace SauceNAO.Infrastructure.API;

internal interface ISnaoResponse
{
    public SnaoHeader Header { get; }
}
