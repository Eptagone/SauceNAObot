// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Application;

/// <summary>
/// Represents the factory that creates instances of <see cref="ISauceNaoContext"/>.
/// </summary>
public interface ISauceNaoContextFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="ISauceNaoContext"/>.
    /// </summary>
    /// <param name="client">The Telegram bot client.</param>
    /// <param name="update">The update that triggered the creation of the context.</param>
    /// <returns>A new instance of <see cref="ISauceNaoContext"/>.</returns>
    ISauceNaoContext Create(Update update);
}
