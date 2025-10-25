using Microsoft.Extensions.Caching.Distributed;
using SauceNAO.Core.Entities.ChatAggregate;
using SauceNAO.Core.Entities.UserAggregate;
using SauceNAO.Core.Extensions;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Services;

/// <inheritdoc/>
class BotHelper(
    ITelegramBotClient client,
    IUserRepository userManager,
    IChatRepostory groupManager,
    IDistributedCache cache
) : IBotHelper
{
    // This helps to avoid re-upserting the same data in the same bot update
    private readonly Dictionary<
        Message,
        (TelegramUser user, TelegramChat? group, string? languageCode)
    > upsertedData = [];

    public async Task<User> GetMeAsync(CancellationToken cancellationToken)
    {
        var me = await cache.GetOrCreateAsync(
            "snao:me",
            async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return await client.GetMeAsync(cancellationToken);
            },
            cancellationToken
        );
        return me ?? await client.GetMeAsync(cancellationToken);
    }

    public async Task<(
        TelegramUser user,
        TelegramChat? group,
        string? languageCode
    )> UpsertDataFromMessageAsync(Message message, CancellationToken cancellationToken)
    {
        if (this.upsertedData.TryGetValue(message, out var existingData))
        {
            return existingData;
        }

        var user = await userManager.UpsertAsync(message.From!, cancellationToken);
        var group =
            message.Chat.Type == ChatTypes.Private
                ? null
                : await groupManager.UpsertFromMessageAsync(message, cancellationToken);
        var languageCode = user.AlwaysUseOwnLanguage
            ? user.LanguageCode
            : group?.LanguageCode ?? user.LanguageCode;
        if (!user.PrivateChatStarted && message.Chat.Type == ChatTypes.Private)
        {
            user.PrivateChatStarted = true;
            await userManager.UpdateAsync(user, cancellationToken);
        }

        var data = (user, group, languageCode);
        this.upsertedData.Add(message, data);
        return data;
    }

    public async Task<bool> IsAdminAsync(
        long userId,
        long chatId,
        CancellationToken cancellationToken
    )
    {
        var admins = await client.GetChatAdministratorsAsync(chatId, cancellationToken);
        return admins.Any(a => a.User.Id == userId);
    }

    public async Task<bool> IsSauceMentioned(Message message, CancellationToken cancellationToken)
    {
        if (message.Chat.Type is ChatTypes.Private)
        {
            return true;
        }
        if (message.ForwardOrigin is not null)
        {
            return false;
        }
        var text = message.Text ?? message.Caption;
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        var me = await this.GetMeAsync(cancellationToken);
        var entities = message.Entities ?? message.CaptionEntities;
        string[] SAUCE_KEYWORDS = ["sauce", "salsa", "saucenao"];

        return entities?.Any(e => e.User?.Id == me.Id) == true
            || SAUCE_KEYWORDS.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
    }
}
