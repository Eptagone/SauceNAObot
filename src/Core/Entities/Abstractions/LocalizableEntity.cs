// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace SauceNAO.Core.Entities.Abstractions;

/// <summary>
/// Defines a base class for entities that define a language code.
/// </summary>
public abstract class LocalizableEntity : TimestampableEntity, ILocalizableEntity
{
    /// <inheritdoc />
    [StringLength(8)]
    public string? LanguageCode { get; set; }
}
