using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SauceNAO.Application.Resources;

/// <inheritdoc />
class BetterStringLocalizerFactory(
    IOptions<LocalizationOptions> localizationOptions,
    ILoggerFactory loggerFactory
)
    : ResourceManagerStringLocalizerFactory(localizationOptions, loggerFactory),
        IBetterStringLocalizerFactory
{
    private readonly ILoggerFactory loggerFactory = loggerFactory;
    private readonly IResourceNamesCache resourceNamesCache = new ResourceNamesCache();

    protected override ResourceManagerStringLocalizer CreateResourceManagerStringLocalizer(
        Assembly assembly,
        string baseName
    )
    {
        return new BetterStringLocalizer(
            new ResourceManager(baseName, assembly),
            assembly,
            baseName,
            this.resourceNamesCache,
            this.loggerFactory.CreateLogger<BetterStringLocalizer>()
        );
    }
}
