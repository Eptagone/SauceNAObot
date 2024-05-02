// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI;

namespace SauceNAO.Application.Services;

/// <summary>
/// Represents the SauceNAO bot.
/// </summary>
public interface ISauceNaoBot : ITelegramBot
{
    /// <summary>
    /// Unique identifier for this bot.
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// Bot's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Bot's username.
    /// </summary>
    public string Username { get; }
}
