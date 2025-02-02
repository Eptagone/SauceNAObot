// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Infrastructure.API.Types;

namespace SauceNAO.Infrastructure.API;

internal sealed class SnaoSuccessfulResponse(
    SnaoSuccessfulHeader header,
    IEnumerable<SnaoResult> results
) : SnaoResponseBase<SnaoSuccessfulHeader>(header), ISnaoResponse
{
    public IEnumerable<SnaoResult> Results { get; } = results;

    SnaoHeader ISnaoResponse.Header => this.Header;
}
