using SauceNAO.Core.Models;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Represents a user state handler
/// </summary>
public interface IUserStateHandler
{
    /// <summary>
    /// Determines whether the specified state can be handled by this handler
    /// </summary>
    /// <param name="state">The target state</param>
    /// <returns>True if the state can be handled, otherwise false</returns>
    bool CanHandleState(UserState state);

    /// <summary>
    /// Continue the user state.
    /// </summary>
    /// <param name="message">The current message</param>
    /// <param name="state">The user state</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task ContinueAsync(Message message, UserState state, CancellationToken cancellationToken);
}
