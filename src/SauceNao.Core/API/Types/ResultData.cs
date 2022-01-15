// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

#nullable disable

namespace SauceNAO.Core.API
{
    public sealed class ResultData
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }
        [JsonPropertyName("anidb_aid")]
        public uint AnidbAid { get; set; }
        [JsonPropertyName("ext_urls")]
        public string[] ExtUrls { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("part")]
        public string Part { get; set; }
        [JsonPropertyName("year")]
        public string Year { get; set; }
        [JsonPropertyName("est_time")]
        public string EstTime { get; set; }
        [JsonPropertyName("eng_name")]
        public string EngName { get; set; }
        [JsonPropertyName("jp_name")]
        public string JpName { get; set; }
        [JsonPropertyName("creator")]
        public object Creator { get; set; }
        [JsonPropertyName("artist")]
        public string Artist { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonPropertyName("material")]
        public string Material { get; set; }
        [JsonPropertyName("characters")]
        public string Characters { get; set; }
        // Pixiv
        [JsonPropertyName("pixiv_id")]
        public uint PixivId { get; set; }
        [JsonPropertyName("member_name")]
        public string MemberName { get; set; }
        [JsonPropertyName("member_id")]
        public uint MemberId { get; set; }
        // DA
        //[JsonPropertyName("da_id")]
        //public uint DaId { get; set; }
        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; }
        [JsonPropertyName("author_url")]
        public string AuthorUrl { get; set; }
        // Others
        [JsonPropertyName("danbooru_id")]
        public uint DanbooruId { get; set; }
        [JsonPropertyName("yandere_id")]
        public uint YandereId { get; set; }
        [JsonPropertyName("gelbooru_id")]
        public uint GelbooruId { get; set; }
        [JsonPropertyName("konachan_id")]
        public uint KonachanId { get; set; }
        [JsonPropertyName("md_id")]
        public uint MdId { get; set; }
        [JsonPropertyName("mu_id")]
        public uint MuId { get; set; }
        [JsonPropertyName("mal_id")]
        public uint MalId { get; set; }
    }
}
