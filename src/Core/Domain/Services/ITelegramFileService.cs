// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain.Services;

/// <summary>
/// Defines a methods to get temporary access to files stored in Telegram.
/// </summary>
public interface ITelegramFileService
{
    /// <summary>
    /// Gets the URL of a file.
    /// </summary>
    /// <param name="fileId">Identifier for the file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the URL of the file or <see langword="null"/> if the file could not be found.</returns>
    Task<string?> GetFileUrlAsync(string fileId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the URL of a file.
    /// </summary>
    /// <param name="fileId">Identifier for the file.</param>
    /// <param name="publicAccess">Whether the URL should be public-safe URL which can be shared with others users.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the URL of the file or <see langword="null"/> if the file could not be found.</returns>
    Task<string?> GetFileUrlAsync(
        string fileId,
        bool publicAccess,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Gets a local path to the file.
    /// </summary>
    /// <remarks>If the file is not stored locally, it will be downloaded.</remarks>
    /// <param name="fileId">Identifier for the file.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the local path to the file or <see langword="null"/> if the file could not be found.</returns>
    Task<string?> GetFilePathAsync(string fileId, CancellationToken cancellationToken);
}
