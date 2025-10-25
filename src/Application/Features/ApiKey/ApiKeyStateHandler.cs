using SauceNAO.Application.Resources;
using SauceNAO.Core.Entities.SauceAggregate;
using SauceNAO.Core.Models;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Features.ApiKey;

class ApiKeyStateHandler(
    ITelegramBotClient client,
    IBotHelper helper,
    IUserRepository userManager,
    IBetterStringLocalizer<ApiKeyStateHandler> localizer,
    ISauceNaoService sauceNao,
    IUserStateManager manager
) : IUserStateHandler
{
    public const string STATE_SCOPE = "apikey";
    public const string ACTION_PARAM_NAME = "action";
    public const string ADD_VALUE = "add";
    public const string DELETE_VALUE = "delete";
    public const string API_KEY_PARAM_NAME = "apikey";
    public const string IS_PUBLIC_PARAM_NAME = "isPublic";
    public const string NAME_PARAM_NAME = "name";

    private string AskForKeyMessage => localizer["ApiKeyAskForKey"];
    private string AskForShareMessage => localizer["ApiKeyAskForShare"];
    private string AskForKeyNameMessage => localizer["ApiKeyAskForName"];
    private string AddSuccessMessage => localizer["ApiKeyAddSuccess"];
    private string ApiKeyDeletedMessage => localizer["ApiKeyDeleted"];
    private string ConfirmDeleteMessage => localizer["ApiKeyConfirmDelete"];
    private string CancelMessage => localizer["CancelMessage"];
    private string YesLabel => localizer["Yes"];
    private string NoLabel => localizer["No"];

    public bool CanHandleState(UserState state) => state.Scope == "apikey";

    public async Task ContinueAsync(
        Message message,
        UserState state,
        CancellationToken cancellationToken
    )
    {
        var (user, _, languageCode) = await helper.UpsertDataFromMessageAsync(
            message,
            cancellationToken
        );
        if (!string.IsNullOrEmpty(languageCode))
        {
            localizer.ChangeCulture(languageCode);
        }

        var action = state.Data[ACTION_PARAM_NAME];
        var userResponse = message.Text;
        switch (action)
        {
            case ADD_VALUE:
                {
                    var name = state.Data[NAME_PARAM_NAME];
                    var apikey = state.Data[API_KEY_PARAM_NAME];
                    var isPublic = state.Data[IS_PUBLIC_PARAM_NAME];

                    // If the name for the apikey is not defined, try to get it from the message.
                    if (string.IsNullOrEmpty(name))
                    {
                        // If the user did not respond, ask again.
                        if (string.IsNullOrEmpty(userResponse))
                        {
                            await client.SendMessageAsync(
                                state.ChatId,
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
                    if (user.ApiKeys.Any(x => x.Name == name))
                    {
                        // Make sure the duplicate name is not cached.
                        if (state.Data.ContainsKey(NAME_PARAM_NAME))
                        {
                            state.Data[NAME_PARAM_NAME] = null;
                            await manager.CreateOrUpdateAsync(state, cancellationToken);
                        }

                        throw new ApiKeyException(
                            message,
                            ApiKeyErrorType.NameNotAvailable,
                            languageCode
                        );
                    }

                    // Back up the name.
                    state.Data[NAME_PARAM_NAME] = name;
                    // Back up the user state.
                    await manager.CreateOrUpdateAsync(state, cancellationToken);

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
                            state.ChatId,
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
                    else if (user.ApiKeys.Any(x => x.Value == apikey))
                    {
                        // Make sure the duplicate apikey is not cached.
                        if (state.Data.ContainsKey(API_KEY_PARAM_NAME))
                        {
                            state.Data[API_KEY_PARAM_NAME] = null;
                            await manager.CreateOrUpdateAsync(state, cancellationToken);
                        }

                        throw new ApiKeyException(
                            message,
                            ApiKeyErrorType.ApiKeyAlreadyExists,
                            languageCode
                        );
                    }

                    var isValid = await sauceNao.IsPremiumUserAsync(apikey, cancellationToken);
                    // If the apikey could not be validated, cancel the operation.
                    if (isValid is null)
                    {
                        // Remove the user state.
                        await manager.RemoveAsync(state, cancellationToken);

                        throw new ApiKeyException(
                            message,
                            ApiKeyErrorType.CouldNotBeAdded,
                            languageCode
                        );
                    }

                    // If the apikey is not premium, cancel the operation.
                    if (isValid == false)
                    {
                        // Remove the user state.
                        await manager.RemoveAsync(state, cancellationToken);
                        throw new ApiKeyException(
                            message,
                            ApiKeyErrorType.NotPremium,
                            languageCode
                        );
                    }

                    // Back up the apikey in the user state if it was not defined before.
                    state.Data[API_KEY_PARAM_NAME] ??= apikey;
                    // Back up the user state.
                    await manager.CreateOrUpdateAsync(state, cancellationToken);

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
                                state.ChatId,
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
                            state.Data[IS_PUBLIC_PARAM_NAME] = isPublic;
                            // Clear the user response.
                            userResponse = null;
                        }
                    }

                    // Remove the user state.
                    await manager.RemoveAsync(state, cancellationToken);

                    // Tell the user that the apikey has been added.
                    await client.SendMessageAsync(
                        state.ChatId,
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
                    user.ApiKeys.Add(
                        new SauceApiKey(name, apikey) { IsPublic = isPublic == "true" }
                    );
                    await userManager.UpdateAsync(user, cancellationToken);
                }
                break;
            case DELETE_VALUE:
                {
                    var name = state.Data[NAME_PARAM_NAME]!;
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
                        var apikey = user.ApiKeys.FirstOrDefault(x =>
                            x.Name == name || x.Name == message.Text
                        );

                        if (apikey is not null)
                        {
                            user.ApiKeys.Remove(apikey);
                            await userManager.UpdateAsync(user, cancellationToken);
                            await client.SendMessageAsync(
                                state.ChatId,
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
                        await manager.RemoveAsync(state, cancellationToken);
                    }
                    else if (message.Text == this.NoLabel)
                    {
                        await userManager.UpdateAsync(user, cancellationToken);
                        await client.SendMessageAsync(
                            state.ChatId,
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
                        await manager.RemoveAsync(state, cancellationToken);
                    }
                }

                break;
        }
    }
}
