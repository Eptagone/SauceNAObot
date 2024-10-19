using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SauceNAO.Infrastructure.Data;

/// <summary>
/// Represents a converter for <see cref="DateTimeOffset"/>.
/// </summary>
public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetConverter()
        : base(d => d.ToUniversalTime(), d => d.ToUniversalTime()) { }
}
