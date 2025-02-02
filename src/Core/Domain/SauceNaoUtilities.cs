// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Entities.SauceAggregate;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Domain;

/// <summary>
/// Provides utility methods used by the bot.
/// </summary>
public static class SauceNaoUtilities
{
    public const int MAX_PHOTO_SIZE = 20 * 1024 * 1024; // 20 MB
    public const int MAX_VIDEO_SIZE = 50 * 1024 * 1024; // 50 MB

    // Supported image formats.
    public static readonly string[] SUPPORTED_IMAGE_FORMATS =
    [
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "image/bmp",
        "image/tiff",
    ];

    /// <summary>
    /// Extracts the media details from a Telegram message.
    /// </summary>
    /// <param name="message">The message to extract the media from.</param>
    /// <returns>The media details extracted from the message.</returns>
    public static MediaDetail? ExtractMediaFromMessage(Message? message)
    {
        if (message is null)
        {
            return null;
        }

        string fileUniqueId;
        string fileId;
        long? fileSize;
        string? mimeType;
        string? thumbnailFileId;
        long? thumbnailSize;
        TelegramMediaType mediaType;

        if (message.Animation is not null)
        {
            fileUniqueId = message.Animation.FileUniqueId;
            fileId = message.Animation.FileId;
            fileSize = message.Animation.FileSize;
            mimeType = message.Animation.MimeType;
            thumbnailFileId = message.Animation.Thumbnail?.FileId;
            thumbnailSize = message.Animation.Thumbnail?.FileSize;
            mediaType = TelegramMediaType.Animation;
        }
        else if (message.Document is not null)
        {
            fileUniqueId = message.Document.FileUniqueId;
            fileId = message.Document.FileId;
            fileSize = message.Document.FileSize;
            mimeType = message.Document.MimeType;
            thumbnailFileId = message.Document.Thumbnail?.FileId;
            thumbnailSize = message.Document.Thumbnail?.FileSize;
            mediaType = TelegramMediaType.Document;
        }
        else if (message.Photo?.Any() == true)
        {
            var photo = message.Photo.OrderByDescending(p => p.FileSize).First();

            fileUniqueId = photo.FileUniqueId;
            fileId = photo.FileId;
            fileSize = photo.FileSize;
            mimeType = null;
            thumbnailFileId = null;
            thumbnailSize = null;
            mediaType = TelegramMediaType.Photo;
        }
        else if (message.Video is not null)
        {
            fileUniqueId = message.Video.FileUniqueId;
            fileId = message.Video.FileId;
            fileSize = message.Video.FileSize;
            mimeType = message.Video.MimeType;
            thumbnailFileId = message.Video.Thumbnail?.FileId;
            thumbnailSize = message.Video.Thumbnail?.FileSize;
            mediaType = TelegramMediaType.Video;
        }
        else if (message.Sticker is not null)
        {
            fileUniqueId = message.Sticker.FileUniqueId;
            fileId = message.Sticker.FileId;
            fileSize = message.Sticker.FileSize;
            mimeType = null;
            thumbnailFileId = message.Sticker.Thumbnail?.FileId;
            thumbnailSize = message.Sticker.Thumbnail?.FileSize;
            mediaType = TelegramMediaType.Sticker;
        }
        else
        {
            return null;
        }

        return new MediaDetail(
            fileUniqueId,
            fileId,
            mimeType,
            fileSize,
            thumbnailFileId,
            thumbnailSize,
            mediaType,
            message
        );
    }

    /// <summary>
    /// Extracts the first non-empty value from a collection of recipes.
    /// </summary>
    /// <param name="sauces">The collection of recipes to extract the value from.</param>
    /// <param name="selector">The selector function to extract the value from a recipe.</param>
    /// <returns>The first non-empty value extracted from the collection of recipes.</returns>
    public static string? FirstNonEmpty(
        this IEnumerable<Sauce> sauces,
        Func<Sauce, string?> selector
    )
    {
        return sauces.Select(selector).FirstOrDefault(value => !string.IsNullOrEmpty(value));
    }
}
