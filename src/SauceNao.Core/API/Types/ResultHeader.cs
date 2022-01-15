// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

#nullable disable

namespace SauceNAO.Core.API
{
    public sealed class ResultHeader
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
