using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SauceNAO.Core.Entities;

namespace SauceNAO.Infrastructure.Data.Configuration;

sealed class WithCreationDateConfiguration<T> : IEntityTypeConfiguration<T>
    where T : class, IWithCreationDate
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
    }
}
