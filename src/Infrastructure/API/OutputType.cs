// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Infrastructure.API;

/// <summary>
/// 0=normal html 1=xml api(not implemented) 2=json api
/// </summary>
internal enum OutputType
{
    /// <summary>Normal HTML.</summary>
    NormalHtml = 0,

    /// <summary>XML Api.</summary>
    XmlApi = 1,

    /// <summary>Json Api.</summary>
    JsonApi = 2,
}
