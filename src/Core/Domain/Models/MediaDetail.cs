// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Domain;

/// <summary>
/// Represents the details of a media extracted from a Telegram message.
/// </summary>
/// <param name="FileUniqueId">Unique identifier for the media file.</param>
/// <param name="FileId">Identifier for the media file.</param>
/// <param name="MimeType">MIME type of the media file.</param>
/// <param name="Size">File size of the media file in bytes.</param>
/// <param name="ThumbnailFileId">Identifier for the thumbnail of the media file.</param>
/// <param name="ThumbnailSize">File size of the thumbnail of the media file in bytes.</param>
/// <param name="MediaType">Type of the media file.</param>
/// <param name="Message">The message containing the media.</param>
public record MediaDetail(
    string FileUniqueId,
    string FileId,
    string? MimeType,
    long? Size,
    string? ThumbnailFileId,
    long? ThumbnailSize,
    TelegramMediaType MediaType,
    Message Message
);
