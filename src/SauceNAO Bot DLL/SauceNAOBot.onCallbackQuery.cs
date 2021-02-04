// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System;
using SauceNAO.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.UpdatingMessages;
using Telegram.BotAPI.InlineMode;
using SauceNAO.Resources;

namespace SauceNAO
{
    public partial class SauceNAOBot
    {
        /// <summary>Process a Callback Query</summary>
        /// <param name="query">Callback Query</param>
        private async Task OnCallbackQuery(CallbackQuery query)
        {
            user = await GetUserData(query.From);
            lang = new CultureInfo(string.IsNullOrEmpty(user.Lang) ? "en" : user.Lang);
            var keywords = query.Data.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            switch (keywords[0])
            {
                case "clear":
                    switch (keywords[1])
                    {
                        case "yes":
                            await Bot.AnswerCallbackQueryAsync(query.Id, MSG.HistoryErased(lang), true, cacheTime: 30).ConfigureAwait(false);
                            await Bot.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId).ConfigureAwait(false);
                            break;
                        case "no":
                            await Bot.AnswerCallbackQueryAsync(query.Id, MSG.Cancelled(lang), true, cacheTime: 30).ConfigureAwait(false);
                            await Bot.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId).ConfigureAwait(false);
                            break;
                    }
                    break;
            }
        }
    }
}
