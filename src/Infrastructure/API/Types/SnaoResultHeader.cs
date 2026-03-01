// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;
using SauceNAO.Infrastructure.API.Converters;

namespace SauceNAO.Infrastructure.API.Types;

internal sealed class SnaoResultHeader
{
    [JsonConverter(typeof(StringToFloatConverter))]
    public float Similarity { get; set; }
    public required string Thumbnail { get; set; }
    public SauceIndex IndexId { get; set; }
    public required string IndexName { get; set; }
    // public int Dupes { get; set; }
    // public bool Hidden { get; set; }
}
