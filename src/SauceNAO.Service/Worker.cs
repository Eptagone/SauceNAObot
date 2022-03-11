// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Service
{
    public class Worker : BackgroundService
    {
        private readonly BotClient _botapi;
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, SnaoBotProperties properties, IServiceProvider serviceProvider)
        {
            (_botapi, _logger, _serviceProvider) = (properties.Api, logger, serviceProvider);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            // Long Polling
            var updates = await _botapi.GetUpdatesAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
            while (!stoppingToken.IsCancellationRequested)
            {
                if (updates.Any())
                {
                    await Parallel.ForEachAsync(updates, stoppingToken, async (update, cancellationToken) => await ProcessUpdate(update, cancellationToken)).ConfigureAwait(false);

                    updates = await _botapi.GetUpdatesAsync(updates[^1].UpdateId + 1, cancellationToken: stoppingToken).ConfigureAwait(false);
                }
                else
                {
                    updates = await _botapi.GetUpdatesAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);
            return base.StopAsync(cancellationToken);
        }

        private async Task ProcessUpdate(Update update, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var bot = scope.ServiceProvider.GetRequiredService<SauceNaoBot>();
            await bot.OnUpdateAsync(update, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
