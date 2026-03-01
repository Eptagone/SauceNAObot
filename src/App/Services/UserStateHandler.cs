// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Services;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.App.Services;

/// <summary>
/// Base class for user state handlers
/// </summary>
/// <typeparam name="TState">State type</typeparam>
/// <param name="stateManager">The state manager</param>
abstract class UserStateHandler<TState>(IStateManager stateManager) : IUserStateHandler
    where TState : class
{
    public async Task<bool> TryContinueAsync(Message message, CancellationToken cancellationToken)
    {
        var state = await stateManager.GetAsync<TState>(message, cancellationToken);
        if (state is null)
        {
            return false;
        }

        if (message.Text == "/cancel")
        {
            await stateManager.RemoveAsync<TState>(message, cancellationToken);
            throw new UserStateCancelledException(message);
        }
        else if (message.Text?.StartsWith('/') is true)
        {
            throw new UserStateDisallowedCommandException(message);
        }

        await this.ContinueAsync(state, message, cancellationToken);
        return true;
    }

    /// <summary>
    /// Handles the user state
    /// </summary>
    /// <param name="state">The user state</param>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    protected abstract Task ContinueAsync(
        TState state,
        Message message,
        CancellationToken cancellationToken
    );

    protected Task SaveStateAsync(
        Message message,
        TState state,
        CancellationToken cancellationToken
    ) => stateManager.UpdateAsync(message, state, cancellationToken);

    protected Task ClearStateAsync(Message message, CancellationToken cancellationToken) =>
        stateManager.RemoveAsync<TState>(message, cancellationToken);
}
