using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Thrown when the bot is unable to download a file from Telegram
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
/// <param name="downloadUrl">The url used to download the file</param>
/// <param name="innerException">The inner exception, if any</param>
public sealed class DownloadFailedException(
    Message receivedMessage,
    string downloadUrl,
    Exception innerException
) : MessageException(receivedMessage, innerException)
{
    /// <summary>
    /// The url used to download the file
    /// </summary>
    public string DownloadUrl { get; } = downloadUrl;
}
