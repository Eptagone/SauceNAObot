// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core;

public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
{
	protected override async Task OnMyChatMemberAsync(ChatMemberUpdated myChatMemberUpdated, CancellationToken cancellationToken)
	{
		var member = myChatMemberUpdated.NewChatMember;
		this.Group = await this._db.Groups.GetGroupAsync(myChatMemberUpdated.Chat, cancellationToken).ConfigureAwait(false);

		async Task DeleteGroupAsync()
		{
			await this._db.Groups.DeleteAsync(this.Group, cancellationToken).ConfigureAwait(false);
		}

		if (member is ChatMemberBanned)
		{
#if DEBUG
			this._logger.LogInformation("The bot was banned from the group \"{group_title}\" [{chat_id}]. Group data will be removed from database.", this.Group.Title, this.Group.Id);
#endif
			await DeleteGroupAsync().ConfigureAwait(false);
		}
		else if (member is ChatMemberRestricted)
		{
#if DEBUG
			this._logger.LogInformation("The bot was restricted from the group \"{group_title}\" [{chat_id}]. Bot will leave chat and all group data will be removed from database.", this.Group.Title, this.Group.Id);
#endif
			await DeleteGroupAsync().ConfigureAwait(false);
			await this.Api.LeaveChatAsync(myChatMemberUpdated.Chat.Id, cancellationToken).ConfigureAwait(false);
		}
	}

	protected override async Task OnChatMemberAsync(ChatMemberUpdated chatMemberUpdated, CancellationToken cancellationToken)
	{
		var member = chatMemberUpdated.NewChatMember;
		if (member.User.IsBot)
		{
			if (member is ChatMemberLeft or ChatMemberBanned)
			{
				this.Group = await this._db.Groups.GetGroupAsync(chatMemberUpdated.Chat, cancellationToken).ConfigureAwait(false);
				var anticheat = this.Group.AntiCheats.FirstOrDefault(a => a.BotId == member.User.Id);
				if (anticheat != default)
				{
					this.Group.AntiCheats.Remove(anticheat);
					await this._db.Groups.UpdateAsync(this.Group, cancellationToken).ConfigureAwait(false);
				}
			}
		}
	}
}
