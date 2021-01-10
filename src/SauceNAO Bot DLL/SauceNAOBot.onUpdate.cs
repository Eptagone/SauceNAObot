// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.Getting_updates;

namespace SauceNAO
{
    public partial class SauceNAOBot
    {
        /// <summary>Get and process updates</summary>
        /// <param name="update">Update</param>
        public async Task OnUpdateAsync(Update update)
        {
            try
            {
                if (update == default)
                {
                    throw new ArgumentNullException(nameof(update));
                }
                // Switch by update type
                switch (update.Type)
                {
                    // Message Update
                    case UpdateType.Message:
                        await OnMessage(update.Message).ConfigureAwait(false);
                        break;
                    // Inline Query Update
                    case UpdateType.Inline_query:
                        await OnInlineQuery(update.Inline_query).ConfigureAwait(false);
                        break;
                    // Callback Query Update
                    case UpdateType.Callback_query:
                        await OnCallbackQuery(update.Callback_query).ConfigureAwait(false);
                        break;
                }
            }
            catch (BotRequestException exp)
            {
                await OnBotException(exp, update).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException exp)
            {
                await OnException(exp).ConfigureAwait(false);
            }
            catch (DbUpdateException exp)
            {
                await OnException(exp).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                await OnException(exp).ConfigureAwait(false);
            }
        }
    }
}
