using Microsoft.Extensions.Options;
using SauceNAO.Application.Features.AntiCheats;
using SauceNAO.Application.Resources;
using SauceNAO.Core;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.Extensions.Commands;
using Telegram.BotAPI.UpdatingMessages;

namespace SauceNAO.Application.Features.Search;

[BotCommand("temp", "Create a temporary link to an image")]
[LocalizedBotCommand("es", "Crea un enlace temporal a una imagen")]
class TempCommand(
    IBotHelper helper,
    ITelegramBotClient client,
    ITelegramFileService fileService,
    IFrameExtractor frameExtractor,
    IBetterStringLocalizer<TempCommand> localizer,
    IOptions<GeneralOptions> botOptions
) : IBotCommandHandler
{
    private string? ApplicationUrl => botOptions.Value.ApplicationURL;
    private string GeneratingTmpUrlMessage => localizer["GeneratingTmpUrl"];
    private string TemporalUrlDoneMessage => localizer["TemporalUrlDone"];

    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );

        var media =
            SauceNaoUtilities.ExtractMediaFromMessage(message)
            ?? SauceNaoUtilities.ExtractMediaFromMessage(message.ReplyToMessage)
            ?? throw new InvalidPhotoException(message);

        var (user, group, languageCode) = await helper.UpsertDataFromMessageAsync(
            message,
            cancellationToken
        );

        // If the message was sent in a group and the message was sent by the bot, ignore it if the bot is in the AntiCheat list.
        if (group?.Restrictions.Any(r => r.RestrictedBotId == media.Message.From?.Id) == true)
        {
            throw new CheatingException(message);
        }

        if (!string.IsNullOrEmpty(languageCode))
        {
            localizer.ChangeCulture(languageCode);
        }

        // Send a message indicating that the bot is generating the link.
        Message? sentMessage = null;

        // Define a variable to store the image url.
        string? imageUrl = null;

        // If the image url is still null, try to get the image url from the media file.

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
                throw new UnsupportedFormatException(message);
            }

            if (media.Size is null || media.Size > SauceNaoUtilities.MAX_VIDEO_SIZE)
            {
                throw new TooBigFileException(message);
            }

            sentMessage = await this.CreateGeneratingMessage(
                message,
                media.Message.MessageId,
                cancellationToken
            );

            var videoPath = await fileService.GetFilePathAsync(media.FileId, cancellationToken);
            if (string.IsNullOrEmpty(videoPath))
            {
                throw new TooBigFileException(message, sentMessage);
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
                throw new UnsupportedFormatException(message);
            }

            sentMessage = await this.CreateGeneratingMessage(
                message,
                media.Message.MessageId,
                cancellationToken
            );
            imageUrl = await fileService.GetFileUrlAsync(media.FileId, true, cancellationToken);
        }

        // If the imageUrl is null but a thumbnail is available, try to get the image url from the thumbnail.
        if (string.IsNullOrEmpty(imageUrl) && !string.IsNullOrEmpty(media.ThumbnailFileId))
        {
            sentMessage ??= await this.CreateGeneratingMessage(
                message,
                media.Message.MessageId,
                cancellationToken
            );
            imageUrl = await fileService.GetFileUrlAsync(
                media.ThumbnailFileId,
                true,
                cancellationToken
            );
        }

        // If the image url is still null, then it's too big to download from Telegram servers.
        if (string.IsNullOrEmpty(imageUrl) || sentMessage?.Text is null)
        {
            throw new TooBigFileException(message, sentMessage);
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
            sentMessage.Chat.Id,
            sentMessage.MessageId,
            string.Format(this.TemporalUrlDoneMessage, imageUrl),
            parseMode: FormatStyles.HTML,
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
            replyMarkup: new InlineKeyboardMarkup(keyboard),
            cancellationToken: cancellationToken
        );
    }

    private Task<Message> CreateGeneratingMessage(
        Message message,
        int replyToMessageId,
        CancellationToken cancellationToken
    ) =>
        client.SendMessageAsync(
            message.Chat.Id,
            this.GeneratingTmpUrlMessage,
            replyParameters: new ReplyParameters()
            {
                MessageId = replyToMessageId,
                AllowSendingWithoutReply = true,
            },
            cancellationToken: cancellationToken
        );
}
