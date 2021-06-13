// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.InlineMode;
using Telegram.BotAPI.Payments;

namespace SauceNao.Services
{
    public partial class SauceNaoBot : TelegramBotAsync
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override async Task OnCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
        protected override async Task OnChannelPostAsync(Message message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnChosenInlineResultAsync(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnEditedChannelPostAsync(Message message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnEditedMessageAsync(Message message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnPollAnswerAsync(PollAnswer pollAnswer, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnPollAsync(Poll poll, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnPreCheckoutQueryAsync(PreCheckoutQuery preCheckoutQuery, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnShippingQueryAsync(ShippingQuery shippingQuery, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnChatMemberAsync(ChatMemberUpdated chatMemberUpdated, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnMyChatMemberAsync(ChatMemberUpdated myChatMemberUpdated, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
