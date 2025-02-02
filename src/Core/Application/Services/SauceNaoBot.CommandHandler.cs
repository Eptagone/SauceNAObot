// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using SauceNAO.Application.Commands;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Services;

partial class SauceNaoBot : SimpleTelegramBotBase, ISauceNaoBot
{
    [GeneratedRegex(@"""(?<arg>[^""]+)""|(?<arg>\S+)", RegexOptions.Compiled)]
    private static partial Regex GetArgPattern();

    /// <inheritdoc />
    protected override Task OnCommandAsync(
        Message message,
        string commandName,
        string commandArgs,
        CancellationToken cancellationToken = default
    )
    {
        // Get the command instance from the service provider if it exists.
        var command = this.serviceProvider.GetKeyedService<ITelegramBotCommand>(
            CommandDirectory.GetCommand(commandName)?.Command
        );

        // If the command is not found, return.
        if (command is null)
        {
            return Task.CompletedTask;
        }

        var args = GetArgPattern()
            .Matches(commandArgs)
            .Select(match => match.Groups["arg"].Value)
            .ToArray();
        // Set the command context.
        command.Context = this.Context;
        // Invoke the command asynchronously and return the task.
        return command.InvokeAsync(message, args, cancellationToken);
    }
}
