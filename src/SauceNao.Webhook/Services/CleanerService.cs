// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core;
using SauceNAO.Core.Entities;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Webhook.Services
{
    public sealed class CleanerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CleanerService> _logger;

        public CleanerService(IServiceScopeFactory scopeFactory, ILogger<CleanerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IBotDb>();

            _logger.LogInformation("Cleaner Service is running.");
            var oldSauces = db.Sauces.GetAllSauces()
                .Where(s => s.Date < DateTime.UtcNow.AddDays(-20));
            var sauceCount = oldSauces.Count();
            _logger.LogInformation("{0} sauces will be cleaned", sauceCount);
            await db.Sauces.DeleteRangeAsync(oldSauces, stoppingToken).ConfigureAwait(false);

            var groups = db.Groups.GetAllGroups();
            _logger.LogInformation("Missing groups will be cleaned.");

            var properties = scope.ServiceProvider.GetRequiredService<SnaoBotProperties>();
            foreach (var group in groups)
            {
                var api = properties.Api;
                var me = properties.User;
                try
                {
                    await api.GetChatAsync(group.Id, stoppingToken).ConfigureAwait(false);
                    var myMemberProfile = await api.GetChatMemberAsync(group.Id, me.Id, stoppingToken).ConfigureAwait(false);
                    if (myMemberProfile is not ChatMemberMember or ChatMemberAdministrator)
                    {
                        try
                        {
                            await api.LeaveChatAsync(group.Id, stoppingToken).ConfigureAwait(false);
                        }
                        finally
                        {
                            await db.Groups.DeleteAsync(group, stoppingToken).ConfigureAwait(false);
                        }
                        continue;
                    }
                }
                catch (BotRequestException e)
                {
                    _logger.LogWarning("Unable to get \"{0}\" group. Error message: {1}\nChat's data will be cleaned from database.", group.Title, e.Message);
                    await db.Groups.DeleteAsync(group, stoppingToken).ConfigureAwait(false);
                    continue;
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while checking \"{0}\" group. Error message: {1}", group.Title, e.Message);
                    continue;
                }

                if (group.AntiCheats.Any())
                {
                    foreach (AntiCheat item in group.AntiCheats)
                    {
                        try
                        {
                            var chatMember = await api.GetChatMemberAsync(group.Id, item.BotId, stoppingToken).ConfigureAwait(false);
                            if (chatMember is ChatMemberLeft or ChatMemberBanned)
                            {
                                group.AntiCheats.Remove(item);
                            }
                        }
                        catch
                        {
                            group.AntiCheats.Remove(item);
                        }
                    }
                    await db.Groups.UpdateAsync(group, stoppingToken).ConfigureAwait(false);
                }
            }
            _logger.LogInformation("Cleaner Service was finished.");
        }
    }
}
