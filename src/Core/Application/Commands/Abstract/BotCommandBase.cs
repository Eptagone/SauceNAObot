// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Entities.UserAggregate;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Commands;

/// <summary>
/// Represents a base class to build bot commands.
/// </summary>
abstract class BotCommandBase : ITelegramBotCommand
{
    /// <summary>
    /// SauseNAO context.
    /// </summary>
    private ISauceNaoContext? context;

    /// <summary>
    /// SauseNAO context.
    /// </summary>
    protected ISauceNaoContext Context =>
        this.context ?? throw new InvalidOperationException("The command context is not set.");

    /// <summary>
    /// The user who sent the command.
    /// </summary>
    protected TelegramUser User =>
        this.Context.User ?? throw new InvalidOperationException("The command context is not set.");

    /// <inheritdoc />
    protected virtual Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken = default
    ) => this.InvokeAsync(message, cancellationToken);

    /// <inheritdoc />
    protected virtual Task InvokeAsync(Message message, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    /// <inheritdoc />
    ISauceNaoContext? ITelegramBotCommand.Context
    {
        get => this.context;
        set => this.context = value;
    }

    /// <inheritdoc />
    Task ITelegramBotCommand.InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    ) => this.InvokeAsync(message, args, cancellationToken);
}
