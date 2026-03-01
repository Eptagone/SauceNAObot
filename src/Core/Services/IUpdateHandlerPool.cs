// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Core.Services;

/// <summary>
/// Defines a method to receive updates to be processed later.
/// </summary>
public interface IUpdateHandlerPool
{
    /// <summary>
    /// Receives updates from the Telegram bot.
    /// </summary>
    /// <param name="update">The update.</param>
    void QueueUpdate(Update update);
}
