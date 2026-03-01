using SauceNAO.App.Resources;
using SauceNAO.App.Services;
using SauceNAO.Core.Data;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.App.Features.ApiKeys;

sealed class DeleteApiKeyHandler(
    IStateManager stateManager,
    IContextProvider contextProvider,
    IApiKeyRespository apiKeyRespository,
    IBetterStringLocalizer<DeleteApiKeyHandler> localizer,
    ITelegramBotClient client
) : UserStateHandler<DeleteApiKeyState>(stateManager)
{
    protected override async Task ContinueAsync(
        DeleteApiKeyState state,
        Message message,
        CancellationToken cancellationToken
    )
    {
        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );
        await contextProvider.LoadAsync(message, cancellationToken);

        var apiKeys = await apiKeyRespository.GetByUserIdAsync(message.From!.Id, cancellationToken);
        if (!apiKeys.Any(k => k.Name == state.Name))
        {
            await this.ClearStateAsync(message, cancellationToken);
            await client.SendMessageAsync(
                message.Chat.Id,
                localizer["ApiKeyNotFound"],
                parseMode: FormatStyles.HTML,
                replyParameters: new()
                {
                    AllowSendingWithoutReply = true,
                    MessageId = message.MessageId,
                },
                cancellationToken: cancellationToken
            );
            return;
        }

        var input = message.Text;
        if (!state.ConfirmDelete)
        {
            var yesLabel = localizer["Yes"];
            var noLabel = localizer["No"];
            askAgain:
            if (string.IsNullOrEmpty(input))
            {
                await client.SendMessageAsync(
                    message.Chat.Id,
                    localizer["ApiKeyConfirmDelete"],
                    parseMode: FormatStyles.HTML,
                    replyParameters: new()
                    {
                        AllowSendingWithoutReply = true,
                        MessageId = message.MessageId,
                    },
                    replyMarkup: new ReplyKeyboardMarkup(
                        new ReplyKeyboardBuilder()
                            .AppendText(yesLabel, new(Style: "primary"))
                            .AppendText(noLabel, new(Style: "danger"))
                    )
                    {
                        OneTimeKeyboard = true,
                        ResizeKeyboard = true,
                    },
                    cancellationToken: cancellationToken
                );
                return;
            }
            else if (input == yesLabel)
            {
                state.ConfirmDelete = true;
            }
            else if (input == noLabel)
            {
                await this.ClearStateAsync(message, cancellationToken);
                await client.SendMessageAsync(
                    message.Chat.Id,
                    localizer["ApiKeyDeleteCanceled"],
                    parseMode: FormatStyles.HTML,
                    replyParameters: new()
                    {
                        AllowSendingWithoutReply = true,
                        MessageId = message.MessageId,
                    },
                    cancellationToken: cancellationToken
                );
                return;
            }
            else
            {
                goto askAgain;
            }
        }

        foreach (var apiKey in apiKeys.Where(k => k.Name == state.Name))
        {
            await apiKeyRespository.DeleteAsync(apiKey, cancellationToken);
        }
        await this.ClearStateAsync(message, cancellationToken);
        await client.SendMessageAsync(
            message.Chat.Id,
            localizer["ApiKeyDeleted"],
            parseMode: FormatStyles.HTML,
            replyParameters: new()
            {
                AllowSendingWithoutReply = true,
                MessageId = message.MessageId,
            },
            cancellationToken: cancellationToken
        );
    }
}
