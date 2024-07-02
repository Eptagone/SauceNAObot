// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Domain.Entities.ChatAggregate;
using SauceNAO.Domain.Entities.SauceAggregate;
using SauceNAO.Domain.Entities.UserAggregate;

#nullable disable

namespace SauceNAO.Infrastructure.Data;

/// <summary>
/// Represents the database context for SauceNAO.
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initialize a new instance of SauceNaoContext.
    /// </summary>
    public ApplicationDbContext() { }

    /// <summary>
    /// Initialize a new instance of SauceNaoContext.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    /// <summary>
    /// Groups/Channels where the bot has been added.
    /// </summary>
    public DbSet<TelegramChat> Chats { get; set; }

    /// <summary>
    /// Anti-Cheat restrictions for bots.
    /// </summary>
    public DbSet<AntiCheatRestriction> AntiCheatRestrictions { get; set; }

    /// <summary>
    /// Users who have interacted with the bot.
    /// </summary>
    public DbSet<TelegramUser> Users { get; set; }

    /// <summary>
    /// Search history of users.
    /// </summary>
    public DbSet<SearchRecord> SearchRecords { get; set; }

    /// <summary>
    /// Represents a search result from SauceNAO, including image information and similarity score.
    /// </summary>
    public DbSet<SauceMedia> SearchedMedias { get; set; }

    /// <summary>
    /// Represents a single posible source of a search result.
    /// </summary>
    public DbSet<Sauce> Sauces { get; set; }

    /// <summary>
    /// Represents a search result from SauceNAO, including image information and similarity score.
    /// </summary>
    public DbSet<SauceApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelegramUser>().HasIndex(u => u.UserId).IsUnique();
        modelBuilder.Entity<TelegramChat>().HasIndex(c => c.ChatId).IsUnique();
    }
}
