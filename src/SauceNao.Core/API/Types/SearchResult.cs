// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

#nullable disable

namespace SauceNAO.Core.API
{
    public sealed class SearchResult
    {
        [JsonPropertyName("header")]
        public ResultHeader Header { get; set; }
        [JsonPropertyName("data")]
        public ResultData Data { get; set; }
    }
}
