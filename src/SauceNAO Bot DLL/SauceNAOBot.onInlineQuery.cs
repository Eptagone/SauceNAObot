// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.InlineMode;

namespace SauceNAO
{
    public partial class SauceNAOBot
    {
        /// <summary>Process a Inline query</summary>
        /// <param name="iquery">Inline Query</param>
        /// <param name="DB">DB Context</param>
        private async Task OnInlineQuery(InlineQuery iquery)
        {
            user = await GetUserData(iquery.From).ConfigureAwait(false);
            lang = new CultureInfo(string.IsNullOrEmpty(user.Lang) ? "en" : user.Lang);
            var offset = string.IsNullOrEmpty(iquery.Offset) ? 0 : int.Parse(iquery.Offset);
            var history = DB.History.Where(h => h.Id == user.Id).OrderByDescending(h => h.Date).Skip(offset).Take(8);
            var results = new List<InlineQueryResult>();
            if (history.Any())
            {
                foreach (var h in history)
                {
                    var sauce = DB.Searches.FirstOrDefault(s => s.File == h.SauceFile);
                    if (sauce == default)
                    {
                        DB.Remove(h);
                    }
                    else
                    {
                        offset++;
                        var title = string.IsNullOrEmpty(sauce.Title) ? "Noname" : sauce.Title;
                        var description = sauce.Type;
                        var content = sauce.ReadInfo(lang);
                        var buttons = Utilities.ButtonsFromUrls(sauce.Data);
                        switch (sauce.Type)
                        {
                            case "photo":
                                results.Add(new InlineQueryResultCachedPhoto
                                {
                                    Id = offset.ToString(),
                                    PhotoFileId = sauce.OriginalFile,
                                    Title = title,
                                    Description = description,
                                    Caption = content,
                                    ParseMode = ParseMode.HTML,
                                    ReplyMarkup = new InlineKeyboardMarkup
                                    {
                                        InlineKeyboard = buttons
                                    }
                                });
                                break;
                            case "sticker":
                                results.Add(new InlineQueryResultCachedSticker
                                {
                                    Id = offset.ToString(),
                                    StickerFileId = sauce.OriginalFile,
                                    InputMessageContent = new InputTextMessageContent
                                    {
                                        MessageText = content,
                                        ParseMode = ParseMode.HTML,
                                    },
                                    ReplyMarkup = new InlineKeyboardMarkup
                                    {
                                        InlineKeyboard = buttons
                                    }
                                });
                                break;
                            case "animation":
                                results.Add(new InlineQueryResultCachedGif
                                {
                                    Id = offset.ToString(),
                                    GifFileId = sauce.OriginalFile,
                                    Title = title,
                                    Caption = content,
                                    ParseMode = ParseMode.HTML,
                                    ReplyMarkup = new InlineKeyboardMarkup
                                    {
                                        InlineKeyboard = buttons
                                    }
                                });
                                break;
                            case "video":
                                results.Add(new InlineQueryResultCachedVideo
                                {
                                    Id = offset.ToString(),
                                    VideoFileId = sauce.OriginalFile,
                                    Title = title,
                                    Description = description,
                                    Caption = content,
                                    ParseMode = ParseMode.HTML,
                                    ReplyMarkup = new InlineKeyboardMarkup
                                    {
                                        InlineKeyboard = buttons
                                    }
                                });
                                break;
                            case "document":
                                results.Add(new InlineQueryResultCachedDocument
                                {
                                    Id = offset.ToString(),
                                    DocumentFileId = sauce.OriginalFile,
                                    Title = title,
                                    Description = description,
                                    Caption = content,
                                    ParseMode = ParseMode.HTML,
                                    ReplyMarkup = new InlineKeyboardMarkup
                                    {
                                        InlineKeyboard = buttons
                                    }
                                });
                                break;
                        }
                    }
                }
            }
            var answeriquery = new AnswerInlineQueryArgs
            {
                InlineQueryId = iquery.Id,
                NextOffset = results.Count < 8 ? string.Empty : offset.ToString(),
                IsPersonal = true,
                CacheTime = 30,
                Results = results.ToArray()
            };
            await Bot.AnswerInlineQueryAsync(answeriquery).ConfigureAwait(false);
        }
    }
}
