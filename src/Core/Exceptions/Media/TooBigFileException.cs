using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.Media;

/// <summary>
/// Thrown when the bot is unable to process a request because the received file is too big
/// </summary>
/// <param name="message">The message sent by the user that triggered the exception</param>
public sealed class TooBigFileException(Message message) : MessageMediaException(message) { }
