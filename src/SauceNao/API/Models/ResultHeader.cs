// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the MIT License, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNao.API.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
