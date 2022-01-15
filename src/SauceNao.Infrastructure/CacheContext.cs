// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Entities;

#nullable disable

namespace SauceNAO.Infrastructure
{
    /// <summary>Cache DB Context</summary>
    public partial class CacheContext : DbContext
    {
        /// <summary>Initialize a new instance of SauceNaoContext.</summary>
        public CacheContext()
        {
        }

        /// <summary>Initialize a new instance of SauceNaoContext.</summary>
        public CacheContext(DbContextOptions<CacheContext> options)
            : base(options)
        {
        }

        /// <summary>Temporal Files</summary>
        public virtual DbSet<CachedTelegramFile> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}