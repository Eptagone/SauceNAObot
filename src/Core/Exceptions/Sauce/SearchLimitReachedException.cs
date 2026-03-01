// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Exceptions.Sauce;

/// <summary>
/// Thrown when the search limit has been reached for the user.
/// </summary>
/// <param name="innerException">The inner exception</param>
public sealed class SearchLimitReachedException(Exception innerException)
    : SauceNAOException(innerException: innerException) { }
