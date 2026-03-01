using SauceNAO.App.Resources;
using SauceNAO.Core.Data;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Features.ApiKeys;

[
    BotCommand("apikeys", "Manage your API keys to use with the bot"),
    LocalizedBotCommand("es", "Gestionar tus claves API para usar con el bot"),
    BotCommandVisibility(BotCommandVisibility.PrivateChats)
]
sealed class ApiKeyCommand(
    IStateManager stateManager,
    IApiKeyRespository apiKeyRespository,
    IContextProvider contextProvider,
    IBetterStringLocalizer<ApiKeyCommand> localizer,
    ITelegramBotClient client
) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        if (message.Chat.Type == ChatTypes.Private)
        {
            await contextProvider.LoadAsync(message, cancellationToken);
            var action = args.FirstOrDefault() ?? "help";
            switch (action)
            {
                case "add":
                    {
                        var state = new AddApiKeyState(
                            args.ElementAtOrDefault(1),
                            args.ElementAtOrDefault(2)
                        );
                        var isPublicArg = args.ElementAtOrDefault(3);
                        if (
                            isPublicArg == "true"
                            || isPublicArg == "--public"
                            || isPublicArg == "—public"
                        )
                        {
                            state.IsPublic = true;
                        }
                        await stateManager.DispatchAsync(message, state, cancellationToken);
                    }
                    break;

                case "delete":
                case "remove":
                    {
                        var name = args.ElementAtOrDefault(1);
                        if (string.IsNullOrEmpty(name))
                        {
                            goto default;
                        }
                        var state = new DeleteApiKeyState(name);
                        var forceArg = args.ElementAtOrDefault(2);
                        if (forceArg == "--force" || forceArg == "—force")
                        {
                            state.ConfirmDelete = true;
                        }
                        await stateManager.DispatchAsync(message, state, cancellationToken);
                    }
                    break;

                case "list":
                    {
                        await client.SendChatActionAsync(
                            message.Chat.Id,
                            ChatActions.Typing,
                            cancellationToken: cancellationToken
                        );
                        var apiKeys = await apiKeyRespository.GetByUserIdAsync(
                            message.From!.Id,
                            cancellationToken
                        );
                        await contextProvider.LoadAsync(message, cancellationToken);
                        var text = apiKeys.Any()
                            ? localizer[
                                "ApiKeyList",
                                apiKeys
                                    .Select(k =>
                                        $"<b>{k.Name}:</b> <tg-spoiler><code>{k.Value}</code></tg-spoiler>"
                                    )
                                    .Aggregate((a, b) => $"{a}\n{b}")
                            ]
                            : localizer["ApiKeyListEmpty"];
                        await client.SendMessageAsync(
                            message.Chat.Id,
                            text,
                            parseMode: FormatStyles.HTML,
                            replyParameters: new()
                            {
                                AllowSendingWithoutReply = true,
                                MessageId = message.MessageId,
                            },
                            cancellationToken: cancellationToken
                        );
                    }
                    break;

                case "show":
                    {
                        var name = args.ElementAtOrDefault(1);
                        if (string.IsNullOrEmpty(name))
                        {
                            goto default;
                        }

                        await client.SendChatActionAsync(
                            message.Chat.Id,
                            ChatActions.Typing,
                            cancellationToken: cancellationToken
                        );
                        var apiKeys = await apiKeyRespository.GetByUserIdAsync(
                            message.From!.Id,
                            cancellationToken
                        );
                        var value = apiKeys.FirstOrDefault(k => k.Name == name)?.Value;
                        if (string.IsNullOrEmpty(value))
                        {
                            await client.SendMessageAsync(
                                message.Chat.Id,
                                localizer["NotFound"],
                                parseMode: FormatStyles.HTML,
                                replyParameters: new()
                                {
                                    AllowSendingWithoutReply = true,
                                    MessageId = message.MessageId,
                                },
                                cancellationToken: cancellationToken
                            );
                        }
                        else
                        {
                            await client.SendMessageAsync(
                                message.Chat.Id,
                                localizer["ApiKeyDetail", value],
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
                    break;

                case "help":
                default:
                    {
                        await contextProvider.LoadAsync(message, cancellationToken);
                        await client.SendMessageAsync(
                            message.Chat.Id,
                            localizer["ApiKeyHelp"],
                            parseMode: FormatStyles.HTML,
                            linkPreviewOptions: new() { IsDisabled = true },
                            replyParameters: new()
                            {
                                AllowSendingWithoutReply = true,
                                MessageId = message.MessageId,
                            },
                            cancellationToken: cancellationToken
                        );
                    }
                    break;
            }
        }
    }
}
