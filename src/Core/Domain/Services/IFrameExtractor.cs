// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain.Services;

/// <summary>
/// Provides method to extract the first frame of a video or animated image.
/// </summary>
public interface IFrameExtractor
{
    /// <summary>
    /// Extract the first frame of a video or animated image.
    /// </summary>
    /// <param name="input">The input file path.</param>
    /// <param name="output">The output file path.</param>
    /// <returns>True if the frame was extracted successfully; otherwise, false.</returns>
    Task<bool> ExtractAsync(string input, string output, CancellationToken cancellationToken);
}
