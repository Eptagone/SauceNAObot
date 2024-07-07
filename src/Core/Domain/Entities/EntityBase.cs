// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain.Entities;

/// <summary>
/// Defines a base class for entities.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Unique identifier of the entity.
    /// </summary>
    public int Id { get; set; }
}
