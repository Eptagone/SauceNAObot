// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Entities;

#nullable disable

namespace SauceNAO.Infrastructure;

/// <summary>Cache DB Context</summary>
public partial class CacheDbContext : DbContext
{
	/// <summary>Initialize a new instance of SauceNaoContext.</summary>
	public CacheDbContext()
	{
	}

	/// <summary>Initialize a new instance of SauceNaoContext.</summary>
	public CacheDbContext(DbContextOptions<CacheDbContext> options)
		: base(options)
	{
	}

	/// <summary>Temporal Files</summary>
	public virtual DbSet<CachedTelegramFile> Files { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		this.OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}