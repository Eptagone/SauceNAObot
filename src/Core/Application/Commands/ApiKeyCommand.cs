// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Application.Models;
using SauceNAO.Application.Services;
using SauceNAO.Domain.Entities.SauceAggregate;
using SauceNAO.Domain.Repositories;
using SauceNAO.Domain.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

/// <summary>
/// Represents a command to manage user API keys.
/// </summary>
/// <param name="client">The Telegram Bot API client.</param>
[TelegramBotCommand(COMMAND_NAME, "Manage your API keys.")]
[BotCommandVisibility(BotCommandVisibility.PrivateChat)]
class ApiKeyCommand(
    ITelegramBotClient client,
    IUserStateManager stateManager,
    ISauceNaoService sauceNao,
    IUserRepository userRepository
) : BotCommandStateHandlerBase
{
    internal const string COMMAND_NAME = "apikey";
    private const string ACTION_PARAM_NAME = "action";
    private const string ADD_VALUE = "add";
    private const string DELETE_VALUE = "delete";
    private const string API_KEY_PARAM_NAME = "apikey";
    private const string IS_PUBLIC_PARAM_NAME = "isPublic";
    private const string NAME_PARAM_NAME = "name";

    private string HelpMessage => this.Context.Localizer["ApiKeyHelp"];
    private string ListMessage => this.Context.Localizer["ApiKeyList"];
    private string ListEmptyMessage => this.Context.Localizer["ApiKeyListEmpty"];
    private string AskForKeyMessage => this.Context.Localizer["ApiKeyAskForKey"];
    private string AddNotPremiumMessage => this.Context.Localizer["ApiKeyAddNotPremium"];
    private string CouldNotBeAddedMessage => this.Context.Localizer["ApiKeyCouldNotBeAdded"];
    private string AskForShareMessage => this.Context.Localizer["ApiKeyAskForShare"];
    private string AskForKeyNameMessage => this.Context.Localizer["ApiKeyAskForName"];
    private string AddSuccessMessage => this.Context.Localizer["ApiKeyAddSuccess"];
    private string DetailMessage => this.Context.Localizer["ApiKeyDetail"];
    private string CouldNotBeFoundMessage => this.Context.Localizer["ApiKeyCouldNotBeFound"];
    private string ApiKeyDeletedMessage => this.Context.Localizer["ApiKeyDeleted"];
    private string ConfirmDeleteMessage => this.Context.Localizer["ApiKeyConfirmDelete"];
    private string NameNotAvailableMessage => this.Context.Localizer["ApiKeyNameNotAvailable"];
    private string ApiKeyAlreadyExistsMessage => this.Context.Localizer["ApiKeyAlreadyExists"];
    private string CancelMessage => this.Context.Localizer["CancelMessage"];

    private string YesLabel => this.Context.Localizer["Yes"];
    private string NoLabel => this.Context.Localizer["No"];

    /// <inheritdoc />
    protected override Task InvokeAsync(
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

            var action = args.FirstOrDefault() ?? "help";
            switch (action)
            {
                case ADD_VALUE:

                    // Initialize the user state.
                    var initialData = new Dictionary<string, string?>()
                    {
                        { ACTION_PARAM_NAME, ADD_VALUE },
                        { NAME_PARAM_NAME, args.ElementAtOrDefault(1) },
                        { API_KEY_PARAM_NAME, args.ElementAtOrDefault(2) },
                        { IS_PUBLIC_PARAM_NAME, args.ElementAtOrDefault(3) },
                    };

                    // If the third argument is provided, change it to a boolean.
                    if (initialData[IS_PUBLIC_PARAM_NAME] is string isPublic)
                    {
                        initialData[IS_PUBLIC_PARAM_NAME] = (
                            isPublic == "true" || isPublic == "--public"
                        )
                            .ToString()
                            .ToLowerInvariant();
                    }

                    return this.InitializeStateAsync(initialData, message, cancellationToken);

                case DELETE_VALUE:
                case "remove":
                    if (
                        args.Length != 2
                        || !this.User.ApiKeys.Any(a => a.Name == args[1] || a.Value == args[1])
                    )
                    {
                        goto default;
                    }

                    return this.InitializeStateAsync(
                        new Dictionary<string, string?>
                        {
                            { ACTION_PARAM_NAME, DELETE_VALUE },
                            { NAME_PARAM_NAME, args[1] },
                        },
                        message,
                        cancellationToken
                    );

                case "list":

                    var text =
                        this.User.ApiKeys.Count == 0
                            ? this.ListEmptyMessage
                            : string.Format(
                                this.ListMessage,
                                string.Join(
                                    "\n",
                                    this.User.ApiKeys.Select(a => $"<code>{a.Name}</code>")
                                )
                            );
                    return client.SendMessageAsync(
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

                case "show":
                    if (args.Length != 2)
                    {
                        goto default;
                    }
                    var apikey = this.User.ApiKeys.FirstOrDefault(a => a.Name == args[1]);
                    if (apikey is null)
                    {
                        return client.SendMessageAsync(
                            message.Chat.Id,
                            this.CouldNotBeFoundMessage,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true,
                            },
                            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                            cancellationToken: cancellationToken
                        );
                    }

                    return client.SendMessageAsync(
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
                default:
                    return client.SendMessageAsync(
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
            }
        }

        return Task.CompletedTask;
    }

    protected async Task InitializeStateAsync(
        IDictionary<string, string?> initialData,
        Message message,
        CancellationToken cancellationToken
    )
    {
        var state = new UserState(COMMAND_NAME, message.Chat.Id, message.From!.Id, initialData);

        // Empty the text of the message to continue the process.
        message.Text = null;

        // Save the user state.
        stateManager.CreateOrUpdateState(state);

        // Continue resolving the state.
        await this.ResolveStateAsync(state, message, cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task ResolveStateAsync(
        UserState userState,
        Message message,
        CancellationToken cancellationToken
    )
    {
        var action = userState.Data[ACTION_PARAM_NAME];
        var userResponse = message.Text;
        switch (action)
        {
            case ADD_VALUE:
                {
                    var name = userState.Data[NAME_PARAM_NAME];
                    var apikey = userState.Data[API_KEY_PARAM_NAME];
                    var isPublic = userState.Data[IS_PUBLIC_PARAM_NAME];

                    // If the name for the apikey is not defined, try to get it from the message.
                    if (string.IsNullOrEmpty(name))
                    {
                        // If the user did not respond, ask again.
                        if (string.IsNullOrEmpty(userResponse))
                        {
                            await client.SendMessageAsync(
                                userState.ChatId,
                                this.AskForKeyNameMessage,
                                parseMode: FormatStyles.HTML,
                                replyParameters: new ReplyParameters
                                {
                                    MessageId = message.MessageId,
                                    AllowSendingWithoutReply = true,
                                },
                                cancellationToken: cancellationToken
                            );
                            return;
                        }
                        // Otherwise, save the name.
                        else
                        {
                            name = userResponse;
                            // Clear the user response.
                            userResponse = null;
                        }
                    }

                    // If the name is already defined, ask again.
                    if (this.User.ApiKeys.Any(x => x.Name == name))
                    {
                        await client.SendMessageAsync(
                            userState.ChatId,
                            this.NameNotAvailableMessage,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true,
                            },
                            cancellationToken: cancellationToken
                        );
                        // Make sure the duplicate name is not cached.
                        if (userState.Data.ContainsKey(NAME_PARAM_NAME))
                        {
                            userState.Data[NAME_PARAM_NAME] = null;
                            stateManager.CreateOrUpdateState(userState);
                        }
                        return;
                    }

                    // Back up the name.
                    userState.Data[NAME_PARAM_NAME] = name;
                    // Back up the user state.
                    stateManager.CreateOrUpdateState(userState);

                    // If the apikey is not defined, try to get it from the message.
                    if (string.IsNullOrEmpty(apikey))
                    {
                        apikey ??= userResponse;
                        // Clear the user response.
                        userResponse = null;
                    }

                    // If the message does not contain an apikey, ask for one again.
                    if (string.IsNullOrEmpty(apikey))
                    {
                        await client.SendMessageAsync(
                            userState.ChatId,
                            this.AskForKeyMessage,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true,
                            },
                            cancellationToken: cancellationToken
                        );
                        return;
                    }
                    // If the apikey already exists, inform the user and wait for a new one.
                    else if (this.User.ApiKeys.Any(x => x.Value == apikey))
                    {
                        await client.SendMessageAsync(
                            userState.ChatId,
                            this.ApiKeyAlreadyExistsMessage,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true,
                            },
                            cancellationToken: cancellationToken
                        );
                        // Make sure the duplicate apikey is not cached.
                        if (userState.Data.ContainsKey(API_KEY_PARAM_NAME))
                        {
                            userState.Data[API_KEY_PARAM_NAME] = null;
                            stateManager.CreateOrUpdateState(userState);
                        }
                        return;
                    }

                    var isValid = await sauceNao.IsPremiumUserAsync(apikey, cancellationToken);
                    // If the apikey could not be validated, cancel the operation.
                    if (isValid is null)
                    {
                        await client.SendMessageAsync(
                            userState.ChatId,
                            this.CouldNotBeAddedMessage,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true,
                            },
                            replyMarkup: new ReplyKeyboardRemove(),
                            cancellationToken: cancellationToken
                        );
                        // Remove the user state.
                        stateManager.RemoveState(userState);
                        return;
                    }

                    // If the apikey is not premium, cancel the operation.
                    if (isValid == false)
                    {
                        await client.SendMessageAsync(
                            userState.ChatId,
                            this.AddNotPremiumMessage,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true,
                            },
                            cancellationToken: cancellationToken
                        );
                        // Remove the user state.
                        stateManager.RemoveState(userState);
                        return;
                    }

                    // Back up the apikey in the user state if it was not defined before.
                    userState.Data[API_KEY_PARAM_NAME] ??= apikey;
                    // Back up the user state.
                    stateManager.CreateOrUpdateState(userState);

                    // If the user has not decided if share the apikey, check it their response or ask again.
                    if (isPublic is null)
                    {
                        noResponse:
                        // If the user did not respond, ask again.
                        if (string.IsNullOrEmpty(userResponse))
                        {
                            var keyboard = new ReplyKeyboardBuilder()
                                .AppendText(this.YesLabel)
                                .AppendText(this.NoLabel);
                            await client.SendMessageAsync(
                                userState.ChatId,
                                this.AskForShareMessage,
                                replyParameters: new ReplyParameters
                                {
                                    MessageId = message.MessageId,
                                    AllowSendingWithoutReply = true,
                                },
                                replyMarkup: new ReplyKeyboardMarkup(keyboard)
                                {
                                    ResizeKeyboard = true,
                                },
                                cancellationToken: cancellationToken
                            );
                            return;
                        }
                        // Otherwise, try to parse the user response.
                        else
                        {
                            isPublic = (userResponse == this.YesLabel)
                                .ToString()
                                .ToLowerInvariant();
                            if (isPublic == "false" && userResponse != this.NoLabel)
                            {
                                userResponse = null;
                                goto noResponse;
                            }

                            // Backu up the user decision.
                            userState.Data[IS_PUBLIC_PARAM_NAME] = isPublic;
                            // Clear the user response.
                            userResponse = null;
                        }
                    }

                    // Remove the user state.
                    stateManager.RemoveState(userState);

                    // Tell the user that the apikey has been added.
                    await client.SendMessageAsync(
                        userState.ChatId,
                        this.AddSuccessMessage,
                        replyParameters: new ReplyParameters
                        {
                            MessageId = message.MessageId,
                            AllowSendingWithoutReply = true,
                        },
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken
                    );
                    // Save the apikey in the database.
                    this.User.ApiKeys.Add(
                        new SauceApiKey(name, apikey) { IsPublic = isPublic == "true" }
                    );
                    await userRepository.UpdateAsync(this.User, cancellationToken);
                }
                break;
            case DELETE_VALUE:
                {
                    var name = userState.Data[NAME_PARAM_NAME]!;
                    // If the user has not responded, ask.
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        var keyboard = new ReplyKeyboardBuilder()
                            .AppendText(this.YesLabel)
                            .AppendText(this.NoLabel);
                        // Ask for confirmation.
                        await client.SendMessageAsync(
                            message.Chat.Id,
                            this.ConfirmDeleteMessage,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true,
                            },
                            replyMarkup: new ReplyKeyboardMarkup(keyboard)
                            {
                                ResizeKeyboard = true,
                            },
                            cancellationToken: cancellationToken
                        );
                    }

                    if (message.Text == this.YesLabel)
                    {
                        var apikey = this.User.ApiKeys.FirstOrDefault(x =>
                            x.Name == name || x.Name == message.Text
                        );

                        if (apikey is not null)
                        {
                            this.User.ApiKeys.Remove(apikey);
                            await userRepository.UpdateAsync(this.User, cancellationToken);
                            await client.SendMessageAsync(
                                userState.ChatId,
                                this.ApiKeyDeletedMessage,
                                replyParameters: new ReplyParameters
                                {
                                    MessageId = message.MessageId,
                                    AllowSendingWithoutReply = true,
                                },
                                replyMarkup: new ReplyKeyboardRemove(),
                                cancellationToken: cancellationToken
                            );
                        }

                        // Remove the user state.
                        stateManager.RemoveState(userState);
                    }
                    else if (message.Text == this.NoLabel)
                    {
                        await userRepository.UpdateAsync(this.User, cancellationToken);
                        await client.SendMessageAsync(
                            userState.ChatId,
                            this.CancelMessage,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true,
                            },
                            replyMarkup: new ReplyKeyboardRemove(),
                            cancellationToken: cancellationToken
                        );

                        // Remove the user state.
                        stateManager.RemoveState(userState);
                    }
                }

                break;
        }
    }
}
