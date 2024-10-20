// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Localization;
using System.Collections;
using System.Globalization;
using System.Resources;

namespace SauceNAO.Application;

/// <summary>
/// Accessor class for the bot messages in the application.
/// </summary>
/// <param name="languageCode">The language code to use for the messages.</param>
sealed class BotMessagesLocalizer(string? languageCode) : IStringLocalizer
{
    private static ResourceManager? resourceManager;

    /// <summary>
    /// Returns the cached ResourceManager instance used by this class.
    /// </summary>
    internal static ResourceManager ResourceManager
    {
        get
        {
            return resourceManager ??= new ResourceManager(
                "SauceNAO.Application.Resources.Messages",
                typeof(BotMessagesLocalizer).Assembly
            );
        }
    }

    private readonly CultureInfo culture = string.IsNullOrEmpty(languageCode)
        ? CultureInfo.InvariantCulture
        : new(languageCode);

    /// <inheritdoc />
    public LocalizedString this[string name]
    {
        get
        {
            var value = ResourceManager.GetString(name, this.culture);
            if (value is null)
            {
                return new(name, string.Empty, true);
            }

            return new(name, value);
        }
    }

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = ResourceManager.GetString(name, this.culture);
            if (value is null)
            {
                return new(name, string.Empty, true);
            }

            return new(name, string.Format(value, arguments));
        }
    }

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return ResourceManager
                .GetResourceSet(this.culture, true, includeParentCultures)
                ?.Cast<DictionaryEntry>()
                .Select(entry => new LocalizedString((string)entry.Key, (string)entry.Value!))
            ?? [];
    }
}
