// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the MIT License, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNao.API.Models
{
    public sealed class HeaderIndex
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("parent_id")]
        public int ParentId { get; set; }
        [JsonPropertyName("results")]
        public int Results { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
