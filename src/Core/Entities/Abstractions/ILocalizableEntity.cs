// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Entities.Abstractions;

/// <summary>
/// Defines a language code for an entity.
/// </summary>
interface ILocalizableEntity
{
    /// <summary>
    /// A two-letter ISO 639-1 language code.
    /// </summary>
    public string? LanguageCode { get; set; }
}
