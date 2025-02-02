// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SauceNAO.Infrastructure.API.Converters;

/// <summary>
/// Converts a single value or an array of values to a single value or an array of values.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
internal class SingleOrArrayConverter<T> : JsonConverter<IEnumerable<T>>
{
    public override IEnumerable<T>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            return JsonSerializer.Deserialize<IEnumerable<T>>(ref reader, options);
        }
        else
        {
            var t = JsonSerializer.Deserialize<T>(ref reader, options) ?? throw new JsonException();
            return [t];
        }
    }

    public override void Write(
        Utf8JsonWriter writer,
        IEnumerable<T>? value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}
