﻿// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Entities;

/// <summary>
/// AntiCheat model
/// </summary>
[Table("AntiCheat", Schema = "tg")]
public partial class AntiCheat
{
	/// <summary>
	/// Initialize a new instance of AntiCheats
	/// </summary>
	public AntiCheat() { }

	/// <summary>
	/// Initializes a new instance of AntiCheats with the specified user and userId.
	/// </summary>
	/// <param name="user">The telegram user.</param>
	/// <param name="userId">The user Id who added the AntiCheats registry.</param>
	internal AntiCheat(ITelegramUser user, long userId)
	{
		if (!user.IsBot)
		{
			throw new ArgumentException("The telegram user must be a bot.");
		}

		this.BotId = user.Id;
		this.AddedByUserId = userId;
	}

	/// <summary>
	/// The AntiCheat Primary Key
	/// </summary>
	[Key]
	public int Key { get; set; }

	/// <summary>
	/// The Group Key.
	/// </summary>
	[Required]
	public int ChatKey { get; set; }

	/// <summary>
	/// Optional. Unique identifier of the bot that was added to the AntiCheats registry.
	/// </summary>
	[Required]
	public long BotId { get; set; }

	/// <summary>
	/// The user Id who added the AntiCheats registry.
	/// </summary>
	[Required]
	public long AddedByUserId { get; set; }

	/// <summary>User who added the AntiCheats registry.</summary>
	[ForeignKey(nameof(AddedByUserId))]
	[InverseProperty(nameof(UserData.AntiCheats))]
	public virtual UserData AddedByUser { get; set; }

	/// <summary>User who was added to the AntiCheats registry.</summary>
	[ForeignKey(nameof(ChatKey))]
	[InverseProperty(nameof(TelegramGroup.AntiCheats))]
	public virtual TelegramGroup Group { get; set; }
}
