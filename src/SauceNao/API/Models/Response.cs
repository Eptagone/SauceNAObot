// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the MIT License, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNao.API.Models
{
    public sealed class Response
    {
        [JsonPropertyName("header")]
        public Header Header { get; set; }
        [JsonPropertyName("results")]
        public Result[] Results { get; set; }
    }
}
