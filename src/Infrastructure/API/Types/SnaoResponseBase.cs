// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Infrastructure.API.Types;

internal abstract class SnaoResponseBase<THeader>(THeader Header)
    where THeader : SnaoHeader
{
    public THeader Header { get; } = Header;
}
