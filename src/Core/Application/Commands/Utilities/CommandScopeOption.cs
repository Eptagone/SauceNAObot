// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Application.Commands;

/// <summary>
/// Represents the scope of a command.
/// </summary>
enum BotCommandVisibility
{
    /// <summary>
    /// The command is available in all chats.
    /// </summary>
    Default,

    /// <summary>
    /// The command is available in all chats but it is not shown in the command list.
    /// </summary>
    Hidden,

    /// <summary>
    /// The command is available in private chats only.
    /// </summary>
    PrivateChat,

    /// <summary>
    /// The command is available in groups and supergroups only.
    /// </summary>
    Group,

    /// <summary>
    /// The command is available for administrators only in groups and supergroups.
    /// </summary>
    GroupAdmin,
}
