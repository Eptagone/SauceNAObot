// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Infrastructure.API.Types;

internal class SnaoErrorHeader : SnaoHeader
{
    public required string Message { get; set; }
}
