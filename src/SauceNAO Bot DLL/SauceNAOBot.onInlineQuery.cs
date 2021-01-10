// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.BotAPI.Available_Types;
using Telegram.BotAPI.Inline_mode;

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
                                    Photo_file_id = sauce.OriginalFile,
                                    Title = title,
                                    Description = description,
                                    Caption = content,
                                    Parse_mode = ParseMode.HTML,
                                    Reply_markup = new InlineKeyboardMarkup
                                    {
                                        Inline_keyboard = buttons
                                    }
                                });
                                break;
                            case "sticker":
                                results.Add(new InlineQueryResultCachedSticker
                                {
                                    Id = offset.ToString(),
                                    Sticker_file_id = sauce.OriginalFile,
                                    Input_message_content = new InputTextMessageContent
                                    {
                                        Message_text = content,
                                        Parse_mode = ParseMode.HTML,
                                    },
                                    Reply_markup = new InlineKeyboardMarkup
                                    {
                                        Inline_keyboard = buttons
                                    }
                                });
                                break;
                            case "animation":
                                results.Add(new InlineQueryResultCachedGif
                                {
                                    Id = offset.ToString(),
                                    Gif_file_id = sauce.OriginalFile,
                                    Title = title,
                                    Caption = content,
                                    Parse_mode = ParseMode.HTML,
                                    Reply_markup = new InlineKeyboardMarkup
                                    {
                                        Inline_keyboard = buttons
                                    }
                                });
                                break;
                            case "video":
                                results.Add(new InlineQueryResultCachedVideo
                                {
                                    Id = offset.ToString(),
                                    Video_file_id = sauce.OriginalFile,
                                    Title = title,
                                    Description = description,
                                    Caption = content,
                                    Parse_mode = ParseMode.HTML,
                                    Reply_markup = new InlineKeyboardMarkup
                                    {
                                        Inline_keyboard = buttons
                                    }
                                });
                                break;
                            case "document":
                                results.Add(new InlineQueryResultCachedDocument
                                {
                                    Id = offset.ToString(),
                                    Document_file_id = sauce.OriginalFile,
                                    Title = title,
                                    Description = description,
                                    Caption = content,
                                    Parse_mode = ParseMode.HTML,
                                    Reply_markup = new InlineKeyboardMarkup
                                    {
                                        Inline_keyboard = buttons
                                    }
                                });
                                break;
                        }
                    }
                }
            }
            var answeriquery = new AnswerInlineQueryArgs
            {
                Inline_query_id = iquery.Id,
                Next_offset = results.Count() < 8 ? string.Empty : offset.ToString(),
                Is_personal = true,
                Cache_time = 30,
                Results = results.ToArray()
            };
            await Bot.AnswerInlineQueryAsync(answeriquery).ConfigureAwait(false);
        }
    }
}
