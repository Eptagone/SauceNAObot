using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Represents a message handler.
/// </summary>
public interface IMessageHandler
{
    /// <summary>
    /// Handles an incoming message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task<bool> HandleAsync(Message message, CancellationToken cancellationToken);
}
