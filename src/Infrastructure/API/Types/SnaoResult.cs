// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Infrastructure.API;

internal sealed class SnaoResult
{
    public required SnaoResultHeader Header { get; set; }
    public required SnaoResultData Data { get; set; }
}
