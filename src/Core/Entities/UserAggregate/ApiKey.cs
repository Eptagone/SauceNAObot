// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using SauceNAO.Core.Entities.Abstractions;

namespace SauceNAO.Core.Entities.UserAggregate;

/// <summary>
/// Represents an API key used to access the SauceNAO API.
/// </summary>
/// <param name="name">Name of the API key. Used to identify the key from the user's list of keys.</param>
/// <param name="value">The API key itself.</param>
public class ApiKey(string name, string value) : EntityBase
{
    /// <summary>
    /// Name of the API key. Used to identify the key from the user's list of keys.
    /// </summary>
    [MaxLength(32)]
    public string Name { get; set; } = name;

    /// <summary>
    /// The API key itself.
    /// </summary>
    [MaxLength(40)]
    public string Value { get; set; } = value;

    /// <summary>
    /// True if the user granted authorization to the bot to use this API key for public searches.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// The user who owns this API key.
    /// </summary>
    public UserEntity Owner { get; set; } = default!;
}
