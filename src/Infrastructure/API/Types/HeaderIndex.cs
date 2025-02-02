// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;

namespace SauceNAO.Infrastructure.API;

internal sealed class HeaderIndex
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
