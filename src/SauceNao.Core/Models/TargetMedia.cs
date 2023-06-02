// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Enums;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Models;

internal class TargetMedia
{
	private readonly string targetFileId = null!;

	internal TargetMedia(Message message)
	{
		if (message.Photo != null) // If media is a Photo
		{
			this.IsValid = true;
			this.FileId = message.Photo[0].FileId;
			this.FileUniqueId = message.Photo[0].FileUniqueId;
			this.Type = MediaType.Photo;
		}
		else if (message.Sticker != null) // If media is a Sticker
		{
			this.Type = MediaType.Sticker;
			if (!message.Sticker.IsAnimated)
			{
				this.IsValid = true;
				this.FileUniqueId = message.Sticker.FileUniqueId;
				this.FileId = message.Sticker.FileId;
			}
		}
		else if (message.Animation != null) // If media is an Animation
		{
			this.IsValid = true;
			this.Type = MediaType.Animation;
			this.FileId = message.Animation.FileId;
			this.FileUniqueId = message.Animation.FileUniqueId;
			if (message.Animation.Thumbnail != null)
			{
				this.targetFileId = message.Animation.Thumbnail.FileId;
			}
			else if (message.Animation.MimeType != null && message.Animation.MimeType.Contains("video"))
			{
				this.NeedConversion = true;
				this.ContentType = message.Animation.MimeType;
			}
		}
		else if (message.Video != null) // If media is a Video
		{
			this.IsValid = true;
			this.Type = MediaType.Video;
			this.FileId = message.Video.FileId;
			this.FileUniqueId = message.Video.FileUniqueId;
			if (message.Video.Thumbnail != null)
			{
				this.targetFileId = message.Video.Thumbnail.FileId;
			}
			else
			{
				this.NeedConversion = true;
				this.ContentType = message.Video.MimeType;
			}
		}
		else if (message.Document != null) // If media is a Document
		{
			this.Type = MediaType.Document;
			this.FileId = message.Document.FileId;
			this.FileUniqueId = message.Document.FileUniqueId;
			if (message.Document.MimeType != null && message.Document.MimeType.Contains("image"))
			{
				this.IsValid = true;
			}
			else if (message.Document.Thumbnail != null)
			{
				this.IsValid = true;
				this.targetFileId = message.Document.Thumbnail.FileId;
			}
			else if (message.Document.MimeType != null && message.Document.MimeType.Contains("video"))
			{
				this.IsValid = true;
				this.NeedConversion = true;
				this.ContentType = message.Document.MimeType;
			}
		}
	}
	internal string FileId { get; } = null!;
	internal string FilePath { get; set; } = null!;
	internal string FileUniqueId { get; } = null!;
	internal bool IsValid { get; }
	internal bool NeedConversion { get; }
	internal string TargetFileId => this.targetFileId ?? this.FileId;
	internal string TargetSearchPath => this.TemporalFilePath ?? this.FilePath;
	internal string TemporalFilePath { get; set; } = null!;
	internal MediaType Type { get; }
	internal string? ContentType { get; }
}
