// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using SauceNAO.Domain.Entities.SauceAggregate;

namespace SauceNAO.Domain.Entities.UserAggregate;

/// <summary>
/// Represents a user on Telegram.
/// </summary>
public class TelegramUser : LocalizableEntityBase
{
    /// <summary>
    /// Unique identifier for this user.
    /// </summary>
    public required long UserId { get; set; }

    /// <summary>
    /// User's first name.
    /// </summary>
    [MaxLength(128)]
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    [MaxLength(128)]
    public string? LastName { get; set; }

    /// <summary>
    /// User's username.
    /// </summary>
    [MaxLength(32)]
    public string? Username { get; set; }

    /// <summary>
    /// Indicates if the user prefers to always use their own language.
    /// </summary>
    public bool AlwaysUseOwnLanguage { get; set; }

    /// <summary>
    /// Indicates if the user has initiated a private chat with the bot.
    /// </summary>
    public bool PrivateChatStarted { get; set; }

    /// <summary>
    /// List of API keys associated with this user.
    /// </summary>
    public virtual ICollection<SauceApiKey> ApiKeys { get; set; } = [];

    /// <summary>
    /// List of search history entries associated with this user.
    /// </summary>
    public virtual ICollection<SearchRecord> SearchHistory { get; set; } = [];
}
