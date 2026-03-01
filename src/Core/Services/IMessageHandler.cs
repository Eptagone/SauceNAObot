using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Defines a method to handle incoming messages
/// </summary>
public interface IMessageHandler
{
    /// <summary>
    /// Handles an incoming message
    /// </summary>
    /// <param name="message">The incoming message</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
    /// <returns><see langword="true"/> if the message was handled</returns>
    Task<bool> TryHandleAsync(Message message, CancellationToken cancellationToken);
}
