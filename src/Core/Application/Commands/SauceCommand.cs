// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Options;
using SauceNAO.Domain;
using SauceNAO.Domain.Entities.SauceAggregate;
using SauceNAO.Domain.Entities.UserAggregate;
using SauceNAO.Domain.Repositories;
using SauceNAO.Domain.Services;
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
    IUserRepository userRepository,
    ISauceNaoService sauceNao,
    IOptions<GeneralOptions> options
) : BotCommandBase
{
    private readonly string? SupportChatLink = options.Value.SupportChatInvitationLink;
    private readonly string? ApplicationUrl = options.Value.ApplicationURL;
    private string SupportChatText => this.Context.Localizer["SupportChat"];
    private string SearchingMsg => this.Context.Localizer["Searching"];
    private string InvalidPhotoMsg => this.Context.Localizer["InvalidPhoto"];
    private string TooBigFileMsg => this.Context.Localizer["TooBigFile"];
    private string UnsupportedFormatMsg => this.Context.Localizer["UnsupportedFormat"];
    private string NoPublicKeysMsg => this.Context.Localizer["NoPublicKeys"];
    private string NotFoundMsg => this.Context.Localizer["NotFound"];
    private string NotFoundWithoutLinksMsg => this.Context.Localizer["NotFoundWithoutLinks"];
    private string NotCheatingMsg => this.Context.Localizer["AnticheatsNoCheats"];
    private string BusyMsg =>
        this.Context.Localizer["Busy", options.Value.SupportChatInvitationLink];

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
        var similarity = 55;

        if (args.Length > 0)
        {
            if (int.TryParse(args[0], out var sim))
            {
                similarity = Math.Clamp(sim, 10, 90);
            }
        }

        // If no media was found, send an error message and return.
        if (media is null)
        {
            await client.SendMessageAsync(
                message.Chat.Id,
                this.InvalidPhotoMsg,
                replyParameters: new ReplyParameters()
                {
                    MessageId = message.MessageId,
                    AllowSendingWithoutReply = true
                },
                cancellationToken: cancellationToken
            );
            return;
        }

        // If the message was sent in a group and the message was sent by the bot, ignore it if the bot is in the AntiCheat list.
        if (
            this.Context.Group?.Restrictions.Any(r => r.RestrictedBotId == media.Message.From?.Id)
            == true
        )
        {
            await client.SendMessageAsync(
                message.Chat.Id,
                this.NotCheatingMsg,
                replyParameters: new ReplyParameters()
                {
                    MessageId = media.Message.MessageId,
                    AllowSendingWithoutReply = true
                },
                cancellationToken: cancellationToken
            );
            return;
        }

        // Send a message indicating that the bot is searching for the image source.
        var responseMessage = await client.SendMessageAsync(
            message.Chat.Id,
            this.SearchingMsg,
            replyParameters: new ReplyParameters()
            {
                MessageId = media.Message.MessageId,
                AllowSendingWithoutReply = true
            },
            cancellationToken: cancellationToken
        );

        // Define a variable to store the sauces
        IEnumerable<Sauce>? sauces = null;

        // Try to get the image url from the user history.
        var userRecord = this.User.SearchHistory.FirstOrDefault(s =>
            s.Media.FileUniqueId == media.FileUniqueId && s.Similarity == similarity
        );
        var sauceMedia =
            userRecord?.Media ?? seachedMediaRepository.GetByFileUniqueId(media.FileUniqueId);
        bool busy = false;

        string? imageUrl = null;
        string? publicImageUrl = null;

        // If the sauce media doesn't exist yet, perform a new search.
        if (sauceMedia is null)
        {
            // If a thumbnail is available, try to get the image url from the thumbnail.
            if (!string.IsNullOrEmpty(media.ThumbnailFileId))
            {
                imageUrl = await fileService.GetFileUrlAsync(
                    media.ThumbnailFileId,
                    cancellationToken
                );
            }

            // If the thumbnail is not available, try to get the original image url.
            if (string.IsNullOrEmpty(imageUrl))
            {
                (imageUrl, publicImageUrl, var error) = await this.GetHighQualityImageUrl(
                    media,
                    false,
                    cancellationToken
                );

                // If an error occurred, send the error message.
                if (!string.IsNullOrEmpty(error))
                {
                    await client.EditMessageTextAsync(
                        message.Chat.Id,
                        responseMessage.MessageId,
                        error,
                        cancellationToken: cancellationToken
                    );
                    return;
                }
            }

            // Get the api key entities to use them in the sauce search.
            IEnumerable<SauceApiKey> apiKeyEntities;
            if (this.User.ApiKeys.Count > 0)
            {
                apiKeyEntities = this.User.ApiKeys;
                // If the user share any public api key, then also get the public api keys in case the user's api keys are not enough.
                if (this.User.ApiKeys.Any(key => key.IsPublic))
                {
                    apiKeyEntities = apiKeyEntities.Concat(apiKeyRespository.GetPublicKeys());
                }
            }
            // If the user doesn't have any api keys, then get the public api keys.
            else
            {
                apiKeyEntities = apiKeyRespository.GetPublicKeys();
            }

            if (!apiKeyEntities.Any())
            {
                var supportKeyboard = string.IsNullOrEmpty(this.SupportChatLink)
                    ? null
                    : new InlineKeyboardMarkup(
                        new InlineKeyboardBuilder().AppendUrl(
                            this.SupportChatText,
                            this.SupportChatLink
                        )
                    );
                await client.EditMessageTextAsync(
                    responseMessage.Chat.Id,
                    responseMessage.MessageId,
                    this.NoPublicKeysMsg,
                    replyMarkup: supportKeyboard,
                    cancellationToken: cancellationToken
                );
                return;
            }

            // Order the api keys by the last usage date.
            apiKeyEntities = apiKeyEntities.OrderBy(key => key.UpdatedAt);

            // Try to search for the image source using the api keys.
            foreach (var keyEntity in apiKeyEntities)
            {
                var result = await sauceNao.SearchByUrlAsync(
                    (publicImageUrl ?? imageUrl)!,
                    keyEntity.Value,
                    cancellationToken
                );

                // If the search was successful, break the loop
                if (result is not null)
                {
                    // If the response is not ok, try to get the next api key.
                    if (!result.Ok)
                    {
                        // If the last api key usage date is older than 1 week, then remove it.
                        if (keyEntity.UpdatedAt < DateTime.UtcNow.AddDays(-7))
                        {
                            await apiKeyRespository.DeleteAsync(keyEntity, cancellationToken);
                        }

                        busy = true;
                        continue;
                    }

                    keyEntity.UpdatedAt = DateTime.UtcNow;
                    // Update the API KEY last usage date
                    await apiKeyRespository.UpdateAsync(keyEntity, cancellationToken);

                    // Save the sauces for future searches
                    sauces = result.Recipes.Select(r =>
                    {
                        var sauce = new Sauce()
                        {
                            Similarity = r.Similarity,
                            Author = r.Author,
                            Title = r.Title,
                            Characters = r.Characters,
                            EstimationTime = r.EstimationTime,
                            Material = r.Material,
                            Part = r.Part,
                            Year = r.Year
                        };
                        foreach (var link in r.Urls)
                        {
                            sauce.Links.Add(link);
                        }

                        return sauce;
                    });
                    sauceMedia = new SauceMedia()
                    {
                        FileId = media.FileId,
                        FileUniqueId = media.FileUniqueId,
                        MediaType = media.MediaType,
                        ThumbnailFileId = media.ThumbnailFileId,
                    };
                    foreach (var sauce in sauces)
                    {
                        sauceMedia.Sauces.Add(sauce);
                    }

                    // Save the sauce media.
                    await seachedMediaRepository.InsertAsync(sauceMedia, cancellationToken);

                    // Stop the iteration
                    break;
                }
            }
        }

        if (sauceMedia is null || !sauceMedia.Sauces.Any(s => s.Similarity >= similarity))
        {
            // if the traffic was reached, return.
            if (busy)
            {
                await client.EditMessageTextAsync(
                    responseMessage.Chat.Id,
                    responseMessage.MessageId,
                    this.BusyMsg,
                    parseMode: FormatStyles.HTML,
                    cancellationToken: cancellationToken
                );

                return;
            }

            // Try to get a temporal link to the file.
            if (string.IsNullOrEmpty(publicImageUrl))
            {
                (_, publicImageUrl, var error) = await this.GetHighQualityImageUrl(
                    media,
                    true,
                    cancellationToken
                );
                if (!string.IsNullOrEmpty(error))
                {
                    await client.EditMessageTextAsync(
                        responseMessage.Chat.Id,
                        responseMessage.MessageId,
                        error,
                        cancellationToken: cancellationToken
                    );
                    return;
                }
            }

            // If the media URL is not available, print the not found message without alternative links.
            if (string.IsNullOrEmpty(publicImageUrl))
            {
                await client.EditMessageTextAsync(
                    responseMessage.Chat.Id,
                    responseMessage.MessageId,
                    this.NotFoundWithoutLinksMsg,
                    cancellationToken: cancellationToken
                );
            }
            // Send the not found message with alternative linksf
            else
            {
                var googleUrl = string.Format(ImageSearchLinks.GoogleImageSearch, publicImageUrl);
                var yandexUrl = string.Format(ImageSearchLinks.YandexImageSearch, publicImageUrl);
                var sauceNaoUrl = string.Format(ImageSearchLinks.SauceNAOsearch, publicImageUrl);
                await client.EditMessageTextAsync(
                    responseMessage.Chat.Id,
                    responseMessage.MessageId,
                    string.Format(this.NotFoundMsg, googleUrl, yandexUrl, sauceNaoUrl),
                    parseMode: FormatStyles.HTML,
                    linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                    cancellationToken: cancellationToken
                );
            }
        }
        else
        {
            // If the record doesn't exist, create a new one.
            if (userRecord is null)
            {
                userRecord = new SearchRecord
                {
                    Media = sauceMedia,
                    Similarity = similarity,
                    SearchedAt = DateTime.UnixEpoch.AddSeconds(message.Date)
                };
                this.User.SearchHistory.Add(userRecord);
            }
            // Otherwise, update the date
            else
            {
                userRecord.SearchedAt = DateTime.UnixEpoch.AddSeconds(message.Date);
            }

            // Save the user history.
            await userRepository.UpdateAsync(this.User, cancellationToken);

            var kitchen = new Kitchen(this.Context.Localizer);

            // Cook the sauce
            var (sauceMsg, keyboard) = kitchen.CookSauce(sauceMedia.Sauces, similarity);

            await client.EditMessageTextAsync(
                responseMessage.Chat.Id,
                responseMessage.MessageId,
                sauceMsg,
                parseMode: FormatStyles.HTML,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );
        }
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
