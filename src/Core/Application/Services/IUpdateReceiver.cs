// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Hosting;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Application.Services;

/// <summary>
/// Defines a method to receive updates to be processed by the Telegram bot.
/// </summary>
public interface IUpdateReceiver : IHostedService
{
    /// <summary>
    /// Receives updates from the Telegram bot.
    /// </summary>
    /// <param name="update">The update.</param>
    public void ReceiveUpdate(Update update);
}
