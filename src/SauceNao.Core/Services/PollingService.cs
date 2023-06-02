// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Core.Services;

/// <summary>
/// This class implements a long polling service for SauceNAO bot.
/// </summary>
public class PollingService : BackgroundService
{
	private readonly SnaoBotProperties _properties;
	private readonly ILogger<PollingService> _logger;
	private readonly IServiceProvider _serviceProvider;

	/// <summary>
	/// Initialize a new instance of the <see cref="PollingService"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="properties">The bot properties.</param>
	/// <param name="serviceProvider">The service provider.</param>
	public PollingService(ILogger<PollingService> logger, SnaoBotProperties properties, IServiceProvider serviceProvider)
	{
		(this._properties, this._logger, this._serviceProvider) = (properties, logger, serviceProvider);
	}

	/// <summary>
	/// Start the long polling service.
	/// </summary>
	/// <param name="stoppingToken">The cancellation token.</param>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var webhookInfo = await this._properties.Api
			.GetWebhookInfoAsync(cancellationToken: stoppingToken)
			.ConfigureAwait(false);

		// If webhook is set, terminate the polling service.
		if (!string.IsNullOrEmpty(webhookInfo.Url))
		{
			this._logger.LogInformation("Webhook detected. Polling service was stopped.");
			return;
		}

		this._logger.LogInformation("Polling Service running at: {time}", DateTimeOffset.Now);

		// Get Updates for the first time.
		var updates = await this._properties.Api.GetUpdatesAsync(allowedUpdates: Array.Empty<string>(), cancellationToken: stoppingToken).ConfigureAwait(false);

		// Loop until cancellation is requested.
		while (!stoppingToken.IsCancellationRequested)
		{
			// Process updates if any updates are available.
			if (updates.Any())
			{
				/// Process updates in parallel.
				await Parallel.ForEachAsync(updates, stoppingToken, async (update, cancellationToken) => await this.ProcessUpdate(update, cancellationToken)).ConfigureAwait(false);

				// Get updates from the last update id + 1.
				updates = await this._properties.Api.GetUpdatesAsync(updates[^1].UpdateId + 1, cancellationToken: stoppingToken).ConfigureAwait(false);
			}
			// Get updates if no updates are available.
			else
			{
				updates = await this._properties.Api.GetUpdatesAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
			}
		}
	}

	/// <summary>
	/// Stop the polling service.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	public override Task StopAsync(CancellationToken cancellationToken)
	{
		this._logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);
		return base.StopAsync(cancellationToken);
	}

	/// <summary>
	/// Process a new update from Telegram.
	/// </summary>
	/// <param name="update">The update to process.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	private async Task ProcessUpdate(Update update, CancellationToken cancellationToken)
	{
		using var scope = this._serviceProvider.CreateScope();
		var bot = scope.ServiceProvider.GetRequiredService<SauceNaoBot>();
		await bot.OnUpdateAsync(update, cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
