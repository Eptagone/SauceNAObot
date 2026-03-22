// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities;
using SauceNAO.Core.Entities.UserAggregate;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Provides methods to retrieve the stored data from users and chats
/// </summary>
public interface IContextProvider
{
    /// <summary>
    /// Load the user context from the given message.
    /// </summary>
    /// <param name="message">The received message</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>A tuple containing the user, group, and language code</returns>
    public Task<UserEntity> LoadAsync(Message message, CancellationToken cancellationToken);

    /// <summary>
    /// Load the user context from the given telegram user object
    /// </summary>
    /// <param name="user">The telegram user object</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    public Task<UserEntity> LoadAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Load and return the full context from the given message.
    /// </summary>
    /// <param name="message">The received message</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    public Task<(UserEntity user, ChatEntity? group)> LoadAllAsync(
        Message message,
        CancellationToken cancellationToken
    );
}
