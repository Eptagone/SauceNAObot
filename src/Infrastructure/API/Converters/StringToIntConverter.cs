// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SauceNAO.Infrastructure.API.Converters;

/// <summary>
/// Converts a string to an integer.
/// </summary>
internal class StringToIntConverter : JsonConverter<int>
{
    /// <inheritdoc/>
    public override int Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (int.TryParse(value, out var result))
            {
                return result;
            }
            return 0;
        }

        throw new JsonException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
