// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.Media;

/// <summary>
/// Thrown when the bot is unable to process a request because the received file format is not supported
/// </summary>
/// <param name="message">The message sent by the user that triggered the exception</param>
public sealed class UnsupportedFormatException(Message message) : MessageMediaException(message) { }
