// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
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
        var actionBlock = new ActionBlock<Update>(
            this.ProcessUpdateAsync,
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 20,
                CancellationToken = stoppingToken
            }
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            var update = await this.updates.Reader.ReadAsync(stoppingToken);
            if (update is not null)
            {
                actionBlock.Post(update);
            }
        }

        actionBlock.Complete();
    }

    // Process the update.
    private async Task ProcessUpdateAsync(Update update)
    {
        using var scope = serviceProvider.CreateScope();
        try
        {
            var bot = scope.ServiceProvider.GetRequiredService<ISauceNaoBot>();
            await bot.OnUpdateAsync(update);
        }
        catch (Exception e)
        {
            logger.LogFailedToProcessUpdate(e, update.UpdateId);
        }
    }
}
