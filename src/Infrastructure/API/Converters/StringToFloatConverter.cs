// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SauceNAO.Infrastructure.API.Converters;

/// <summary>
/// Converts a string to a float.
/// </summary>
internal class StringToFloatConverter : JsonConverter<float>
{
    /// <inheritdoc/>
    public override float Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return 0;
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetSingle();
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (float.TryParse(value, out var result))
            {
                return result;
            }
            return 0;
        }

        throw new JsonException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
