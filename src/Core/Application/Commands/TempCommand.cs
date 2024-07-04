// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Options;
using SauceNAO.Domain;
using SauceNAO.Domain.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.UpdatingMessages;

namespace SauceNAO.Application.Commands;

[TelegramBotCommand("temp", "Create a temporary link to an image")]
class TempCommand(
    ITelegramBotClient client,
    ITelegramFileService fileService,
    IFrameExtractor frameExtractor,
    IOptions<GeneralOptions> options
) : BotCommandBase
{
    private readonly string? ApplicationUrl = options.Value.ApplicationURL;

    private string InvalidPhotoMessage => this.Context.Localizer["InvalidPhoto"];
    private string TooBigFileMessage => this.Context.Localizer["TooBigFile"];
    private string UnsupportedFormatMessage => this.Context.Localizer["UnsupportedFormat"];
    private string GeneratingTmpUrlMessage => this.Context.Localizer["GeneratingTmpUrl"];
    private string TemporalUrlDoneMessage => this.Context.Localizer["TemporalUrlDone"];

    /// <inheritdoc />
    protected override async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken = default
    )
    {
        var media =
            SauceNaoUtilities.ExtractMediaFromMessage(message)
            ?? SauceNaoUtilities.ExtractMediaFromMessage(message.ReplyToMessage);

        // If no media was found, send an error message and return.
        if (media is null)
        {
            await client.SendMessageAsync(
                message.Chat.Id,
                this.InvalidPhotoMessage,
                replyParameters: new ReplyParameters()
                {
                    MessageId = message.MessageId,
                    AllowSendingWithoutReply = true
                },
                cancellationToken: cancellationToken
            );
            return;
        }

        // Send a message indicating that the bot is generating the link.
        var responseMessage = await client.SendMessageAsync(
            message.Chat.Id,
            this.GeneratingTmpUrlMessage,
            replyParameters: new ReplyParameters()
            {
                MessageId = media.Message.MessageId,
                AllowSendingWithoutReply = true
            },
            cancellationToken: cancellationToken
        );

        // Define a variable to store the image url.
        string? imageUrl = null;

        // If the image url is still null, try to get the image url from the media file.
        if (string.IsNullOrEmpty(imageUrl))
        {
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
                    await client.EditMessageTextAsync(
                        responseMessage.Chat.Id,
                        responseMessage.MessageId,
                        this.UnsupportedFormatMessage,
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                if (media.Size is null || media.Size > SauceNaoUtilities.MAX_VIDEO_SIZE)
                {
                    await client.EditMessageTextAsync(
                        responseMessage.Chat.Id,
                        responseMessage.MessageId,
                        this.TooBigFileMessage,
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                var videoPath = await fileService.GetFilePathAsync(
                    media.FileId,
                    cancellationToken
                );
                if (string.IsNullOrEmpty(videoPath))
                {
                    await client.EditMessageTextAsync(
                        responseMessage.Chat.Id,
                        responseMessage.MessageId,
                        this.TooBigFileMessage,
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                var frameFilename = $"{media.FileUniqueId}.jpg";
                var framePath = Path.Join(Path.GetTempPath(), frameFilename);
                await frameExtractor.ExtractAsync(videoPath, framePath, cancellationToken);
                imageUrl = $"{this.ApplicationUrl.TrimEnd('/')}/file/{frameFilename}";
            }
            else if (media.Size is null || media.Size <= SauceNaoUtilities.MAX_PHOTO_SIZE)
            {
                var mimeType = media.MimeType?.ToLowerInvariant();
                if (
                    mimeType is not null
                    && !SauceNaoUtilities.SUPPORTED_IMAGE_FORMATS.Contains(mimeType)
                )
                {
                    await client.EditMessageTextAsync(
                        responseMessage.Chat.Id,
                        responseMessage.MessageId,
                        this.UnsupportedFormatMessage,
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                imageUrl = await fileService.GetFileUrlAsync(
                    media.FileId,
                    true,
                    cancellationToken
                );
            }
        }

        // If the imageUrl is null but a thumbnail is available, try to get the image url from the thumbnail.
        if (string.IsNullOrEmpty(imageUrl) && !string.IsNullOrEmpty(media.ThumbnailFileId))
        {
            imageUrl = await fileService.GetFileUrlAsync(
                media.ThumbnailFileId,
                true,
                cancellationToken
            );
        }

        // If the image url is still null, then it's too big to download from Telegram servers.
        if (string.IsNullOrEmpty(imageUrl))
        {
            await client.EditMessageTextAsync(
                responseMessage.Chat.Id,
                responseMessage.MessageId,
                this.TooBigFileMessage,
                cancellationToken: cancellationToken
            );
            return;
        }

        var googleUrl = string.Format(ImageSearchLinks.GoogleImageSearch, imageUrl);
        var yandexUrl = string.Format(ImageSearchLinks.YandexImageSearch, imageUrl);
        var sauceNaoUrl = string.Format(ImageSearchLinks.SauceNAOsearch, imageUrl);
        var keyboard = new InlineKeyboardBuilder()
            .AppendUrl("Google", googleUrl)
            .AppendUrl("Yandex", yandexUrl)
            .AppendRow()
            .AppendUrl("SauceNAO", sauceNaoUrl);

        // Send the image url to the user.
        await client.EditMessageTextAsync(
            responseMessage.Chat.Id,
            responseMessage.MessageId,
            string.Format(this.TemporalUrlDoneMessage, imageUrl),
            parseMode: FormatStyles.HTML,
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
            replyMarkup: new InlineKeyboardMarkup(keyboard),
            cancellationToken: cancellationToken
        );
    }
}
