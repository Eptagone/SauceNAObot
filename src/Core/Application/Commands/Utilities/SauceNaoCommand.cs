// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Reflection;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

/// <summary>
/// Represents a directory of commands.
/// </summary>
sealed class SauceNAOCommand : ExtendedBotCommand, IEquatable<SauceNAOCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SauceNAOCommand"/> class.
    /// </summary>
    /// <param name="type">The type of the command.</param>
    public SauceNAOCommand(Type type)
        : base(type)
    {
        var visibilityAttribute = type.GetCustomAttribute<BotCommandVisibilityAttribute>();
        this.Visibility = visibilityAttribute?.Visibility ?? BotCommandVisibility.Default;
    }

    /// <summary>
    /// Gets the visibility of the command.
    /// </summary>
    public BotCommandVisibility Visibility { get; }

    /// <inheritdoc />
    public bool Equals(SauceNAOCommand? other)
    {
        return this.Command == other?.Command;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is SauceNAOCommand command)
        {
            return this.Equals(command);
        }

        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.Command.GetHashCode();
    }
}
