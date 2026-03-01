// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Exceptions.Sauce;

/// <summary>
/// Thrown when the api key is invalid or expired
/// </summary>
/// <param name="innerException"></param>
public sealed class InvalidApiKeyException(Exception? innerException = null)
    : SauceNAOException(innerException: innerException) { }
