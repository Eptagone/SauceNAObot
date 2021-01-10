// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Models;
using System;

namespace SauceNAO.Data
{
    public class SauceNAOContext : DbContext
    {
        public DbSet<UserSauce> History { get; set; }
        public DbSet<AppChat> Chats { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Sauce> Searches { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Whitelist> Whitelists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=snaobot-F9b8yO3PWImTyF6j.db");
        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == default)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            /// <see cref="Sauce.Data"/>
            builder
                .Entity<Sauce>()
                .Property(e => e.Data)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
