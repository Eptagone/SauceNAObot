using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Handles user state
/// </summary>
public interface IUserStateHandler
{
    /// <summary>
    /// Tries to continue the user state based on the message
    /// </summary>
    /// <param name="message">The message sent by the user</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the state was canceled, false otherwise</returns>
    Task<bool> TryContinueAsync(Message message, CancellationToken cancellationToken);
}
