// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace SauceNAO.Domain.Entities.ChatAggregate;

/// <summary>
/// Represents a telegram group or channel.
/// </summary>
/// <param name="chatId">Unique identifier for this chat.</param>
/// <param name="title">Title of the group or channel.</param>
public class TelegramChat(long chatId, string title) : LocalizableEntityBase
{
    /// <summary>
    /// Unique identifier for this chat.
    /// </summary>
    public long ChatId { get; set; } = chatId;

    /// <summary>
    /// Title of the group or channel.
    /// </summary>
    [StringLength(256)]
    public string Title { get; set; } = title;

    /// <summary>
    /// Optional. Username of the group or channel.
    /// </summary>
    [StringLength(32)]
    public string? Username { get; set; }

    /// <summary>
    /// Anticheats of chat.
    /// </summary>
    public ICollection<AntiCheatRestriction> Restrictions { get; } = [];
}
