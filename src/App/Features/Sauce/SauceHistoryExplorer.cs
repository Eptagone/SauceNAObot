// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.App.Resources;
using SauceNAO.Core;
using SauceNAO.Core.Data;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.InlineMode;

namespace SauceNAO.App.Features.Sauce;

/// <summary>
/// Handles inline queries
/// </summary>
sealed class SauceHistoryExplorer(
    IContextProvider contextProvider,
    ISearchHistoryRepository historyRepository,
    ISauceHandler sauceHandler,
    IBetterStringLocalizer<SauceHistoryExplorer> localizer,
    ITelegramBotClient client
) : IInlineQueryHandler
{
    public async Task<bool> TryHandleAsync(
        InlineQuery inlineQuery,
        CancellationToken cancellationToken = default
    )
    {
        await this.HandleAsync(inlineQuery, cancellationToken);
        return true;
    }

    private async Task HandleAsync(InlineQuery inlineQuery, CancellationToken cancellationToken)
    {
        var offset = string.IsNullOrEmpty(inlineQuery.Offset) ? 0 : int.Parse(inlineQuery.Offset);
        var items = await historyRepository.SearchAsync(
            inlineQuery.From.Id,
            string.Join(' ', inlineQuery.Query),
            10,
            offset,
            cancellationToken
        );

        var user = await contextProvider.LoadAsync(inlineQuery.From, cancellationToken);

        var results = items
            .Select(record =>
            {
                var sauces = record.Media.Sauces.Where(s => s.Similarity >= record.Similarity);
                var (sauceText, keyboard) = sauceHandler.CookSauces(sauces);
                var title = sauces
                    .Select(s => s.Title)
                    .FirstOrDefault(title => !string.IsNullOrEmpty(title));
                if (string.IsNullOrEmpty(title))
                {
                    title = "Noname";
                }

                InlineQueryResult result = record.Media.MediaType switch
                {
                    TelegramMediaType.Animation => new InlineQueryResultCachedGif
                    {
                        Id = offset.ToString(),
                        GifFileId = record.Media.FileId,
                        Title = title,
                        Caption = sauceText,
                        ParseMode = FormatStyles.HTML,
                        ReplyMarkup = keyboard,
                    },
                    TelegramMediaType.Photo => new InlineQueryResultCachedPhoto
                    {
                        Id = offset.ToString(),
                        PhotoFileId = record.Media.FileId,
                        Title = title,
                        Description = record.Media.MediaType.ToString(),
                        Caption = sauceText,
                        ParseMode = FormatStyles.HTML,
                        ReplyMarkup = keyboard,
                    },
                    TelegramMediaType.Sticker => new InlineQueryResultCachedSticker
                    {
                        Id = offset.ToString(),
                        StickerFileId = record.Media.FileId,
                        InputMessageContent = new InputTextMessageContent(sauceText)
                        {
                            ParseMode = FormatStyles.HTML,
                        },
                        ReplyMarkup = keyboard,
                    },
                    TelegramMediaType.Video => new InlineQueryResultCachedVideo
                    {
                        Id = offset.ToString(),
                        VideoFileId = record.Media.FileId,
                        Title = title,
                        Description = record.Media.MediaType.ToString(),
                        Caption = sauceText,
                        ParseMode = FormatStyles.HTML,
                        ReplyMarkup = keyboard,
                    },
                    _ => new InlineQueryResultCachedDocument
                    {
                        Id = offset.ToString(),
                        DocumentFileId = record.Media.FileId,
                        Title = title,
                        Description = record.Media.MediaType.ToString(),
                        Caption = sauceText,
                        ParseMode = FormatStyles.HTML,
                        ReplyMarkup = keyboard,
                    },
                };
                return result;
            })
            .ToList();

        if (results.Count == 0)
        {
            var text = localizer["HistoryEmpty"];
            results.Add(
                new InlineQueryResultArticle
                {
                    Id = "None",
                    Title = text,
                    InputMessageContent = new InputTextMessageContent(text),
                }
            );
        }

        // Update the result Id
        foreach (var result in results)
        {
            offset++;
            result.Id = offset.ToString();
        }

        var answer = new AnswerInlineQueryArgs(inlineQuery.Id, results.ToArray())
        {
            NextOffset = results.Count < 10 ? null : offset.ToString(),
            IsPersonal = true,
#if DEBUG
            CacheTime = 8
#else
            CacheTime = 240
#endif
        };

        await client.AnswerInlineQueryAsync(answer, cancellationToken: cancellationToken);
    }
}
