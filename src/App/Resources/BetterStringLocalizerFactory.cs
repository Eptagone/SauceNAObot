using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace SauceNAO.App.Resources;

/// <inheritdoc />
sealed class BetterStringLocalizerFactory(
    IOptions<LocalizationOptions> localizationOptions,
    ILoggerFactory loggerFactory
)
    : ResourceManagerStringLocalizerFactory(localizationOptions, loggerFactory),
        IBetterStringLocalizerFactory
{
    private readonly ILoggerFactory loggerFactory = loggerFactory;
    private readonly IResourceNamesCache resourceNamesCache = new ResourceNamesCache();
    private readonly ICollection<IBetterStringLocalizer> createdLocalizes = [];
    private CultureInfo? culture;

    public void ChangeCulture(string languageCode)
    {
        this.culture = new CultureInfo(languageCode);
        foreach (var localizer in this.createdLocalizes)
        {
            localizer.ChangeCulture(this.culture);
        }
    }

    protected override ResourceManagerStringLocalizer CreateResourceManagerStringLocalizer(
        Assembly assembly,
        string baseName
    )
    {
        var localizer = new BetterStringLocalizer(
            new ResourceManager(baseName, assembly),
            assembly,
            baseName,
            this.resourceNamesCache,
            this.loggerFactory.CreateLogger<BetterStringLocalizer>()
        );

        if (this.culture is not null)
        {
            localizer.ChangeCulture(this.culture);
        }
        this.createdLocalizes.Add(localizer);
        return localizer;
    }
}
