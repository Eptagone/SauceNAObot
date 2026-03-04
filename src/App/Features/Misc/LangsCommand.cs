using Microsoft.Extensions.Caching.Distributed;
using SauceNAO.App.Resources;
using SauceNAO.Core.Data;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Features.Misc;

[BotCommand("langs", "List the supported languages")]
[BotCommandVisibility(BotCommandVisibility.Hidden)]
class LangsCommand(
    IContextProvider contextProvider,
    ITelegramBotClient client,
    IUserRepository userRepository,
    IBetterStringLocalizer<LangsCommand> localizer,
    IDistributedCache cache
) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );
        await contextProvider.LoadAsync(message, cancellationToken);
        var languages = await cache.GetStringAsync("languages", cancellationToken);
        if (languages is null)
        {
            var languageCodes = await userRepository.GetLanguageCodesAsync(cancellationToken);
            languages = string.Join(
                "\n",
                languageCodes
                    .OrderByDescending(lc => lc.Value)
                    .Select(lc => $"<b>{lc.Key}:</b> [{lc.Value}]")
            );
            await cache.SetStringAsync(
                "languages",
                languages,
                new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) },
                cancellationToken
            );
        }

        var text = localizer["Languages", languages];
        await client.SendMessageAsync(
            message.Chat.Id,
            text,
            replyParameters: new()
            {
                AllowSendingWithoutReply = true,
                MessageId = message.MessageId,
            },
            parseMode: FormatStyles.HTML,
            cancellationToken: cancellationToken
        );
    }
}
