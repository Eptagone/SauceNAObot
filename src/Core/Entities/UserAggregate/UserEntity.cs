// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using SauceNAO.Core.Entities.Abstractions;

namespace SauceNAO.Core.Entities.UserAggregate;

/// <summary>
/// Represents a user on Telegram.
/// </summary>
public class UserEntity(long userId, string firstName) : LocalizableEntity
{
    /// <summary>
    /// Unique identifier for this user.
    /// </summary>
    public long UserId { get; set; } = userId;

    /// <summary>
    /// User's first name.
    /// </summary>
    [MaxLength(128)]
    public string FirstName { get; set; } = firstName;

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

    /// <inheritdoc />
    [StringLength(8)]
    public string? LanguageCode { get; set; }

    /// <summary>
    /// Indicates if the user specified a fixed language code.
    /// If true, the language code won't be updated automatically.
    /// </summary>
    public bool UseFixedLanguage { get; set; }

    /// <summary>
    /// Indicates if the user has initiated a private chat with the bot.
    /// </summary>
    public bool HasStartedDm { get; set; }

    /// <summary>
    /// List of API keys associated with this user.
    /// </summary>
    public ICollection<ApiKey> ApiKeys { get; set; } = [];

    /// <summary>
    /// List of search history entries associated with this user.
    /// </summary>
    public ICollection<SearchRecord> SearchHistory { get; set; } = [];
}
