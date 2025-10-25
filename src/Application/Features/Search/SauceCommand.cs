using Microsoft.Extensions.Options;
using SauceNAO.Application.Features.AntiCheats;
using SauceNAO.Application.Resources;
using SauceNAO.Core;
using SauceNAO.Core.Entities.SauceAggregate;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.Application.Features.Search;

[BotCommand(
    "sauce",
    "Look up for the image source.",
    ["source", "snao", "saucenao", "sourcenow", "search"]
)]
[LocalizedBotCommand("es", "salsa", "Buscar la fuente de la imagen")]
class SauceCommand(
    IBotHelper helper,
    ITelegramBotClient client,
    IBetterStringLocalizer<SauceCommand> localizer,
    IOptions<GeneralOptions> options
) : IBotCommandHandler
{
    private readonly string? SupportChatLink = options.Value.SupportChatInvitationLink;
    private readonly string? ApplicationUrl = options.Value.ApplicationURL;
    private string SupportChatText => localizer["SupportChat"];
    private string SearchingMsg => localizer["Searching"];
    private string NotFoundMsg => localizer["NotFound"];
    private string NotFoundWithoutLinksMsg => localizer["NotFoundWithoutLinks"];

    private string BusyMsg => localizer["Busy", options.Value.SupportChatInvitationLink];

    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        var (user, group, languageCode) = await helper.UpsertDataFromMessageAsync(
            message,
            cancellationToken
        );

        var media =
            SauceNaoUtilities.ExtractMediaFromMessage(message)
            ?? SauceNaoUtilities.ExtractMediaFromMessage(message.ReplyToMessage)
            ?? throw new InvalidPhotoException(message);

        // If the message was sent in a group and the message was sent by the bot, ignore it if the bot is in the AntiCheat list.
        if (group?.Restrictions.Any(r => r.RestrictedBotId == media.Message.From?.Id) == true)
        {
            throw new CheatingException(message);
        }

        if (!string.IsNullOrEmpty(languageCode))
        {
            localizer.ChangeCulture(languageCode);
        }

        var similarity = 55;

        if (args.Length > 0)
        {
            if (int.TryParse(args[0], out var sim))
            {
                similarity = Math.Clamp(sim, 10, 90);
            }
        }

        // Send a message indicating that the bot is searching for the image source.
        var responseMessage = await client.SendMessageAsync(
            message.Chat.Id,
            this.SearchingMsg,
            replyParameters: new ReplyParameters()
            {
                MessageId = media.Message.MessageId,
                AllowSendingWithoutReply = true,
            },
            cancellationToken: cancellationToken
        );

        // Define a variable to store the sauces
        IEnumerable<Sauce>? sauces = null;

        // Try to get the image url from the user history.
        var userRecord = user.SearchHistory.FirstOrDefault(s =>
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
                        if (keyEntity.UpdatedAt < DateTimeOffset.UtcNow.AddDays(-7))
                        {
                            await apiKeyRespository.DeleteAsync(keyEntity, cancellationToken);
                        }

                        busy = true;
                        continue;
                    }

                    keyEntity.UpdatedAt = DateTimeOffset.UtcNow;
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
                            Year = r.Year,
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
                    SearchedAt = DateTime.UnixEpoch.AddSeconds(message.Date),
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

            var kitchen = new Kitchen(localizer);

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
}
