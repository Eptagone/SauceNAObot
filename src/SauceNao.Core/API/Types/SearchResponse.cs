// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNAO.Core.API
{
    /// <summary>
    /// Represents a search response.
    /// </summary>
    public sealed class SearchResponse
    {
        /// <summary>
        /// The search header.
        /// </summary>
        [JsonPropertyName("header")]
        public SearchHeader Header { get; set; } = null!;
        /// <summary>
        /// The search results.
        /// </summary>
        [JsonPropertyName("results")]
        public IEnumerable<SearchResult>? Results { get; set; }
    }
}
