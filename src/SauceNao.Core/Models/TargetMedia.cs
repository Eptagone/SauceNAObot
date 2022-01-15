// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Enums;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Models
{
    internal class TargetMedia
    {
        private readonly string targetFileId;
        internal TargetMedia(Message message)
        {
            if (message.Photo != null) // If media is a Photo
            {
                IsValid = true;
                FileId = message.Photo[0].FileId;
                FileUniqueId = message.Photo[0].FileUniqueId;
                Type = MediaType.Photo;
            }
            else if (message.Sticker != null) // If media is a Sticker
            {
                Type = MediaType.Sticker;
                if (!message.Sticker.IsAnimated)
                {
                    IsValid = true;
                    FileUniqueId = message.Sticker.FileUniqueId;
                    FileId = message.Sticker.FileId;
                }
            }
            else if (message.Animation != null) // If media is an Animation
            {
                IsValid = true;
                Type = MediaType.Animation;
                FileId = message.Animation.FileId;
                FileUniqueId = message.Animation.FileUniqueId;
                if (message.Animation.Thumb != null)
                {
                    targetFileId = message.Animation.Thumb.FileId;
                }
                else if (message.Animation.MimeType.Contains("video"))
                {
                    NeedConversion = true;
                    ContentType = message.Animation.MimeType;
                }
            }
            else if (message.Video != null) // If media is a Video
            {
                IsValid = true;
                Type = MediaType.Video;
                FileId = message.Video.FileId;
                FileUniqueId = message.Video.FileUniqueId;
                if (message.Video.Thumb != null)
                {
                    targetFileId = message.Video.Thumb.FileId;
                }
                else
                {
                    NeedConversion = true;
                    ContentType = message.Video.MimeType;
                }
            }
            else if (message.Document != null) // If media is a Document
            {
                Type = MediaType.Document;
                FileId = message.Document.FileId;
                FileUniqueId = message.Document.FileUniqueId;
                if (message.Document.MimeType.Contains("image"))
                {
                    IsValid = true;
                }
                else if (message.Document.Thumb != null)
                {
                    IsValid = true;
                    targetFileId = message.Document.Thumb.FileId;
                }
                else if (message.Document.MimeType.Contains("video"))
                {
                    IsValid = true;
                    NeedConversion = true;
                    ContentType = message.Document.MimeType;
                }
            }
        }
        internal string FileId { get; }
        internal string FilePath { get; set; }
        internal string FileUniqueId { get; }
        internal bool IsValid { get; }
        internal bool NeedConversion { get; }
        internal string TargetFileId => targetFileId ?? FileId;
        internal string TargetSearchPath => TemporalFilePath ?? FilePath;
        internal string TemporalFilePath { get; set; }
        internal MediaType Type { get; }
        internal string? ContentType { get; }
    }
}
