// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SauceNAO.Domain;
using SauceNAO.Domain.Services;

namespace SauceNAO.Application.Services;

/// <summary>
/// Provides functionality to extract frames from video files.
/// </summary>
/// <param name="logger">The Frame Extractor logger.</param>
/// <param name="options">The application options.</param>
class FrameExtractor(ILogger<FrameExtractor> logger, IOptions<GeneralOptions> options)
    : IFrameExtractor
{
    private readonly ILogger<FrameExtractor> logger = logger;
    private readonly string ffmpegPath = options.Value.FFmpegPath ?? "ffmpeg";

    /// <inheritdoc/>
    public Task<bool> ExtractAsync(string input, string output, CancellationToken cancellationToken)
    {
        // Check if the output file already exists.
        if (File.Exists(output))
        {
            return Task.FromResult(true);
        }

        var command = $"-i \"{input}\" -vf \"select=eq(n\\,0)\" -frames:v 1 \"{output}\" -y";

        this.logger.LogInformation(command);
        return this.RunFFmpeg(command, cancellationToken);
    }

    /// <summary>
    /// Executes an FFmpeg command with the specified arguments and captures the output log.
    /// </summary>
    /// <param name="arguments">Command-line arguments for FFmpeg.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the command executed successfully, false otherwise.</returns>
    async Task<bool> RunFFmpeg(string arguments, CancellationToken cancellationToken)
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
            this.logger.LogFFmpegError(log);
            return false; // return false if an exception occurred
        }
        return true; // return true if the command executed successfully
    }
}
