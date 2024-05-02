// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

/// <summary>
/// Represents a directory of commands.
/// </summary>
static class CommandDirectory
{
    private static readonly ICollection<SauceNAOCommand> commands = [];

    /// <summary>
    /// Gets a command by its name or alias.
    /// </summary>
    internal static IExtendedBotCommand? GetCommand(string commandName) =>
        commands.FirstOrDefault(c => c.Aliases.Contains(commandName));

    /// <summary>
    /// Gets all commands available in all chats.
    /// </summary>
    /// <returns>A list of commands available in all chats.</returns>
    internal static IEnumerable<IExtendedBotCommand> GetDefaultCommands() =>
        commands.Where(c => c.Visibility == BotCommandVisibility.Default);

    /// <summary>
    /// Gets all commands available in private chats.
    /// </summary>
    /// <returns>A list of commands available in private chats.</returns>
    internal static IEnumerable<IExtendedBotCommand> GetPrivateChatCommands() =>
        commands.Where(c =>
            c.Visibility == BotCommandVisibility.Default
            || (c.Visibility & BotCommandVisibility.PrivateChat) != 0
        );

    /// <summary>
    /// Gets all commands available in groups.
    /// </summary>
    /// <returns>A list of commands available in groups.</returns>
    internal static IEnumerable<IExtendedBotCommand> GetGroupCommands() =>
        commands.Where(c =>
            c.Visibility == BotCommandVisibility.Default
            || (c.Visibility & BotCommandVisibility.Group) != 0
        );

    /// <summary>
    /// Gets all commands available for group administrators.
    /// </summary>
    /// <returns>A list of commands available for group administrators.</returns>
    internal static IEnumerable<IExtendedBotCommand> GetGroupAdminCommands() =>
        commands.Where(c =>
            c.Visibility == BotCommandVisibility.Default
            || (c.Visibility & BotCommandVisibility.GroupAdmin) != 0
        );

    /// <summary>
    /// Adds a command to the directory.
    /// </summary>
    /// <param name="command">The command to add.</param>
    internal static void AddCommand(SauceNAOCommand command) => commands.Add(command);
}
