// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

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
