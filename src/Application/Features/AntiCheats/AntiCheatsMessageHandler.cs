using SauceNAO.Core;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.AntiCheats;

class AntiCheatsMessageHandler(IBotHelper helper) : IMessageHandler
{
    public async Task<bool> HandleAsync(Message message, CancellationToken cancellationToken)
    {
        if (
            message.Chat.Type != ChatTypes.Private
            && message.ForwardOrigin is not null
            && await helper.IsSauceMentioned(message, cancellationToken)
        )
        {
            var (_, group, languageCode) = await helper.UpsertDataFromMessageAsync(
                message,
                cancellationToken
            );
            var media =
                SauceNaoUtilities.ExtractMediaFromMessage(message)
                ?? SauceNaoUtilities.ExtractMediaFromMessage(message.ReplyToMessage);
            if (
                media is not null
                && media.Message.From?.IsBot is true
                && group?.Restrictions.Any(r => r.RestrictedBotId == media.Message.From.Id) is true
            )
            {
                throw new CheatingException(message);
            }
        }

        return false;
    }
}
