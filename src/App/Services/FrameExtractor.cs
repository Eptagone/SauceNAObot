// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Diagnostics;
using Microsoft.Extensions.Options;
using SauceNAO.Core.Configuration;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Services;

namespace SauceNAO.App.Services;

/// <summary>
/// Provides functionality to extract frames from video files.
/// </summary>
/// <param name="options">The application options.</param>
sealed class FrameExtractor(IOptions<FilesConfiguration> options) : IFrameExtractor
{
    private readonly string ffmpegPath = options.Value.FFmpegPath ?? "ffmpeg";

    /// <inheritdoc/>
    public Task ExtractAsync(string input, string output, CancellationToken cancellationToken)
    {
        var command = $"-i \"{input}\" -frames:v 1 -q:v 2 -update 1 \"{output}\" -y";
        return this.RunFFmpeg(command, cancellationToken);
    }

    /// <summary>
    /// Executes an FFmpeg command with the specified arguments and captures the output log.
    /// </summary>
    /// <param name="arguments">Command-line arguments for FFmpeg.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the command executed successfully, false otherwise.</returns>
    async Task RunFFmpeg(string arguments, CancellationToken cancellationToken)
    {
        using Process convtask = new();
        convtask.StartInfo.FileName = this.ffmpegPath;
        convtask.StartInfo.Arguments = arguments;
        convtask.StartInfo.UseShellExecute = false;
        convtask.StartInfo.RedirectStandardOutput = false;
        convtask.StartInfo.RedirectStandardError = true;
        try
        {
            convtask.Start(); // start the FFmpeg process
            await convtask.WaitForExitAsync(cancellationToken);
        }
        catch
        {
            var log = convtask.StandardError.ReadToEnd();
            throw new FFmpegException(log);
        }
    }
}
