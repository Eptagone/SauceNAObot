using System.Globalization;
using Microsoft.Extensions.Localization;

namespace SauceNAO.Application.Resources;

/// <inheritdoc />
public interface IBetterStringLocalizer : IStringLocalizer
{
    /// <summary>
    /// Change the culture of the localizer
    /// </summary>
    /// <param name="culture">The new culture</param>
    /// <returns></returns>
    void ChangeCulture(CultureInfo? culture);

    /// <summary>
    /// Change the culture of the localizer
    /// </summary>
    /// <param name="languageCode">The new language code</param>
    /// <returns></returns>
    void ChangeCulture(string languageCode);
}

/// <inheritdoc />
public interface IBetterStringLocalizer<out T> : IStringLocalizer<T>, IBetterStringLocalizer { }
