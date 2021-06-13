// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the MIT License, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNao.API.Models
{
    public sealed class Result
    {
        [JsonPropertyName("header")]
        public ResultHeader Header { get; set; }
        [JsonPropertyName("data")]
        public ResultData Data { get; set; }
    }
}
