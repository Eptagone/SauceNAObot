using Microsoft.Extensions.Caching.Distributed;
using SauceNAO.Application.Resources;
using SauceNAO.Core.Extensions;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.Application.Features.Languages;

[BotCommand("langs", "Shows the languages used by the users", ["languages"])]
[BotCommandVisibility(BotCommandVisibility.Hidden)]
class LanguagesCommand(
    ITelegramBotClient client,
    IBotHelper helper,
    IBetterStringLocalizer<LanguagesCommand> localizer,
    IUserRepository userRepository,
    IDistributedCache cache
) : IBotCommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        var (_, _, languageCode) = await helper.UpsertDataFromMessageAsync(
            message,
            cancellationToken
        );
        if (!string.IsNullOrEmpty(languageCode))
        {
            localizer.ChangeCulture(languageCode);
        }

        var msg = await cache.GetOrCreateAsync(
            $"snao:languages-msg-{languageCode ?? "default"}",
            async entry =>
            {
                // Send a message indicating that the bot is processing the command.
                await client.SendChatActionAsync(
                    message.Chat.Id,
                    ChatActions.Typing,
                    cancellationToken: cancellationToken
                );
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return await this.GetMessageAsync(cancellationToken);
            },
            cancellationToken
        );
        msg ??= await this.GetMessageAsync(cancellationToken);

        await client.SendMessageAsync(
            message.Chat.Id,
            msg,
            parseMode: FormatStyles.HTML,
            replyParameters: new ReplyParameters
            {
                MessageId = message.MessageId,
                AllowSendingWithoutReply = true,
            },
            cancellationToken: cancellationToken
        );
    }

    // Generates the message with the statistics.
    private async Task<string> GetMessageAsync(CancellationToken cancellationToken)
    {
        var languages = await userRepository.GetLanguageCodesAsync(cancellationToken);
        var text = languages
            .OrderByDescending(lc => lc.Value)
            .Select(lc => $"<b>{lc.Key}:</b> [{lc.Value}]");

        return localizer["Languages", string.Join("\n", text)];
    }
}
