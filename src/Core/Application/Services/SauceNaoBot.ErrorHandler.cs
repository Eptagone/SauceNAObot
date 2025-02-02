// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Services;

partial class SauceNaoBot : SimpleTelegramBotBase, ISauceNaoBot
{
    /// <inheritdoc />
    protected override Task OnBotExceptionAsync(
        BotRequestException exp,
        CancellationToken cancellationToken = default
    )
    {
        this.logger.LogError(exp, "An error occurred while processing the request.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnExceptionAsync(
        Exception exp,
        CancellationToken cancellationToken = default
    )
    {
        this.logger.LogError(exp, "An error occurred while processing the request.");
        return Task.CompletedTask;
    }
}
