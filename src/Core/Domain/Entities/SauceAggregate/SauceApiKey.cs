// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Entities.UserAggregate;

namespace SauceNAO.Domain.Entities.SauceAggregate;

/// <summary>
/// Represents an API key used to access the SauceNAO API.
/// </summary>
/// <param name="name">Name of the API key. Used to identify the key from the user's list of keys.</param>
/// <param name="value">The API key itself.</param>
public class SauceApiKey(string name, string value) : EntityBase
{
    /// <summary>
    /// Name of the API key. Used to identify the key from the user's list of keys.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// The API key itself.
    /// </summary>
    public string Value { get; set; } = value;

    /// <summary>
    /// Date and time of last update.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// True if the user granted authorization to the bot to use this API key for public searches.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// The user who owns this API key.
    /// </summary>
    public virtual TelegramUser Owner { get; set; } = default!;
}
