using SauceNAO.Application.Resources;
using SauceNAO.Core.Models;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.Application.Features.ApiKey;

[BotCommand("apikey", "Manage your apikeys to use the bot")]
[LocalizedBotCommand("es", "Gestiona tus apikeys para usar el bot")]
[BotCommandVisibility(BotCommandVisibility.PrivateChats)]
class ApiKeyCommand(
    ITelegramBotClient client,
    IBotHelper helper,
    IBetterStringLocalizer<ApiKeyCommand> localizer,
    IUserStateManager manager,
    IEnumerable<IUserStateHandler> stateHandlers
) : IBotCommandHandler
{
    private string HelpMessage => localizer["ApiKeyHelp"];
    private string ListMessage => localizer["ApiKeyList"];
    private string ListEmptyMessage => localizer["ApiKeyListEmpty"];
    private string DetailMessage => localizer["ApiKeyDetail"];

    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        // Process the command only in private chats.
        if (message.Chat.Type == ChatTypes.Private)
        {
            // Send a message indicating that the bot is processing the command.
            client.SendChatAction(message.Chat.Id, ChatActions.Typing);
            var (user, _, languageCode) = await helper.UpsertDataFromMessageAsync(
                message,
                cancellationToken
            );
            if (!string.IsNullOrEmpty(languageCode))
            {
                localizer.ChangeCulture(languageCode);
            }

            var action = args.FirstOrDefault() ?? "help";
            switch (action)
            {
                case ApiKeyStateHandler.ADD_VALUE:

                    // Initialize the user state.
                    var initialData = new Dictionary<string, string?>()
                    {
                        { ApiKeyStateHandler.ACTION_PARAM_NAME, ApiKeyStateHandler.ADD_VALUE },
                        { ApiKeyStateHandler.NAME_PARAM_NAME, args.ElementAtOrDefault(1) },
                        { ApiKeyStateHandler.API_KEY_PARAM_NAME, args.ElementAtOrDefault(2) },
                        { ApiKeyStateHandler.IS_PUBLIC_PARAM_NAME, args.ElementAtOrDefault(3) },
                    };

                    // If the third argument is provided, change it to a boolean.
                    if (initialData[ApiKeyStateHandler.IS_PUBLIC_PARAM_NAME] is string isPublic)
                    {
                        initialData[ApiKeyStateHandler.IS_PUBLIC_PARAM_NAME] = (
                            isPublic == "true" || isPublic == "--public"
                        )
                            .ToString()
                            .ToLowerInvariant();
                    }

                    await this.CreateStateAndContinueAsync(initialData, message, cancellationToken);
                    break;

                case ApiKeyStateHandler.DELETE_VALUE:
                case "remove":
                    if (
                        args.Length != 2
                        || !user.ApiKeys.Any(a => a.Name == args[1] || a.Value == args[1])
                    )
                    {
                        goto default;
                    }

                    await this.CreateStateAndContinueAsync(
                        new Dictionary<string, string?>
                        {
                            {
                                ApiKeyStateHandler.ACTION_PARAM_NAME,
                                ApiKeyStateHandler.DELETE_VALUE
                            },
                            { ApiKeyStateHandler.NAME_PARAM_NAME, args[1] },
                        },
                        message,
                        cancellationToken
                    );
                    break;

                case "list":

                    var text =
                        user.ApiKeys.Count == 0
                            ? this.ListEmptyMessage
                            : string.Format(
                                this.ListMessage,
                                string.Join(
                                    "\n",
                                    user.ApiKeys.Select(a => $"<code>{a.Name}</code>")
                                )
                            );
                    await client.SendMessageAsync(
                        message.Chat.Id,
                        text,
                        parseMode: FormatStyles.HTML,
                        replyParameters: new ReplyParameters
                        {
                            MessageId = message.MessageId,
                            AllowSendingWithoutReply = true,
                        },
                        linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                        cancellationToken: cancellationToken
                    );
                    break;

                case "show":
                    if (args.Length != 2)
                    {
                        goto default;
                    }
                    var apikey =
                        user.ApiKeys.FirstOrDefault(a => a.Name == args[1])
                        ?? throw new ApiKeyException(
                            message,
                            ApiKeyErrorType.MissingApiKey,
                            languageCode
                        );
                    await client.SendMessageAsync(
                        message.Chat.Id,
                        string.Format(this.DetailMessage, apikey.Value),
                        parseMode: FormatStyles.HTML,
                        replyParameters: new ReplyParameters
                        {
                            MessageId = message.MessageId,
                            AllowSendingWithoutReply = true,
                        },
                        linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                        cancellationToken: cancellationToken
                    );
                    break;
                default:
                    await client.SendMessageAsync(
                        message.Chat.Id,
                        this.HelpMessage,
                        parseMode: FormatStyles.HTML,
                        replyParameters: new ReplyParameters
                        {
                            MessageId = message.MessageId,
                            AllowSendingWithoutReply = true,
                        },
                        linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                        cancellationToken: cancellationToken
                    );
                    break;
            }
        }
    }

    private async Task CreateStateAndContinueAsync(
        IDictionary<string, string?> initialData,
        Message message,
        CancellationToken cancellationToken
    )
    {
        var state = new UserState(
            ApiKeyStateHandler.STATE_SCOPE,
            message.Chat.Id,
            message.From!.Id,
            initialData
        );

        // Empty the text of the message to continue the process.
        message.Text = null;

        // Save the user state.
        await manager.CreateOrUpdateAsync(state, cancellationToken);

        foreach (var handler in stateHandlers)
        {
            if (handler.CanHandleState(state))
            {
                await handler.ContinueAsync(message, state, cancellationToken);
                break;
            }
        }
    }
}
