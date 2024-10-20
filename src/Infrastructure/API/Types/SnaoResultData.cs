// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Infrastructure.API.Converters;
using System.Text.Json.Serialization;

namespace SauceNAO.Infrastructure.API;

internal sealed class SnaoResultData
{
    public IEnumerable<string>? ExtUrls { get; set; }

    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    [JsonPropertyName("creator")]
    public IEnumerable<string>? Creators { get; set; }
    public string? Source { get; set; }
    public string? Artist { get; set; }
    public string? Author { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorUrl { get; set; }
    public int AnidbAid { get; set; }
    public int? AnilistId { get; set; }
    public string? Title { get; set; }
    public string? Part { get; set; }
    public string? Year { get; set; }
    public string? EstTime { get; set; }
    public string? EngName { get; set; }
    public string? JpName { get; set; }
    public string? Material { get; set; }
    public string? Characters { get; set; }
    public int? PixivId { get; set; }
    public string? MemberName { get; set; }
    public uint? MemberId { get; set; }
    public uint? DanbooruId { get; set; }
    public uint? YandereId { get; set; }
    public int? GelbooruId { get; set; }
    public int? KonachanId { get; set; }
    public string? MdId { get; set; }
    public int? MuId { get; set; }
    public uint? MalId { get; set; }

    [JsonConverter(typeof(StringToIntConverter))]
    public int? DaId { get; set; }
    public string? AsProject { get; set; }
}
