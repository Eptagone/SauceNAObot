// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Represents a command
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Invokes the command
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="args">The arguments</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task InvokeAsync(Message message, string[] args, CancellationToken cancellationToken);
}
