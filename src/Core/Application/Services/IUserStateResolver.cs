// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Application.Models;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Services;

/// <summary>
/// Defines a method to handle user state
/// </summary>
/// <param name="userState">The user state</param>
/// <param name="cancellationToken">The cancellation token</param>
public interface IUserStateHandler
{
    /// <summary>
    /// Bot context
    /// </summary>
    ISauceNaoContext? Context { set; get; }

    /// <summary>
    /// Resolve the user state
    /// </summary>
    /// <param name="userState">The user state</param>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task ResolveStateAsync(
        UserState userState,
        Message message,
        CancellationToken cancellationToken
    );
}
