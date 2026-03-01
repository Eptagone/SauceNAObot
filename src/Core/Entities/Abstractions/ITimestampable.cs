// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Entities.Abstractions;

/// <summary>
/// Defines properties for entities that have a creation and update date.
/// </summary>
public interface ITimestampable
{
    /// <summary>
    /// Date and time of creation.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Date and time of last update.
    /// </summary>
    DateTimeOffset UpdatedAt { get; set; }
}
