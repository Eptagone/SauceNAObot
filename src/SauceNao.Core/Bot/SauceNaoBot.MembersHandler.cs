// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core
{
    public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
    {
        protected override async Task OnMyChatMemberAsync(ChatMemberUpdated myChatMemberUpdated, CancellationToken cancellationToken)
        {
            var member = myChatMemberUpdated.NewChatMember;
            Group = await _db.Groups.GetGroupAsync(myChatMemberUpdated.Chat, cancellationToken).ConfigureAwait(false);

            async Task DeleteGroupAsync()
            {
                await _db.Groups.DeleteAsync(Group, cancellationToken).ConfigureAwait(false);
            }

            if (member is ChatMemberBanned)
            {
#if DEBUG
                _logger.LogInformation("The bot was banned from the group \"{group_title}\" [{chat_id}]. Group data will be removed from database.", Group.Title, Group.Id);
#endif
                await DeleteGroupAsync().ConfigureAwait(false);
            }
            else if (member is ChatMemberRestricted)
            {
#if DEBUG
                _logger.LogInformation("The bot was restricted from the group \"{group_title}\" [{chat_id}]. Bot will leave chat and all group data will be removed from database.", Group.Title, Group.Id);
#endif
                await DeleteGroupAsync().ConfigureAwait(false);
                await Api.LeaveChatAsync(myChatMemberUpdated.Chat.Id, cancellationToken).ConfigureAwait(false);
            }
        }

        protected override async Task OnChatMemberAsync(ChatMemberUpdated chatMemberUpdated, CancellationToken cancellationToken)
        {
            var member = chatMemberUpdated.NewChatMember;
            if (member.User.IsBot)
            {
                if (member is ChatMemberLeft or ChatMemberBanned)
                {
                    Group = await _db.Groups.GetGroupAsync(chatMemberUpdated.Chat, cancellationToken).ConfigureAwait(false);
                    var anticheat = Group.AntiCheats.FirstOrDefault(a => a.BotId == member.User.Id);
                    if (anticheat != default)
                    {
                        Group.AntiCheats.Remove(anticheat);
                        await _db.Groups.UpdateAsync(Group, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
