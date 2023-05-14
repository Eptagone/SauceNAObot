// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Abstractions;

public abstract class TelegramChat : ITelegramChat
{
	public TelegramChat()
	{
	}

	public TelegramChat(ITelegramChat chat)
	{
		this.Id = chat.Id;
		this.Title = chat.Title;
		this.Username = chat.Username;
		this.Description = chat.Description;
		this.InviteLink = chat.InviteLink;
		this.LinkedChatId = chat.LinkedChatId;
		this.Type = chat.Type;
	}

	public long Id { get; set; }
	[Required]
	[StringLength(128)]
	public string Title { get; set; } = null!;
	public string Type { get; set; } = null!;
	[StringLength(32)]
	public string? Username { get; set; }
	public string? Description { get; set; }
	public string? InviteLink { get; set; }
	public long? LinkedChatId { get; set; }
}
