using SauceNAO.Application.Resources;
using SauceNAO.Core;
using SauceNAO.Core.Entities.UserAggregate;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.InlineMode;

namespace SauceNAO.Application.Features.History;

/// <summary>
/// Handles inline queries related to search history.
/// </summary>
class HistoryInlineHandler(
    IUserRepository userManager,
    ISearchHistoryRepository historyRepository,
    ISauceMessageBuilder sauceBuilder,
    IBetterStringLocalizer<HistoryInlineHandler> localizer,
    ITelegramBotClient client
) : IInlineQueryHandler
{
    public async Task<bool> HandleAsync(
        InlineQuery inlineQuery,
        CancellationToken cancellationToken = default
    )
    {
        var offset = string.IsNullOrEmpty(inlineQuery.Offset) ? 0 : int.Parse(inlineQuery.Offset);
        var user = await userManager.UpsertAsync(inlineQuery.From, cancellationToken);
        var records = await historyRepository.GetByUserIdAsync(
            user.Id,
            inlineQuery.Query,
            10,
            offset,
            cancellationToken
        );

        if (user.LanguageCode is not null)
        {
            localizer.ChangeCulture(user.LanguageCode);
        }

        var results = records
            .Select<SearchRecord, InlineQueryResult>(record =>
            {
                // Cook the sauce
                var sauceText = sauceBuilder.BuildText(
                    record.Media.Sauces,
                    record.Similarity,
                    user.LanguageCode
                );
                var keyboard = sauceBuilder.BuildKeyboard(record.Media.Sauces, record.Similarity);
                var title = record.Media.Sauces.FirstNonEmpty(r => r.Title);
                if (string.IsNullOrEmpty(title))
                {
                    title = "Noname";
                }

                return record.Media.MediaType switch
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
            })
            .ToList();

        // Update the result Id
        foreach (var result in results)
        {
            offset++;
            result.Id = offset.ToString();
        }

        if (!records.Any())
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

        var answeriquery = new AnswerInlineQueryArgs(inlineQuery.Id, [.. results])
        {
            NextOffset = results.Count < 10 ? string.Empty : offset.ToString(),
            IsPersonal = true,
#if DEBUG
            CacheTime = 16
#else
            CacheTime = 480
#endif
        };

        await client.AnswerInlineQueryAsync(answeriquery, cancellationToken: cancellationToken);

        return true;
    }
}
