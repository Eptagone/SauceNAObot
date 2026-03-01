// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
using SauceNAO.Core.Services;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.App.Services;

sealed class UpdateHandlerPool(ILogger<UpdateHandlerPool> logger, IServiceProvider serviceProvider)
    : BackgroundService,
        IUpdateHandlerPool
{
    private readonly Channel<Update> updates = Channel.CreateUnbounded<Update>();

    /// <inheritdoc />
    public void QueueUpdate(Update update)
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
                MaxDegreeOfParallelism = 32,
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

#if DEBUG
        var stoppingToken = cancellationToken;
#else
        var ss = new CancellationTokenSource();
        ss.CancelAfter(TimeSpan.FromSeconds(30));
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ss.Token);
        var stoppingToken = cts.Token;
#endif

        using var scope = serviceProvider.CreateScope();
        try
        {
            var handler = scope.ServiceProvider.GetRequiredService<IUpdateHandler>();
            await handler.HandleAsync(update, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogCancelledUpdate(update.UpdateId);
        }
        catch (Exception e)
        {
            logger.LogFailedToProcessUpdate(update.UpdateId, e);
        }
    }
}
