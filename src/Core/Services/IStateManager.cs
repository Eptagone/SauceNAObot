using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Provides methods to manage user stages
/// </summary>
public interface IStateManager
{
    /// <summary>
    /// Dispatches the given state and use the given cache key to store it
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <param name="message">Message instance</param>
    /// <param name="state">State instance</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DispatchAsync<TState>(Message message, TState state, CancellationToken cancellationToken)
        where TState : class;

    /// <summary>
    /// Retrieves the state associated with the given message if it exists
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <param name="message">Message instance</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    Task<TState?> GetAsync<TState>(Message message, CancellationToken cancellationToken)
        where TState : class;

    /// <summary>
    /// Removes the state associated with the given message
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <param name="message">Message instance</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    Task RemoveAsync<TState>(Message message, CancellationToken cancellationToken)
        where TState : class;

    /// <summary>
    /// Updates the state associated with the given message
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <param name="message">Message instance</param>
    /// <param name="state">State instance</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    Task UpdateAsync<TState>(Message message, TState state, CancellationToken cancellationToken)
        where TState : class;
}
