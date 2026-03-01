using System.ComponentModel.DataAnnotations;

namespace SauceNAO.Core.Entities;

/// <summary>
/// Defines a base class for entities that define a language code.
/// </summary>
public abstract class LocalizableEntityBase : EntityBase, ILocalizableEntity
{
    /// <inheritdoc />
    [StringLength(8)]
    public string? LanguageCode { get; set; }
}
