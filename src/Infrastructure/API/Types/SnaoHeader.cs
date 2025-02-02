// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;
using SauceNAO.Infrastructure.API.Converters;
using SauceNAO.Infrastructure.API.Types;

namespace SauceNAO.Infrastructure.API;

internal sealed class SnaoSuccessfulHeader : SnaoHeader
{
    [JsonConverter(typeof(StringToIntConverter))]
    public int UserId { get; set; }

    [JsonConverter(typeof(StringToIntConverter))]
    public int AccountType { get; set; }

    [JsonConverter(typeof(StringToIntConverter))]
    public int ShortLimit { get; set; }
    public int ShortRemaining { get; set; }

    [JsonConverter(typeof(StringToIntConverter))]
    public int LongLimit { get; set; }
    public int LongRemaining { get; set; }

    /**
        Other known properties:
        - results_requested (int)
        - index (dictionary<int, string>)
            - id (int)
            - status (int)
            - parent_id (int)
            - results (int)
        - search_depth (string)
        - minimum_similarity (double)
        - query_image_display (string)
        - query_image (string)
        - results_returned (int)
    */
}
