// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.App.Resources;
using SauceNAO.App.Services;
using SauceNAO.Core.Data;
using SauceNAO.Core.Exceptions.ApiKeys;
using SauceNAO.Core.Exceptions.Sauce;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.App.Features.ApiKeys;

sealed class AddApiKeyHandler(
    IStateManager stateManager,
    IContextProvider contextProvider,
    IApiKeyRespository apiKeyRepository,
    ISauceNAOClient snao,
    IBetterStringLocalizer<AddApiKeyHandler> localizer,
    ITelegramBotClient client
) : UserStateHandler<AddApiKeyState>(stateManager)
{
    protected override async Task ContinueAsync(
        AddApiKeyState state,
        Message message,
        CancellationToken cancellationToken
    )
    {
        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );

        var user = await contextProvider.LoadAsync(message, cancellationToken);
        string? input = message.Text;
        var existingApiKeys = await apiKeyRepository.GetByUserIdAsync(
            message.From!.Id,
            cancellationToken
        );

        // Request name
        var nameWasEmpty = false;
        if (string.IsNullOrEmpty(state.Name))
        {
            nameWasEmpty = true;
            if (string.IsNullOrEmpty(input))
            {
                await client.SendMessageAsync(
                    message.Chat.Id,
                    localizer["AskForName"],
                    parseMode: FormatStyles.HTML,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true,
                    },
                    replyMarkup: new ForceReply(),
                    cancellationToken: cancellationToken
                );
                return;
            }

            state.Name = input;
            input = null;
        }

        if (!state.IsNameValidated)
        {
            if (existingApiKeys.Any(k => k.Name == state.Name))
            {
                // If the name was set via the initial command instead of the assistant, clear it
                if (!nameWasEmpty)
                {
                    state.Name = null;
                    await this.SaveStateAsync(message, state, cancellationToken);
                }
                throw new ApiKeyNameAlreadyExistsException(message);
            }
            state.IsNameValidated = true;
            await this.SaveStateAsync(message, state, cancellationToken);
        }

        // Request API key
        var apiKeyWasEmpty = false;
        if (string.IsNullOrEmpty(state.Value))
        {
            apiKeyWasEmpty = true;
            if (string.IsNullOrEmpty(input))
            {
                await client.SendMessageAsync(
                    message.Chat.Id,
                    localizer["AskForKey"],
                    parseMode: FormatStyles.HTML,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true,
                    },
                    replyMarkup: new ForceReply(),
                    cancellationToken: cancellationToken
                );
                return;
            }

            state.Value = input;
            input = null;
        }

        if (!state.IsValueValidated)
        {
            try
            {
                if (existingApiKeys.Any(k => k.Value == state.Value))
                {
                    // If the API key was set from initial command instead of the assistant, clear it
                    if (!apiKeyWasEmpty)
                    {
                        state.Value = null;
                        await this.SaveStateAsync(message, state, cancellationToken);
                    }
                    throw new ApiKeyAlreadyExistsException(message);
                }
                var isPremium = await snao.IsPremiumAsync(state.Value, cancellationToken);
                if (!isPremium)
                {
                    // If the API key was set from initial command instead of the assistant, clear it
                    if (!apiKeyWasEmpty)
                    {
                        state.Value = null;
                        await this.SaveStateAsync(message, state, cancellationToken);
                    }
                    throw new ApiKeyNotPremiumException(message);
                }
                state.IsValueValidated = true;
                await this.SaveStateAsync(message, state, cancellationToken);
            }
            catch (SauceNAOException exp)
            {
                if (exp is not InvalidApiKeyException)
                {
                    await this.ClearStateAsync(message, cancellationToken);
                }
                // If the API key was set from initial command instead of the assistant, clear it
                else if (!apiKeyWasEmpty)
                {
                    state.Value = null;
                    await this.SaveStateAsync(message, state, cancellationToken);
                }
                throw new ApiKeyValidationException(message, exp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Ask if the API key will be public or private
        if (state.IsPublic is null)
        {
            var yesLabel = localizer["Yes"];
            var noLabel = localizer["No"];
            askAgain:
            if (string.IsNullOrEmpty(input))
            {
                await client.SendMessageAsync(
                    message.Chat.Id,
                    localizer["AskForShare"],
                    parseMode: FormatStyles.HTML,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true,
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
                state.IsPublic = true;
            }
            else if (input == noLabel)
            {
                state.IsPublic = false;
            }
            else
            {
                input = null;
                goto askAgain;
            }
        }

        await apiKeyRepository.InsertAsync(
            new(state.Name, state.Value) { IsPublic = state.IsPublic.Value, Owner = user },
            cancellationToken
        );
        await this.ClearStateAsync(message, cancellationToken);
        await client.SendMessageAsync(
            message.Chat.Id,
            localizer["AddSuccess"],
            parseMode: FormatStyles.HTML,
            replyParameters: new ReplyParameters
            {
                MessageId = message.MessageId,
                AllowSendingWithoutReply = true,
            },
            cancellationToken: cancellationToken
        );
    }
}
