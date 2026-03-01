// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Thrown when a command failed to execute
/// </summary>
/// <param name="commandName">Command name</param>
/// <param name="message">The message sent by the user that triggered the exception</param>
/// <param name="innerException">The inner exception</param>
public class CommandException(string commandName, Message message, Exception? innerException)
    : MessageException(message, innerException)
{
    /// <summary>
    /// Command name
    /// </summary>
    public readonly string CommandName = commandName;
}
