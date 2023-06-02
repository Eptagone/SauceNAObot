// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Entities;

/// <summary>
/// Application User Model
/// </summary>
[Table("User", Schema = "tg")]
public partial class UserData : TelegramUser
{
	/// <summary>
	/// Initialize a new instance of AppUser
	/// </summary>
	public UserData() : base()
	{
		this.AntiCheats = new HashSet<AntiCheat>();
		this.UserSauces = new HashSet<UserSauce>();
	}

	public UserData(ITelegramUser user) : base(user)
	{
		this.AntiCheats = new HashSet<AntiCheat>();
		this.UserSauces = new HashSet<UserSauce>();
	}

	/// <summary>
	/// True, if the user prefers to use their own language in all chats.
	/// </summary>
	public bool LangForce { get; set; }

	/// <summary>
	/// True, if the user started a private chat with SauceNao bot.
	/// </summary>
	public bool Start { get; set; }

	/// <summary>
	/// Private user's apikey
	/// </summary>
	public string? ApiKey { get; set; }

	/// <summary>
	/// AntiCheats added by user.
	/// </summary>
	[InverseProperty(nameof(AntiCheat.AddedByUser))]
	public virtual ICollection<AntiCheat> AntiCheats { get; set; }

	/// <summary>
	/// Sauces searched by user.
	/// </summary>
	[InverseProperty(nameof(UserSauce.User))]
	public virtual ICollection<UserSauce> UserSauces { get; set; }
}
