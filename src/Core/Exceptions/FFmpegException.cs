// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Thrown when FFmpeg throws an exception
/// </summary>
/// <param name="message">The error message</param>
public class FFmpegException(string message) : Exception(message) { }
