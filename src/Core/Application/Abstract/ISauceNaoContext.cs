// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Localization;
using SauceNAO.Domain.Entities.ChatAggregate;
using SauceNAO.Domain.Entities.UserAggregate;

namespace SauceNAO.Application;

/// <summary>
/// Represents the context object of the SauceNAO bot.
/// </summary>
public interface ISauceNaoContext
{
    /// <summary>
    /// Gets or sets the user that is currently interacting with the bot.
    /// </summary>
    TelegramUser? User { get; }

    /// <summary>
    /// Gets or sets the Group chat where the bot is being used.
    /// </summary>
    TelegramChat? Group { get; }

    /// <summary>
    /// Provides access to the available messages for the current culture.
    /// </summary>
    IStringLocalizer Localizer { get; }
}
