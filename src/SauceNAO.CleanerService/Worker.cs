// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Extensions;
using SauceNAO.Infrastructure;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.CleanerService;

public class Worker : BackgroundService
{
	private readonly BotClient _botapi;
	private readonly ILogger<Worker> _logger;
	private readonly IServiceScopeFactory _serviceFactory;

	public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
	{
		this._logger = logger;
		this._serviceFactory = scopeFactory;

		var telegram = configuration.GetSection("Telegram") ?? throw new ArgumentNullException(nameof(configuration), "Missing config section: 'Telegram'");
		var botToken = telegram["BotToken"] ?? throw new ArgumentNullException(nameof(configuration), "Missing config entry: 'Telegram:BotToken'");

		this._botapi = new BotClient(botToken);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			this._logger.LogInformation("Cleaner Service running at: {time}", DateTimeOffset.Now);

			await this.RemoveOldSauces(stoppingToken).ConfigureAwait(false);
			await this.RemoveMissingChats(stoppingToken).ConfigureAwait(false);
			await this.RemoveMissingUsers(stoppingToken).ConfigureAwait(false);

			this._logger.LogInformation("Cleaner Service finished at: {time}", DateTimeOffset.Now);

			// Delay 1 day
			await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
		}
	}

	/// <summary>
	/// Remove old sauces from the database.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns></returns>
	private async Task RemoveOldSauces(CancellationToken cancellationToken)
	{
		using var scope = this._serviceFactory.CreateScope();
		using var context = scope.ServiceProvider.GetRequiredService<SauceNaoContext>();

		var oldSauces = context.SuccessfulSauces
			.Where(s => s.Date < DateTime.UtcNow.AddDays(-40));
		var sauceCount = oldSauces.Count();

		if (sauceCount == 0)
		{
			return;
		}

#if DEBUG
		this._logger.LogInformation("{sauceCount} sauces will be cleaned", sauceCount);
#endif

		context.RemoveRange(oldSauces);
		await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Remove missing chats from the database.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns></returns>
	private async Task RemoveMissingChats(CancellationToken cancellationToken)
	{
		using var scope = this._serviceFactory.CreateScope();
		using var context = scope.ServiceProvider.GetRequiredService<SauceNaoContext>();

		var groups = context.Groups.ToList();

#if DEBUG
		this._logger.LogInformation("Missing groups will be cleaned.");
#endif

		var me = await this._botapi.GetMeAsync(cancellationToken).ConfigureAwait(false);

		foreach (var group in groups)
		{
			try
			{
				// Get the chat.
				await this._botapi.GetChatAsync(group.Id, cancellationToken).ConfigureAwait(false);

				// Get my member profile.
				var myMemberProfile = await this._botapi.GetChatMemberAsync(group.Id, me.Id, cancellationToken).ConfigureAwait(false);

				// If I'm not a member or admin, leave the chat.
				if (!myMemberProfile.IsMemberOrAdmin())
				{
					try
					{
						await this._botapi.LeaveChatAsync(group.Id, cancellationToken).ConfigureAwait(false);
					}
					finally
					{
						// Remove the chat from the database.
						context.Remove(group);
					}
					continue;
				}
			}
			catch (BotRequestException e)
			{
				// If message is "Bad Request: chat not found", remove the chat from the database.
				if (e.ErrorCode == 400 && e.Description.Contains("chat not found", StringComparison.OrdinalIgnoreCase))
				{
					this._logger.LogWarning("Chat \"{title}\" (id: {id}) not found. Chat's data will be cleaned from database.", group.Title, group.Id);
					context.Remove(group);
				}
				else
				{
#if DEBUG
					this._logger.LogWarning("Unable to get \"{title}\" group. Error message: {message}\n.", group.Title, e.Message);
#endif
				}
			}
			catch (Exception e)
			{
				this._logger.LogError("Error while checking \"{title}\" group. Error message: {message}", group.Title, e.Message);
			}
		}

		// Save changes.
		await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Remove missing users from the database.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	private async Task RemoveMissingUsers(CancellationToken cancellationToken)
	{
		using var scope = this._serviceFactory.CreateScope();
		using var context = scope.ServiceProvider.GetRequiredService<SauceNaoContext>();

		// Get all users who haven't search for a long time will be removed.
		var users = context.Users.AsQueryable()
			.Include(u => u.UserSauces)
			.Where(u => !u.UserSauces.Any()).ToArray();

#if DEBUG
		this._logger.LogInformation("Missing users will be cleaned.");
#endif

		foreach (var user in users)
		{
			try
			{
				// Get the chat by id.
				Chat chat = await this._botapi.GetChatAsync(user.Id, cancellationToken).ConfigureAwait(false);
#if DEBUG
				this._logger.LogInformation("Chat \"{name}\" (id: {id}) (username: @{}) found.", user.GetFullname(), user.Id, user.Username);
#endif
			}
			catch (BotRequestException e)
			{
				// The user will be removed from the database if any of the following errors occurs:
				var messages = new string[] {
						"Bad Request: chat not found",
						"Forbidden: bot was blocked by the user",
						"Forbidden: user is deactivated",
					};

				if (messages.Contains(e.Message))
				{
#if DEBUG
					this._logger.LogWarning("Unable to get \"{name}\" (\"{username}\") user. Error message: {message}\nUser's data will be cleaned from database.", user.GetFullname(), user.Username, e.Message);
#endif
					context.Remove(user);
				}
			}
		}

		// Save changes.
		await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}
}
