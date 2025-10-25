// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
using SauceNAO.Core.Services;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Application.Services;

/// <summary>
/// Receives updates from the Telegram bot.
/// </summary>
/// <param name="logger">The logger.</param>
/// <param name="serviceProvider">The service provider.</param>
class UpdateReceiver(ILogger<UpdateReceiver> logger, IServiceProvider serviceProvider)
    : BackgroundService,
        IUpdateReceiver
{
    private readonly Channel<Update> updates = Channel.CreateUnbounded<Update>();

    /// <inheritdoc />
    public void ReceiveUpdate(Update update)
    {
        if (!this.updates.Writer.TryWrite(update))
        {
            logger.LogFailedToAddUpdateToPool(update.UpdateId);
        }
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var actionBlock = new ActionBlock<(Update, CancellationToken)>(
            this.ProcessUpdateAsync,
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 128,
                CancellationToken = stoppingToken,
            }
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            var update = await this.updates.Reader.ReadAsync(stoppingToken);
            if (update is not null)
            {
                actionBlock.Post((update, stoppingToken));
            }
        }

        actionBlock.Complete();
    }

    // Process the update.
    private async Task ProcessUpdateAsync((Update, CancellationToken) input)
    {
        var (update, cancellationToken) = input;
        using var scope = serviceProvider.CreateScope();
        try
        {
            var handler = scope.ServiceProvider.GetRequiredService<IAsyncUpdateHandler>();
            await handler.OnUpdateAsync(update, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogFailedToProcessUpdate(e, update.UpdateId);
        }
    }
}

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 10,
        Level = LogLevel.Critical,
        Message = "Failed to add the update {UpdateId} to the updates pool"
    )]
    internal static partial void LogFailedToAddUpdateToPool(
        this ILogger<UpdateReceiver> logger,
        int updateId
    );

    [LoggerMessage(
        EventId = 11,
        Level = LogLevel.Error,
        Message = "Failed to process the update {UpdateId}"
    )]
    internal static partial void LogFailedToProcessUpdate(
        this ILogger<UpdateReceiver> logger,
        Exception e,
        int updateId
    );
}
