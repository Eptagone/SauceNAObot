using SauceNAO.Core;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;
using SauceNAO.Core.Exceptions.Media;
using SauceNAO.Core.Models;
using SauceNAO.Core.Services;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.App.Services;

sealed class MediaExtractor(IMediaFileRepository mediaRepository) : IMediaExtractor
{
    private const int MAX_FILE_SIZE = 20 * 1024 * 1024; // 20 MB

    // Supported image formats.
    private static readonly string[] SUPPORTED_IMAGE_FORMATS =
    [
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "image/bmp",
        "image/tiff",
    ];

    public async Task<MediaTarget?> DeepExtractAsync(
        Message message,
        CancellationToken cancellationToken
    )
    {
        if (message.ReplyToMessage is null)
        {
            return await this.ExtractAsync(message, cancellationToken);
        }

        MediaTarget? media;

        try
        {
            media = await this.ExtractAsync(message.ReplyToMessage, cancellationToken);
        }
        catch (MessageMediaException exp)
        {
            exp.ReceivedMessage = message;
            exp.MediaMessage = message.ReplyToMessage;
            throw;
        }

        if (media is null)
        {
            media = await this.ExtractAsync(message, cancellationToken);
        }
        else
        {
            media.Message = message;
            media.MediaMessage = message.ReplyToMessage;
        }

        return media;
    }

    public async Task<MediaTarget?> ExtractAsync(
        Message message,
        CancellationToken cancellationToken
    )
    {
        MediaFile? media = null;

        // TODO: Consider to make the FileSize as a required field when looking for the media target.
        if (message.Animation is not null)
        {
            media = new MediaFile(message.Animation.FileUniqueId, message.Animation.FileId)
            {
                MediaType = TelegramMediaType.Animation,
                MimeType = message.Animation.MimeType,
            };

            if (media.MimeType is null)
            {
                throw new UnsupportedFormatException(message);
            }

            if (media.MimeType.StartsWith("video/"))
            {
                if (
                    message.Animation.Thumbnail?.FileSize is not null
                    && message.Animation.Thumbnail.FileSize < MAX_FILE_SIZE
                )
                {
                    media.ThumbnailFileId = message.Animation.Thumbnail.FileId;
                }
            }
            else if (!SUPPORTED_IMAGE_FORMATS.Contains(media.MimeType))
            {
                throw new UnsupportedFormatException(message);
            }

            if (
                media.ThumbnailFileId is null
                && message.Animation.FileSize is not null
                && message.Animation.FileSize > MAX_FILE_SIZE
            )
            {
                throw new TooBigFileException(message);
            }
        }
        else if (message.Document is not null)
        {
            media = new MediaFile(message.Document.FileUniqueId, message.Document.FileId)
            {
                MediaType = TelegramMediaType.Document,
                MimeType = message.Document.MimeType,
            };

            if (media.MimeType is null)
            {
                throw new InvalidPhotoException(message);
            }
            if (!SUPPORTED_IMAGE_FORMATS.Contains(media.MimeType))
            {
                throw new UnsupportedFormatException(message);
            }
        }
        else if (message.Photo?.Any() is true)
        {
            var photo =
                message
                    .Photo.Where(p => p.FileSize is null || p.FileSize < MAX_FILE_SIZE)
                    .OrderByDescending(p => p.FileSize)
                    .FirstOrDefault()
                ?? throw new TooBigFileException(message);

            media = new MediaFile(photo.FileUniqueId, photo.FileId)
            {
                MediaType = TelegramMediaType.Photo,
            };
        }
        else if (message.Sticker is not null)
        {
            media = new MediaFile(message.Sticker.FileUniqueId, message.Sticker.FileId)
            {
                MediaType = TelegramMediaType.Sticker,
            };

            if (message.Sticker.IsAnimated || message.Sticker.IsVideo)
            {
                if (message.Sticker.Thumbnail is null)
                {
                    throw new UnsupportedFormatException(message);
                }
                media.ThumbnailFileId = message.Sticker.Thumbnail.FileId;
                if (
                    message.Sticker.Thumbnail.FileSize is not null
                    && message.Sticker.Thumbnail.FileSize > MAX_FILE_SIZE
                )
                {
                    throw new TooBigFileException(message);
                }
            }
        }
        else if (message.Video is not null)
        {
            media = new MediaFile(message.Video.FileUniqueId, message.Video.FileId)
            {
                MediaType = TelegramMediaType.Video,
                MimeType = message.Video.MimeType,
            };

            if (
                message.Video.Thumbnail?.FileSize is null
                || message.Video.Thumbnail.FileSize < MAX_FILE_SIZE
            )
            {
                media.ThumbnailFileId = message.Video.Thumbnail?.FileId;
            }

            if (
                media.ThumbnailFileId is null
                && message.Video.FileSize is not null
                && message.Video.FileSize > MAX_FILE_SIZE
            )
            {
                throw new TooBigFileException(message);
            }
        }

        if (media is not null)
        {
            var existingMedia = await mediaRepository.GetByFileUniqueIdAsync(
                media.FileUniqueId,
                cancellationToken
            );
            media = existingMedia is null
                ? await mediaRepository.InsertAsync(media, cancellationToken)
                : existingMedia;

            return new(media, message);
        }

        return null;
    }
}
