// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities.SauceAggregate;

namespace SauceNAO.Core.Entities.UserAggregate;

/// <summary>
/// Represents a single entry in a user's search history.
/// </summary>
public class SearchRecord : EntityBase
{
    /// <summary>
    /// The date the search was made.
    /// </summary>
    public DateTimeOffset SearchedAt { get; set; }

    /// <summary>
    /// The similarity of the search.
    /// </summary>
    public float Similarity { get; set; }

    /// <summary>
    /// The user who made the search.
    /// </summary>
    public TelegramUser User { get; set; } = null!;

    /// <summary>
    /// The media that was searched for.
    /// </summary>
    public required SauceMedia Media { get; set; }
}
