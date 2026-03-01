using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Entities;
using SauceNAO.Core.Entities.UserAggregate;
using SauceNAO.Infrastructure.Data.Configuration;

namespace SauceNAO.Infrastructure.Data;

partial class SnaoDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<SearchRecord> SearchRecords { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<ChatEntity> Groups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().HasIndex(u => u.UserId).IsUnique();
        modelBuilder.Entity<ChatEntity>().HasIndex(u => u.ChatId).IsUnique();
        modelBuilder.Entity<MediaFile>().HasIndex(u => u.FileUniqueId).IsUnique();
        modelBuilder.Entity<MediaFile>().HasIndex(u => u.FileId).IsUnique();
        modelBuilder.Entity<MediaFile>().ComplexCollection(e => e.Sauces, b => b.ToJson());

        modelBuilder.ApplyConfiguration(new WithCreationDateConfiguration<MediaFile>());
        modelBuilder.ApplyConfiguration(new WithCreationDateConfiguration<SearchRecord>());
    }
}
