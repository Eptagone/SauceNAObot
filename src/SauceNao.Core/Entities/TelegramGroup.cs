// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Entities;

/// <summary>
/// Telegram Group
/// </summary>
[Table("Group", Schema = "tg")]
public partial class TelegramGroup : TelegramChat
{
	/// <summary>
	/// Initialize a new instance of AppChat
	/// </summary>
	public TelegramGroup()
	{
		this.Title = string.Empty;
		this.Type = string.Empty;
		this.AntiCheats = new HashSet<AntiCheat>();
	}

	public TelegramGroup(ITelegramChat chat)
	{
		if (chat == default)
		{
			throw new ArgumentNullException(nameof(chat));
		}
		this.Id = chat.Id;
		this.Title = chat.Title ?? "Chat desconocido";
		this.Description = chat.Description ?? string.Empty;
		this.Username = chat.Username ?? string.Empty;
		this.InviteLink = chat.InviteLink ?? (string.IsNullOrEmpty(this.Username) ? string.Empty : $"https://t.me/{this.Username}");
		this.Type = chat.Type;

		this.AntiCheats = new HashSet<AntiCheat>();
	}

	/// <summary>
	/// The AppChat Id.
	/// </summary>
	[Key]
	public int Key { get; set; }

	/// <summary>
	/// Chat Language Code.
	/// </summary>
	[StringLength(8)]
	public string? LanguageCode { get; set; }

	/// <summary>
	/// Anticheats of chat.
	/// </summary>
	[InverseProperty(nameof(AntiCheat.Group))]
	public virtual ICollection<AntiCheat> AntiCheats { get; set; }
}

