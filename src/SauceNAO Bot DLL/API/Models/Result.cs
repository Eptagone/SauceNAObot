// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNAO.API.Models
{
    public sealed class Result
    {
        [JsonPropertyName("header")]
        public HeaderResult Header { get; set; }
        [JsonPropertyName("data")]
        public DataResult Data { get; set; }
    }
}
