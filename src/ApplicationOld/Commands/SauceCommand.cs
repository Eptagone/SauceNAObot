// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Options;
using SauceNAO.Core;
using SauceNAO.Core.Entities.SauceAggregate;
using SauceNAO.Core.Entities.UserAggregate;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.UpdatingMessages;

namespace SauceNAO.Application.Commands;

[TelegramBotCommand(
    "sauce",
    "Look up for the image source.",
    ["source", "salsa", "snao", "saucenao", "sourcenow", "search"]
)]
class SauceCommand(
    ITelegramBotClient client,
    ITelegramFileService fileService,
    IFrameExtractor frameExtractor,
    IApiKeyRespository apiKeyRespository,
    ISauceMediaRepository seachedMediaRepository,
    Domain.Repositories.IUserRepository userRepository,
    ISauceNaoService sauceNao,
    IOptions<GeneralOptions> options
) : BotCommandBase
{


    /// <inheritdoc />
    protected override async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken = default
    )
    {
       }

    // Define a function to generate the required urls to get the sauce.
    // It returns null if the URLs could not be generated and the command should be aborted.
    private async Task<(
        string? ImageUrl,
        string? PublicImageUrl,
        string? Error
    )> GetHighQualityImageUrl(MediaDetail media, bool isPublic, CancellationToken cancellationToken)
    {
        string? imageUrl = null;
        string? publicImageUrl = null;

        // If the media is a video, get the frame.
        if (
            media.MimeType?.StartsWith("video/", StringComparison.InvariantCultureIgnoreCase)
                == true
            || media.MediaType == TelegramMediaType.Video
            || media.Message.Sticker?.IsVideo == true
        )
        {
            // Videos are not supported in any way if the webhook is not available.
            if (string.IsNullOrEmpty(this.ApplicationUrl))
            {
                return (null, null, this.UnsupportedFormatMsg);
            }

            if (media.Size is null || media.Size > SauceNaoUtilities.MAX_VIDEO_SIZE)
            {
                return (null, null, this.TooBigFileMsg);
            }

            var videoPath = await fileService.GetFilePathAsync(media.FileId, cancellationToken);
            if (string.IsNullOrEmpty(videoPath))
            {
                return (null, null, this.TooBigFileMsg);
            }

            var frameFilename = $"{media.FileUniqueId}.jpg";
            var framePath = Path.Join(Path.GetTempPath(), frameFilename);
            await frameExtractor.ExtractAsync(videoPath, framePath, cancellationToken);
            publicImageUrl = $"{this.ApplicationUrl.TrimEnd('/')}/file/{frameFilename}";
        }
        else if (media.Size is null || media.Size <= SauceNaoUtilities.MAX_PHOTO_SIZE)
        {
            var mimeType = media.MimeType?.ToLowerInvariant();
            if (
                mimeType is not null
                && !SauceNaoUtilities.SUPPORTED_IMAGE_FORMATS.Contains(mimeType)
            )
            {
                return (null, null, this.UnsupportedFormatMsg);
            }

            var url = await fileService.GetFileUrlAsync(media.FileId, isPublic, cancellationToken);

            if (isPublic)
            {
                publicImageUrl = url;
            }
            else
            {
                imageUrl = url;
            }
        }

        // If the image url is null, then it's too big to download from Telegram servers.
        if (string.IsNullOrEmpty(imageUrl) && string.IsNullOrEmpty(publicImageUrl))
        {
            return (null, null, this.TooBigFileMsg);
        }

        return (imageUrl, publicImageUrl, null);
    }
}
