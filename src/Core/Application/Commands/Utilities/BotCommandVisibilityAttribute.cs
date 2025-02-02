// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Application.Commands;

/// <summary>
/// Represents the visibility of a bot command.
/// </summary>
/// <param name="visibility"></param>
[AttributeUsage(AttributeTargets.Class)]
sealed class BotCommandVisibilityAttribute(BotCommandVisibility visibility) : Attribute
{
    /// <summary>
    /// Specifies the visibility of a bot command.
    /// </summary>
    public BotCommandVisibility Visibility { get; } = visibility;
}
