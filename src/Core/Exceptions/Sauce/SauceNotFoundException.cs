using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.Sauce;

/// <summary>
/// Thrown when no sauce was found
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
/// <param name="mediaUrl">A temporary link to the media</param>
public sealed class SauceNotFoundException(Message receivedMessage, string mediaUrl)
    : SauceException(receivedMessage)
{
    /// <summary>
    /// A temporary link to the media
    /// </summary>
    public readonly string MediaUrl = mediaUrl;
}
