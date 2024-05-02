// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Application;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.WebApp;

public class LongPollingWorker(
    ILogger<LongPollingWorker> logger,
    ITelegramBotClient client,
    IUpdateReceiver updateReceiver
) : BackgroundService
{
    private readonly ILogger<LongPollingWorker> logger = logger;
    private readonly ITelegramBotClient client = client;
    private readonly IUpdateReceiver updateReceiver = updateReceiver;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IEnumerable<Update> updates = [];
        this.logger.LogInformation("Polling for updates...");
        while (!stoppingToken.IsCancellationRequested)
        {
            if (updates.Any())
            {
                // Pass the updates to the update receiver.
                foreach (var update in updates)
                {
                    this.updateReceiver.ReceiveUpdate(update);
                }

                // Get offset for the next update.
                var offset = updates.Max(u => u.UpdateId) + 1;
                updates = await this.client.GetUpdatesAsync(
                    offset,
                    cancellationToken: stoppingToken
                );
            }
            else
            {
                // Wait 100 ms before polling again.
                await Task.Delay(100, stoppingToken);
                // Get updates from the bot API.
                updates = await this.client.GetUpdatesAsync(cancellationToken: stoppingToken);
            }
        }
    }
}
