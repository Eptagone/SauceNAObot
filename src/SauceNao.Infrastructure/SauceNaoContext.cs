// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Entities;

#nullable disable

namespace SauceNAO.Infrastructure
{
    /// <summary>SauceNao DB Context</summary>
    public partial class SauceNaoContext : DbContext
    {
        /// <summary>Initialize a new instance of SauceNaoContext.</summary>
        public SauceNaoContext()
        {
        }

        /// <summary>Initialize a new instance of SauceNaoContext.</summary>
        public SauceNaoContext(DbContextOptions<SauceNaoContext> options)
            : base(options)
        {
        }

        /// <summary>Anticheats</summary>
        public virtual DbSet<AntiCheat> AntiCheats { get; set; }
        /// <summary>Chats</summary>
        public virtual DbSet<TelegramGroup> Groups { get; set; }
        /// <summary>Users</summary>
        public virtual DbSet<UserData> Users { get; set; }
        /// <summary>Successful Sauces</summary>
        public virtual DbSet<SuccessfulSauce> SuccessfulSauces { get; set; }
        /// <summary>User Sauces</summary>
        public virtual DbSet<UserSauce> UserSauces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AntiCheat>(entity =>
            {
                entity.HasOne(d => d.AddedByUser)
                    .WithMany(p => p.AntiCheats)
                    .HasForeignKey(d => d.AddedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AntiCheats)
                    .HasForeignKey(d => d.ChatKey)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<UserSauce>(entity =>
            {
                entity.HasOne(d => d.Sauce)
                    .WithMany(p => p.UserSauces)
                    .HasForeignKey(d => d.SauceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSauces)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SuccessfulSauce>(entity =>
            {
                entity.HasIndex(s => s.FileUniqueId)
                    .HasDatabaseName("UQ_FileUniqueId")
                    .IsUnique();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
