// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNao.Services
{
    public partial class SauceNaoBot : TelegramBotAsync
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override async Task OnUpdateAsync(Update update, CancellationToken cancellationToken)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            cUpdate = update;
            await base.OnUpdateAsync(update, cancellationToken);
        }
    }
}
