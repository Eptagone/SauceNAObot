using SauceNAO.Core.Entities.ChatAggregate;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Specifications;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Infrastructure.Data.Repositories;

class ChatRepository(ApplicationDbContext context)
    : AsyncRepositoryBase<TelegramChat>(context),
        IChatRepostory
{
    /// <inheritdoc/>
    public Task<TelegramChat> UpsertFromMessageAsync(
        Message message,
        CancellationToken cancellationToken
    )
    {
        var spec = new ChatSpecification(message.Chat.Id);
        var groupEntity = spec.Evaluate(context.Chats).SingleOrDefault();

        if (groupEntity is null)
        {
            groupEntity = new TelegramChat(message.Chat.Id, message.Chat.Title!)
            {
                Username = message.Chat.Username,
            };
            return this.InsertAsync(groupEntity, cancellationToken);
        }

        if (message.MigrateToChatId is not null && message.MigrateFromChatId == message.Chat.Id)
        {
            groupEntity.ChatId = (long)message.MigrateToChatId;
        }
        groupEntity.Title = message.Chat.Title!;
        groupEntity.Username = message.Chat.Username;
        return this.UpdateAsync(groupEntity, cancellationToken);
    }
}
