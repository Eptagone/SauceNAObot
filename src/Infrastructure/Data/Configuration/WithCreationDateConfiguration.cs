// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SauceNAO.Core.Entities.Abstractions;

namespace SauceNAO.Infrastructure.Data.Configuration;

sealed class TimestampableConfiguration<T> : IEntityTypeConfiguration<T>
    where T : class, ITimestampable
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
    }
}
