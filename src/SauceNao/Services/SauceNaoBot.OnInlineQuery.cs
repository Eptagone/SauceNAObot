// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNao.Data.Models;
using SauceNao.Enums;
using SauceNao.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.InlineMode;

namespace SauceNao.Services
{
    public partial class SauceNaoBot : TelegramBotAsync
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override async Task OnInlineQueryAsync(InlineQuery inlineQuery, CancellationToken cancellationToken)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            cUser = await GetUserDataAsync(inlineQuery.From).ConfigureAwait(false);
            cLang = new CultureInfo(cUser.Lang ?? "en");

            var offset = string.IsNullOrEmpty(inlineQuery.Offset) ? 0 : int.Parse(inlineQuery.Offset);
            var myHistory = DB.UserSauces.AsNoTracking().Where(h => h.UserId == cUser.Id).OrderByDescending(h => h.Date).Select(s => s.Sauce);
            var results = new List<InlineQueryResult>();
            if (myHistory.Any())
            {
                void AddUserSauces(IEnumerable<SuccessfulSauce> sauces)
                {
                    foreach (var sauce in sauces)
                    {
                        var title = "Noname";
                        if (!string.IsNullOrEmpty(sauce.Info))
                        {
                            var info = JsonSerializer.Deserialize<SauceInfo>(sauce.Info);
                            if (!string.IsNullOrEmpty(info.Title))
                            {
                                title = info.Title;
                            }
                        }
                        var description = sauce.Type;
                        var sauceText = sauce.GetInfo(cLang);
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

            var answeriquery = new AnswerInlineQueryArgs
            {
                InlineQueryId = inlineQuery.Id,
                NextOffset = results.Count < 10 ? string.Empty : offset.ToString(),
                IsPersonal = true,
                CacheTime = 120,
                Results = results.ToArray()
            };
            await Bot.AnswerInlineQueryAsync(answeriquery, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
