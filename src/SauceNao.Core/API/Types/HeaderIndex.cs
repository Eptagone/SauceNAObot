// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNAO.Core.API
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
