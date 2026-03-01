using Microsoft.Extensions.Localization;

namespace SauceNAO.App.Resources;

/// <inheritdoc />
interface IBetterStringLocalizerFactory : IStringLocalizerFactory
{
    /// <summary>
    /// Change the culture of created localizers
    /// </summary>
    /// <param name="languageCode">The new language code</param>
    /// <returns>void</returns>
    void ChangeCulture(string languageCode);
}
