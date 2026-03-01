// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text;
using SauceNAO.App.Resources;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;
using SauceNAO.Core.Entities.UserAggregate;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Exceptions.Sauce;
using SauceNAO.Core.Models;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.UpdatingMessages;

namespace SauceNAO.App.Services;

sealed class SauceHandler(
    ITelegramBotClient client,
    IContextProvider contextProvider,
    IMediaUrlGenerator urlGenerator,
    ISauceNAOClient snao,
    ISearchHistoryRepository searchHistory,
    IMediaFileRepository mediaRepository,
    IApiKeyRespository apiKeyRepository,
    IBetterStringLocalizer<SauceHandler> localizer
) : ISauceHandler
{
    private static readonly IReadOnlyDictionary<string, string> SITES = new Dictionary<
        string,
        string
    >()
    {
        { "pixiv", "Pixiv" },
        { "danbooru", "Danbooru" },
        { "gelbooru", "Gelbooru" },
        { "sankaku", "Sankaku" },
        { "anime-pictures.net", "Anime Pictures" },
        { "yande.re", "Yandere" },
        { "imdb", "IMDB" },
        { "deviantart", "Deviantart" },
        { "patreon", "Patreon" },
        { "anilist", "AniList" },
        { "artstation", "ArtStation" },
        { "twitter", "Twitter" },
        { "x.com", "X" },
        { "nijie.info", "Nijie" },
        { "pawoo.net", "Pawoo" },
        { "seiga.nicovideo.jp", "Seiga Nicovideo" },
        { "tumblr.com", "Tumblr" },
        { "anidb.net", "Anidb" },
        { "sankakucomplex.com", "Sankaku" },
        { "mangadex.org", "MangaDex" },
        { "mangaupdates.com", "MangaUpdates" },
        { "myanimelist.net", "MyAnimeList" },
        { "furaffinity.net", "FurAffinity" },
        { "fakku.net", "FAKKU!" },
        { "nhentai.net", "nhentai" },
        { "e-hentai.org", "E-Hentai" },
        { "e621.net", "e621" },
        { "kemono.su", "Kemono" },
    };

    public async Task HandleAsync(MediaTarget target, CancellationToken cancellationToken = default)
    {
        await this.HandleAsync(target, 55, cancellationToken);
    }

    public async Task HandleAsync(
        MediaTarget target,
        float similarity,
        CancellationToken cancellationToken = default
    )
    {
        await client.SendChatActionAsync(
            target.Message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );

        var message = target.Message;
        if (target.Media.Sauces.Any() && (DateTimeOffset.Now - target.Media.UpdatedAt).Days < 10)
        {
            var u = await contextProvider.LoadAsync(message, cancellationToken);
            var sauces = target.Media.Sauces.Where(s => s.Similarity >= similarity);
            if (!sauces.Any())
            {
                throw new SauceNotFoundException(
                    message,
                    await urlGenerator.GenerateSafeAsync(target, cancellationToken)
                );
            }
            var (text, keyboard) = this.CookSauces(sauces);
            await client.SendMessageAsync(
                message.Chat.Id,
                text,
                parseMode: FormatStyles.HTML,
                replyParameters: new()
                {
                    MessageId = target.MediaMessage?.MessageId ?? message.MessageId,
                    AllowSendingWithoutReply = true,
                },
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );
            await this.UpdateHistoryAsync(u, target.Media, similarity, cancellationToken);
            return;
        }

        var privateKeys = await apiKeyRepository.GetByUserIdAsync(
            message.From!.Id,
            cancellationToken
        );
        var publicKeys = await apiKeyRepository.GetPublicKeysAsync(cancellationToken);
        var apiKeys = privateKeys.Concat(publicKeys).ToArray();
        if (apiKeys.Length == 0)
        {
            throw new NoApiKeysException(message);
        }
        Random.Shared.Shuffle(apiKeys);

        var user = await contextProvider.LoadAsync(message, cancellationToken);
        var sentMessage = await client.SendMessageAsync(
            message.Chat.Id,
            localizer["Searching"],
            parseMode: FormatStyles.HTML,
            replyParameters: new()
            {
                AllowSendingWithoutReply = true,
                MessageId = message.MessageId,
            },
            cancellationToken: cancellationToken
        );

        string searchUrl;
        try
        {
            searchUrl = await urlGenerator.GenerateAsync(target, cancellationToken);
        }
        catch (DownloadFailedException exp)
        {
            exp.SentMessage = sentMessage;
            throw;
        }

        SauceNAOException? lastException = null;
        for (int i = 0; i < apiKeys.Length; i++)
        {
            var key = apiKeys[i];
            try
            {
                var sauces = await snao.SearchByUrlAsync(searchUrl, key.Value, cancellationToken);
                target.Media.Sauces = [.. sauces];
                await mediaRepository.UpdateAsync(target.Media, cancellationToken);
                sauces = sauces.Where(s => s.Similarity >= similarity);
                if (!sauces.Any())
                {
                    throw new SauceNotFoundException(
                        message,
                        await urlGenerator.GenerateSafeAsync(target, cancellationToken)
                    )
                    {
                        SentMessage = sentMessage,
                    };
                }
                var (text, keyboard) = this.CookSauces(sauces);
                await client.EditMessageTextAsync(
                    message.Chat.Id,
                    sentMessage.MessageId,
                    text,
                    parseMode: FormatStyles.HTML,
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken
                );
                await this.UpdateHistoryAsync(user, target.Media, similarity, cancellationToken);
                return;
            }
            catch (SearchLimitReachedException exp)
            {
                lastException = exp;
                if (i == apiKeys.Length - 1)
                {
                    throw new SauceException(message, exp) { SentMessage = sentMessage };
                }
                continue;
            }
            catch (InvalidApiKeyException exp)
            {
                lastException = exp;
                await apiKeyRepository.DeleteAsync(key, cancellationToken);
                if (i == apiKeys.Length - 1)
                {
                    throw new SauceException(message, exp) { SentMessage = sentMessage };
                }
                continue;
            }
            catch (SauceNotFoundException)
            {
                throw;
            }
            catch (SauceNAOException exp)
            {
                throw new SauceException(message, exp) { SentMessage = sentMessage };
            }
            catch (Exception exp)
            {
                throw new UnknownMessageException(message, exp) { SentMessage = sentMessage };
            }
        }

        throw new SauceException(message, lastException) { SentMessage = sentMessage };
    }

    private async Task UpdateHistoryAsync(
        UserEntity user,
        MediaFile media,
        float similarity,
        CancellationToken cancellationToken
    )
    {
        var record = await searchHistory.FirstWithMediaAndSimilarityAsync(
            user.Id,
            media.Id,
            similarity,
            cancellationToken
        );
        if (record is null)
        {
            record = new SearchRecord(similarity) { Media = media, User = user };
            await searchHistory.InsertAsync(record, cancellationToken);
            return;
        }

        await searchHistory.UpdateAsync(record, cancellationToken);
    }

    public ValueTuple<string, InlineKeyboardMarkup> CookSauces(IEnumerable<Sauce> sauces)
    {
        var builder = new StringBuilder();
        var title = sauces.Select(s => s.Title).FirstOrDefault(v => !string.IsNullOrEmpty(v));
        var author = sauces.Select(s => s.Author).FirstOrDefault(v => !string.IsNullOrEmpty(v));
        var characters = sauces
            .Select(s => s.Characters)
            .FirstOrDefault(v => !string.IsNullOrEmpty(v));
        var material = sauces.Select(s => s.Material).FirstOrDefault(v => !string.IsNullOrEmpty(v));
        var part = sauces.Select(r => r.Part).FirstOrDefault(v => !string.IsNullOrEmpty(v));
        var year = sauces.Select(r => r.Year).FirstOrDefault(v => !string.IsNullOrEmpty(v));
        var estimationTime = sauces
            .Select(r => r.EstimationTime)
            .FirstOrDefault(v => !string.IsNullOrEmpty(v));

        // Add the title to the sauce.
        if (!string.IsNullOrEmpty(title))
        {
            builder.AppendLine(HtmlTextFormatter.Bold(title));
            builder.AppendLine();
        }
        // Characters
        if (!string.IsNullOrEmpty(characters))
        {
            var label = characters.Contains(',') ? localizer["Characters"] : localizer["Character"];
            builder.Append(HtmlTextFormatter.Bold($"{localizer[label]}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(characters));
        }
        // Material
        if (!string.IsNullOrEmpty(material))
        {
            builder.Append(HtmlTextFormatter.Bold($"{localizer["Material"]}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(material));
        }
        // Part
        if (!string.IsNullOrEmpty(part))
        {
            builder.Append(HtmlTextFormatter.Bold($"{localizer["Part"]}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(part));
        }
        // Creator
        if (!string.IsNullOrEmpty(author))
        {
            builder.Append(HtmlTextFormatter.Bold($"{localizer["Creator"]}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(author));
        }
        // Year
        if (!string.IsNullOrEmpty(year))
        {
            builder.Append(HtmlTextFormatter.Bold($"{localizer["Year"]}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(year));
        }
        // Estimation Time
        if (!string.IsNullOrEmpty(estimationTime))
        {
            builder.Append(HtmlTextFormatter.Bold($"{localizer["EstimationTime"]}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(estimationTime));
        }

        var keyboard = new InlineKeyboardBuilder();
        foreach (var originalUrl in sauces.SelectMany(r => r.Links).Distinct())
        {
            var finalUrl = originalUrl;
            var text = "URL";
            // Fix Pixiv URLs
            if (originalUrl.Contains("i.pximg.net"))
            {
                string pid = Path.GetFileNameWithoutExtension(originalUrl);
                finalUrl = string.Format(
                    "http://www.pixiv.net/member_illust.php?mode=medium&amp;illust_id={0}",
                    pid.Split('_')[0]
                );
            }
            // Fix Kemono URLs
            else if (originalUrl.Contains("kemono.party"))
            {
                finalUrl = originalUrl.Replace("kemono.party", "kemono.su");
            }

            // Get the site name from the URL.
            foreach (var site in SITES)
            {
                if (finalUrl.Contains(site.Key))
                {
                    text = site.Value;
                    break;
                }
            }

            // Append a new row if the last row has 3 buttons.
            if (keyboard.Any() && keyboard.Last().Count() >= 3)
            {
                keyboard.AppendRow();
            }
            // Append the button to the buttons.
            keyboard.AppendUrl(text, finalUrl);
        }

        var sauceMsg = builder.ToString();
        // Truncate the text if it's longer than 4096 characters.
        if (sauceMsg.Length > 4096)
        {
            sauceMsg = sauceMsg[..4096];
        }
        else if (string.IsNullOrWhiteSpace(sauceMsg))
        {
            sauceMsg = "Unknown sauce";
        }

        // Return the sauce and the buttons.
        return (sauceMsg, new InlineKeyboardMarkup(keyboard));
    }
}
