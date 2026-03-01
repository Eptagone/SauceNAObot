namespace SauceNAO.Core.Configuration;

/// <summary>
/// Defines general options for handling files.
/// </summary>
public sealed record FilesConfiguration
{
    /// <summary>
    /// The section name in the configuration.
    /// </summary>
    public static string SectionName => "Files";

    /// <summary>
    /// Path to the FFmpeg executable.
    /// </summary>
    public string? FFmpegPath { get; set; }
}
