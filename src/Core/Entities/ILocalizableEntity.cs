namespace SauceNAO.Core.Entities;

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
