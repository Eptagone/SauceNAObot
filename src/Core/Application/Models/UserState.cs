// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Application.Models;

/// <summary>
/// Represents the user state.
/// </summary>
/// <param name="Scope">The scope describing which part of the bot is waiting for the user's response. Example: <c>"ApiKey"</c>.</param>
/// <param name="ChatId">Unique identifier for the chat where the bot is waiting for the user's response.</param>
/// <param name="UserId">Unique identifier for the user that is currently interacting with the bot.</param>
/// <param name="Data">Additional data associated with the user state.</param>
public record UserState(string Scope, long ChatId, long UserId, IDictionary<string, string?> Data);
