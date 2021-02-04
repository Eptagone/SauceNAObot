// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using SauceNAO.Models;
using System;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO
{
    public partial class SauceNAOBot
    {
        private async Task OnBotException(BotRequestException exp, Update update)
        {
            await DB
                .AddAsync(new Report(exp, update))
                .ConfigureAwait(false);
            await DB
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }
        private async Task OnException(Exception exp)
        {
            await DB
                .AddAsync(new Report(exp))
                .ConfigureAwait(false);
            await DB
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }

        private async Task OnException(Exception exp, Message message, string details)
        {
            await DB
                .AddAsync(new Report(exp, message, details))
                .ConfigureAwait(false);
            await DB
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }
    }
}
