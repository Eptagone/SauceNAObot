using SauceNAO.Core.Entities;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Models;

/// <summary>
/// Represents a media included in a message.
/// </summary>
/// <param name="Media">The media itself</param>
/// <param name="Message">The message sent by the user</param>
/// <param name="MediaMessage">The message containing the media in case it's not the same as the message sent by the user</param>
public record MediaTarget(MediaFile Media, Message Message, Message? MediaMessage = null)
{
    public Message Message { get; set; } = Message;
    public Message? MediaMessage { get; set; } = MediaMessage;
};
