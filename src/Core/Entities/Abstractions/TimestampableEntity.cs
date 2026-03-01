// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Entities.Abstractions;

/// <summary>
/// Defines a base class for entities that have a creation and update date.
/// </summary>
public abstract class TimestampableEntity : EntityBase, ITimestampable
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
