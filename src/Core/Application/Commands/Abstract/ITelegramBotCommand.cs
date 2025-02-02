// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Commands;

/// <summary>
/// Defines a command for the Telegram bot.
/// </summary>
interface ITelegramBotCommand
{
    /// <summary>
    /// Bot context
    /// </summary>
    ISauceNaoContext? Context { set; get; }

    /// <summary>
    /// Invokes the command asynchronously.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task InvokeAsync(Message message, string[] args, CancellationToken cancellationToken);
}
