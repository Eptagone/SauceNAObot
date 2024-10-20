// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain;
using SauceNAO.Infrastructure.API.Converters;
using System.Text.Json.Serialization;

namespace SauceNAO.Infrastructure.API;

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
