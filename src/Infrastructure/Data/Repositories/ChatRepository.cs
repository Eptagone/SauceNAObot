// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Infrastructure.Data.Repositories;

sealed class ChatRepository(SnaoDbContext context)
    : RepositoryBase<ChatEntity>(context),
        IChatRepostory
{
    /// <inheritdoc/>
    public Task<ChatEntity> UpsertFromMessageAsync(
        Message message,
        CancellationToken cancellationToken
    )
    {
        var chatId = message.MigrateFromChatId ?? message.Chat.Id;
        var groupEntity = context
            .Groups.AsNoTrackingWithIdentityResolution()
            .SingleOrDefault(g => g.ChatId == chatId);

        if (groupEntity is null)
        {
            groupEntity = new ChatEntity(message.Chat.Id, message.Chat.Title!)
            {
                Username = message.Chat.Username,
            };
            return this.InsertAsync(groupEntity, cancellationToken);
        }

        if (message.MigrateToChatId is not null)
        {
            groupEntity.ChatId = (long)message.MigrateToChatId;
        }
        groupEntity.Title = message.Chat.Title!;
        groupEntity.Username = message.Chat.Username;
        return this.UpdateAsync(groupEntity, cancellationToken);
    }
}
