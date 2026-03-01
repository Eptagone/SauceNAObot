using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Core.Services;

/// <summary>
/// Defines a method to handle incoming bot updates
/// </summary>
public interface IUpdateHandler
{
    /// <summary>
    /// Handles an incoming update
    /// </summary>
    /// <param name="update">The update</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>The task object representing the asynchronous operation</returns>
    Task HandleAsync(Update update, CancellationToken cancellationToken);
}
