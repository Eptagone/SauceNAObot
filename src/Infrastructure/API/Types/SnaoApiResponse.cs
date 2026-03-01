// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Infrastructure.API.Types;

internal sealed class SnaoApiResponse(SnaoApiResponseHeader header, IEnumerable<SnaoResult> results)
{
    public SnaoApiResponseHeader Header { get; } = header;
    public IEnumerable<SnaoResult>? Results { get; } = results;
}
