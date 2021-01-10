// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using SauceNAO.Models;
using System.Globalization;

namespace SauceNAO
{
    public partial class SauceNAOBot
    {
        /// <summary>Current user instance.</summary>
        private AppUser user;
        /// <summary>Current chat instance.</summary>
        private AppChat chat;
        /// <summary>Current date instance.</summary>
        private uint date;
        /// <summary>Current lang instance.</summary>
        private CultureInfo lang;
    }
}
