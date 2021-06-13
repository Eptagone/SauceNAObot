// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNao.Data.Models;

#nullable disable

namespace SauceNao.Data
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
        public virtual DbSet<AppChat> Chats { get; set; }
        /// <summary>Users</summary>
        public virtual DbSet<AppUser> Users { get; set; }
        /// <summary>Successful Sauces</summary>
        public virtual DbSet<SuccessfulSauce> SuccessfulSauces { get; set; }
        /// <summary>Temporal Files</summary>
        public virtual DbSet<TemporalFile> TemporalFiles { get; set; }
        /// <summary>User Sauces</summary>
        public virtual DbSet<UserSauce> UserSauces { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {

        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void OnModelCreating(ModelBuilder modelBuilder)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            modelBuilder.Entity<AntiCheat>(entity =>
            {
                entity.HasOne(d => d.AddedByUser)
                    .WithMany(p => p.AntiCheats)
                    .HasForeignKey(d => d.AddedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.AntiCheats)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SuccessfulSauce>(entity =>
            {
                entity.Property(e => e.Similarity).HasDefaultValueSql("55");
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
