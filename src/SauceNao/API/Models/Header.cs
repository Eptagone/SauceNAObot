// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the MIT License, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNao.API.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class Header
    {
        [JsonPropertyName("user_id")]
        public object UserId { get; set; }
        [JsonPropertyName("account_type")]
        public object AccountType { get; set; }
        [JsonPropertyName("short_limit")]
        public string ShortLimit { get; set; }
        [JsonPropertyName("long_limit")]
        public string LongLimit { get; set; }
        [JsonPropertyName("long_remaining")]
        public int LongRemaining { get; set; }
        [JsonPropertyName("short_remaining")]
        public int ShortRemaining { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("results_requested")]
        public int ResultsRequested { get; set; }
        [JsonPropertyName("index")]
        public object Index { get; set; }
        [JsonPropertyName("search_depth")]
        public string SearchDepth { get; set; }
        [JsonPropertyName("minimum_similarity")]
        public double MinimumSimilarity { get; set; }
        [JsonPropertyName("query_image_display")]
        public string QueryImageDisplay { get; set; }
        [JsonPropertyName("query_image")]
        public string QueryImage { get; set; }
        [JsonPropertyName("results_returned")]
        public int ResultsReturned { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
