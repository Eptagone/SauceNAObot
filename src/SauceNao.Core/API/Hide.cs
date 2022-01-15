// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace SauceNAO.Core.API
{
    /// <summary>
    /// This controls the hidden field of results based on result content info. All results still show up in the api, check the hidden field for whether the site would have shown the image file.
    /// </summary>
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name")]
    public enum Hide
    {
        /// <summary>
        /// Show all.
        /// </summary>
        ShowAll = 0,
        /// <summary>
        /// Hide expected explicit.
        /// </summary>
        HideExpectedExplicit = 1,
        /// <summary>
        /// Hide expected and suspected explicit.
        /// </summary>
        HideExpectedAndSuspectedExplicit = 2,
        /// <summary>Hide all but expected safe.</summary>
        HideAllButExpectedSafe = 3
    }
}