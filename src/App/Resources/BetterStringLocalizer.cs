// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;

namespace SauceNAO.App.Resources;

/// <inheritdoc />
sealed class BetterStringLocalizer(
    ResourceManager resourceManager,
    Assembly resourceAssembly,
    string baseName,
    IResourceNamesCache resourceNamesCache,
    ILogger logger
)
    : ResourceManagerStringLocalizer(
        resourceManager,
        resourceAssembly,
        baseName,
        resourceNamesCache,
        logger
    ),
        IBetterStringLocalizer
{
    private readonly string baseName = baseName;
    private CultureInfo? culture;

    /// <inheritdoc />
    public override LocalizedString this[string name]
    {
        get
        {
            var value = this.GetStringSafely(name, this.culture);

            return new LocalizedString(
                name,
                value ?? name,
                resourceNotFound: value == null,
                searchedLocation: this.baseName
            );
        }
    }

    /// <inheritdoc />
    public override LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = this.GetStringSafely(name, this.culture);
            var value = string.Format(CultureInfo.CurrentCulture, format ?? name, arguments);

            return new LocalizedString(
                name,
                value,
                resourceNotFound: format == null,
                searchedLocation: this.baseName
            );
        }
    }

    /// <inheritdoc />
    public void ChangeCulture(CultureInfo? culture)
    {
        this.culture = culture;
    }

    /// <inheritdoc />
    public void ChangeCulture(string languageCode) =>
        this.ChangeCulture(new CultureInfo(languageCode));
}

class BetterStringLocalizer<TResourceSource>(IBetterStringLocalizerFactory factory)
    : IBetterStringLocalizer<TResourceSource>
{
    private readonly IBetterStringLocalizer localizer = (
        factory.Create(typeof(TResourceSource)) as IBetterStringLocalizer
    )!;

    public LocalizedString this[string name] => this.localizer[name];

    public LocalizedString this[string name, params object[] arguments] =>
        this.localizer[name, arguments];

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        this.localizer.GetAllStrings(includeParentCultures);

    /// <inheritdoc />
    public void ChangeCulture(CultureInfo? culture) => this.localizer.ChangeCulture(culture);

    /// <inheritdoc />
    public void ChangeCulture(string languageCode) => this.localizer.ChangeCulture(languageCode);
}
