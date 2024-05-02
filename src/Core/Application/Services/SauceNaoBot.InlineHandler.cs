// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain;
using SauceNAO.Domain.Entities.UserAggregate;
using Telegram.BotAPI;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.InlineMode;

namespace SauceNAO.Application.Services;

partial class SauceNaoBot : SimpleTelegramBotBase, ISauceNaoBot
{
    /// <inheritdoc />
    protected override Task OnInlineQueryAsync(
        InlineQuery inlineQuery,
        CancellationToken cancellationToken = default
    )
    {
        var offset = string.IsNullOrEmpty(inlineQuery.Offset) ? 0 : int.Parse(inlineQuery.Offset);
        var myHistory = this.Context.User!.SearchHistory.OrderByDescending(h => h.SearchedAt);
        var keywords = inlineQuery.Query.Split(' ');

        var mySauces = keywords.Any()
            ? myHistory
                .Where(record =>
                    keywords.Any(k =>
                        record.Media.Sauces.Any(s => s.Similarity >= record.Similarity)
                        && record.Media.Sauces.Any(s =>
                            s.Title?.Contains(k, StringComparison.InvariantCultureIgnoreCase)
                                == true
                            || s.Author?.Contains(k, StringComparison.InvariantCultureIgnoreCase)
                                == true
                            || s.Characters?.Contains(
                                k,
                                StringComparison.InvariantCultureIgnoreCase
                            ) == true
                            || s.Material?.Contains(k, StringComparison.InvariantCultureIgnoreCase)
                                == true
                        )
                    )
                )
                .Skip(offset)
                .Take(10)
            : myHistory.Skip(offset).Take(10);

        var kitchen = new Kitchen(this.Context.Localizer);

        var results = mySauces
            .Select<SearchRecord, InlineQueryResult>(record =>
            {
                // Cook the sauce
                var (sauceText, keyboard) = kitchen.CookSauce(
                    record.Media.Sauces,
                    record.Similarity
                );
                var title = record.Media.Sauces.FirstNonEmpty(r => r.Title);
                if (string.IsNullOrEmpty(title))
                {
                    title = "Noname";
                }

                return record.Media.MediaType switch
                {
                    TelegramMediaType.Animation
                        => new InlineQueryResultCachedGif
                        {
                            Id = offset.ToString(),
                            GifFileId = record.Media.FileId,
                            Title = title,
                            Caption = sauceText,
                            ParseMode = FormatStyles.HTML,
                            ReplyMarkup = keyboard
                        },
                    TelegramMediaType.Photo
                        => new InlineQueryResultCachedPhoto
                        {
                            Id = offset.ToString(),
                            PhotoFileId = record.Media.FileId,
                            Title = title,
                            Description = record.Media.MediaType.ToString(),
                            Caption = sauceText,
                            ParseMode = FormatStyles.HTML,
                            ReplyMarkup = keyboard
                        },
                    TelegramMediaType.Sticker
                        => new InlineQueryResultCachedSticker
                        {
                            Id = offset.ToString(),
                            StickerFileId = record.Media.FileId,
                            InputMessageContent = new InputTextMessageContent(sauceText)
                            {
                                ParseMode = FormatStyles.HTML
                            },
                            ReplyMarkup = keyboard
                        },
                    TelegramMediaType.Video
                        => new InlineQueryResultCachedVideo
                        {
                            Id = offset.ToString(),
                            VideoFileId = record.Media.FileId,
                            Title = title,
                            Description = record.Media.MediaType.ToString(),
                            Caption = sauceText,
                            ParseMode = FormatStyles.HTML,
                            ReplyMarkup = keyboard
                        },
                    _
                        => new InlineQueryResultCachedDocument
                        {
                            Id = offset.ToString(),
                            DocumentFileId = record.Media.FileId,
                            Title = title,
                            Description = record.Media.MediaType.ToString(),
                            Caption = sauceText,
                            ParseMode = FormatStyles.HTML,
                            ReplyMarkup = keyboard
                        },
                };
            })
            .ToList();

        if (!mySauces.Any())
        {
            var text = this.Context.Localizer["HistoryEmpty"];
            results.Add(
                new InlineQueryResultArticle
                {
                    Id = "None",
                    Title = text,
                    InputMessageContent = new InputTextMessageContent(text)
                }
            );
        }

        // Update the result Id
        foreach (var result in results)
        {
            offset++;
            result.Id = offset.ToString();
        }

        var answeriquery = new AnswerInlineQueryArgs(inlineQuery.Id, results.ToArray())
        {
            NextOffset = results.Count < 10 ? string.Empty : offset.ToString(),
            IsPersonal = true,
#if DEBUG
            CacheTime = 16
#else
            CacheTime = 480
#endif
        };

        return this.client.AnswerInlineQueryAsync(
            answeriquery,
            cancellationToken: cancellationToken
        );
    }
}
