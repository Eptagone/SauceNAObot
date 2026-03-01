// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Models;
using Telegram.BotAPI;

namespace SauceNAO.Core.Services;

/// <summary>
/// Provides methods to generate download urls for media.
/// </summary>
public interface IMediaUrlGenerator
{
    /// <summary>
    /// Generates a download url for the given media.
    /// </summary>
    /// <param name="target">Target media</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>The url to the file</returns>
    /// <exception cref="BotRequestException">Thrown when the request fails</exception>
    Task<string> GenerateAsync(MediaTarget target, CancellationToken cancellationToken);

    /// <summary>
    /// Generates a safe to share download url for the given media.
    /// </summary>
    /// <param name="target">Target media</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>The url to the file</returns>
    /// <exception cref="BotRequestException">Thrown when the request fails</exception>
    Task<string> GenerateSafeAsync(MediaTarget target, CancellationToken cancellationToken);
}
