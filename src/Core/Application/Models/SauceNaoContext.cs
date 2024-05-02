// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Localization;
using SauceNAO.Domain.Entities.ChatAggregate;
using SauceNAO.Domain.Entities.UserAggregate;

namespace SauceNAO.Application.Models;

/// <summary>
/// Represents the context object of the SauceNAO bot.
/// </summary>
/// <param name="User">The user that is currently interacting with the bot.</param>
/// <param name="Group">The Group chat where the bot is being used.</param>
/// <param name="Localizer">Provides access to the available messages for the current culture.</param>
record SauceNaoContext(TelegramUser? User, TelegramChat? Group, IStringLocalizer Localizer)
    : ISauceNaoContext { }
