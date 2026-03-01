// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities.Abstractions;

namespace SauceNAO.Core.Entities.UserAggregate;

/// <summary>
/// Represents a single entry in a user's search history.
/// </summary>
public class SearchRecord(float similarity) : TimestampableEntity
{
    /// <summary>
    /// The similarity of the search.
    /// </summary>
    public float Similarity { get; set; } = similarity;

    /// <summary>
    /// The user who made the search.
    /// </summary>
    public UserEntity User { get; set; } = null!;

    /// <summary>
    /// The media that was searched for.
    /// </summary>
    public required MediaFile Media { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; set; }
}
