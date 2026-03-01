namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Thrown when FFmpeg throws an exception
/// </summary>
/// <param name="message">The error message</param>
public class FFmpegException(string message) : Exception(message) { }
