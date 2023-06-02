﻿// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SauceNAO.Core.Entities;

/// <summary>
/// Represents a cached telegram file.
/// </summary>
[Table("CachedFile", Schema = "tg")]
public partial class CachedTelegramFile
{
	/// <summary>
	/// Initialize a new instance of <see cref="CachedTelegramFile"/>.
	/// </summary>
	public CachedTelegramFile() { }

	/// <summary>
	/// Initialize a new instance of <see cref="CachedTelegramFile"/>.
	/// </summary>
	/// <param name="fileUniqueId">File unique id.</param>
	/// <param name="filename">Filename.</param>
	/// <param name="contentType">MIME type.</param>
	/// <param name="data">File Data.</param>
	public CachedTelegramFile(string fileUniqueId, string filename, string? contentType, byte[] data)
	{
		if (string.IsNullOrEmpty(fileUniqueId))
		{
			throw new ArgumentException("The fileUniqueId must not be null or empty.");
		}
		if (string.IsNullOrEmpty(filename))
		{
			throw new ArgumentException("The filename must not be null or empty.");
		}
		if (data == null || data.Length == 0)
		{
			throw new ArgumentException("The data must not be null or empty.");
		}

		this.FileUniqueId = fileUniqueId;
		this.Filename = filename;
		this.ContentType = contentType;
		this.RawData = data;
		this.Date = DateTime.UtcNow;
	}

	/// <summary>
	/// Temporal File Id.
	/// </summary>
	[Key]
	public int Key { get; set; }

	/// <summary>
	/// File Unique Id.
	/// </summary>
	[Required]
	[StringLength(64)]
	public string FileUniqueId { get; set; } = null!;

	/// <summary>
	/// Filename.
	/// </summary>
	[Required]
	[StringLength(256)]
	public string Filename { get; set; } = null!;

	/// <summary>
	/// MIME type.
	/// </summary>
	[StringLength(128)]
	public string? ContentType { get; set; }

	/// <summary>
	/// File Data.
	/// </summary>
	[Required]
	public byte[] RawData { get; set; } = null!;

	/// <summary>
	/// File date.
	/// </summary>
	public DateTime Date { get; set; }
}
