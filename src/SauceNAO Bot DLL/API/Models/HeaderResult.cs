// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNAO.API.Models
{
    public sealed class HeaderResult
    {
        [JsonPropertyName("similarity")]
        public string Similarity { get; set; }
        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }
        [JsonPropertyName("index_id")]
        public int IndexId { get; set; }
        [JsonPropertyName("index_name")]
        public string IndexName { get; set; }
    }
}
