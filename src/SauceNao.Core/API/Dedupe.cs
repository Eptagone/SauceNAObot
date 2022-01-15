// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.API
{
    /// <summary>0=no result deduping 1=consolidate booru results and dedupe by item identifier 2=all implemented dedupe methods such as by series name. Default is 2, more levels may be added in future.</summary>
    public enum Dedupe
    {
        /// <summary>No result deduping.</summary>
        NoResultDeduping = 0,
        /// <summary>Consolidate booru results and dedupe by item identifier.</summary>
        ConsolidateBooruResultsAndDedupeByItemIdentifier = 1,
        /// <summary>All implemented dedupe methods such as by series name.</summary>
        AllImplementedDedupeMethodsSuchAsBySeriesName = 2
    }
}
