// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities;
using SauceNAO.Core.Enums;
using SauceNAO.Core.Models;
using SauceNAO.Core.Resources;
using System.Globalization;
using System.Text.Json;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.InlineMode;

namespace SauceNAO.Core
{
    public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
    {
        protected override async Task OnInlineQueryAsync(InlineQuery inlineQuery, CancellationToken cancellationToken)
        {
            User = await db.Users.GetUserAsync(inlineQuery.From, cancellationToken: cancellationToken).ConfigureAwait(false);
            Language = new CultureInfo(User.LanguageCode ?? "en");

            var offset = string.IsNullOrEmpty(inlineQuery.Offset) ? 0 : int.Parse(inlineQuery.Offset);
            var myHistory = User.UserSauces.Where(h => h.UserId == User.Id).OrderByDescending(h => h.Date).Select(s => s.Sauce);
            var results = new List<InlineQueryResult>();
            if (myHistory.Any())
            {
                void AddUserSauces(IEnumerable<SuccessfulSauce> sauces)
                {
                    foreach (SuccessfulSauce sauce in sauces)
                    {
                        var title = "Noname";
                        if (!string.IsNullOrEmpty(sauce.Info))
                        {
                            var info = JsonSerializer.Deserialize<Sauce>(sauce.Info);
                            if (!string.IsNullOrEmpty(info?.Title))
                            {
                                title = info.Title;
                            }
                        }
                        var description = sauce.Type;
                        var sauceText = sauce.GetInfo(Language);
                        var keyboard = sauce.GetKeyboard();
                        var type = Enum.Parse<MediaType>(sauce.Type);
                        offset++;
                        switch (type)
                        {
                            case MediaType.Animation:
                                results.Add(new InlineQueryResultCachedGif
                                {
                                    Id = offset.ToString(),
                                    GifFileId = sauce.FileId,
                                    Title = title,
                                    Caption = sauceText,
                                    ParseMode = ParseMode.HTML,
                                    ReplyMarkup = keyboard
                                });
                                break;
                            case MediaType.Document:
                                results.Add(new InlineQueryResultCachedDocument
                                {
                                    Id = offset.ToString(),
                                    DocumentFileId = sauce.FileId,
                                    Title = title,
                                    Description = description,
                                    Caption = sauceText,
                                    ParseMode = ParseMode.HTML,
                                    ReplyMarkup = keyboard
                                });
                                break;
                            case MediaType.Photo:
                                results.Add(new InlineQueryResultCachedPhoto
                                {
                                    Id = offset.ToString(),
                                    PhotoFileId = sauce.FileId,
                                    Title = title,
                                    Description = description,
                                    Caption = sauceText,
                                    ParseMode = ParseMode.HTML,
                                    ReplyMarkup = keyboard
                                });
                                break;
                            case MediaType.Sticker:
                                results.Add(new InlineQueryResultCachedSticker
                                {
                                    Id = offset.ToString(),
                                    StickerFileId = sauce.FileId,
                                    InputMessageContent = new InputTextMessageContent
                                    {
                                        MessageText = sauceText,
                                        ParseMode = ParseMode.HTML,
                                    },
                                    ReplyMarkup = keyboard
                                });
                                break;
                            case MediaType.Video:
                                results.Add(new InlineQueryResultCachedVideo
                                {
                                    Id = offset.ToString(),
                                    VideoFileId = sauce.FileId,
                                    Title = title,
                                    Description = description,
                                    Caption = sauceText,
                                    ParseMode = ParseMode.HTML,
                                    ReplyMarkup = keyboard
                                });
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(inlineQuery.Query))
                {
                    var mySauces = myHistory.Skip(offset).Take(10);
                    AddUserSauces(mySauces);
                }
                else
                {
                    var args = inlineQuery.Query.Split(' ');
                    var mySauces = myHistory.AsEnumerable().Where(s => args.Any(a => s.Info.Contains(a, StringComparison.InvariantCultureIgnoreCase))).Skip(offset).Take(10);
                    AddUserSauces(mySauces);
                }
            }
            else
            {
                var text = MSG.HistoryNone(Language);
                results.Add(new InlineQueryResultArticle
                {
                    Title = text,
                    InputMessageContent = new InputTextMessageContent
                    {
                        MessageText = text
                    }
                });
            }

            var answeriquery = new AnswerInlineQueryArgs
            {
                InlineQueryId = inlineQuery.Id,
                NextOffset = results.Count < 10 ? string.Empty : offset.ToString(),
                IsPersonal = true,
                CacheTime = 480,
                Results = results.ToArray()
            };
            await Api.AnswerInlineQueryAsync(answeriquery, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
