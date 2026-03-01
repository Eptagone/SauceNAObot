// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

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
