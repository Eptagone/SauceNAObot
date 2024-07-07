// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Globalization;
using Microsoft.Extensions.Localization;
using SauceNAO.Domain.Repositories;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

[TelegramBotCommand(
    "setlang",
    "Set your language preferences.",
    ["idioma", "setchatlang", "setlanguage", "setchatlanguage"]
)]
[BotCommandVisibility(BotCommandVisibility.PrivateChat | BotCommandVisibility.GroupAdmin)]
class SetLangCommand(
    ITelegramBotClient client,
    IUserRepository userRepository,
    IChatRepository chatRepository
) : BotCommandBase
{
    private IStringLocalizer? currentLocalizer;
    private IStringLocalizer Localizer => this.currentLocalizer ??= this.Context.Localizer;
    private string HelpMsg => this.Localizer["SetLangHelp"];
    private string SavedPreferencesMsg => this.Localizer["SetLangSaved"];
    private string NotAllowedMsg => this.Localizer["NotAllowed"];

    /// <inheritdoc />
    protected override async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        // Check if the user is in a private chat or in a group.
        var isPrivate = message.Chat.Type == ChatTypes.Private;

        // Read parameters
        var languageCode = args.ElementAtOrDefault(0);
        var forceArg = args.ElementAtOrDefault(1);
        var force = forceArg == "--force" || forceArg == "true";

        // If the language code was not passed or if it's not valid, send the default message.
        if (string.IsNullOrEmpty(languageCode) || !IsLanguageCodeValid(languageCode))
        {
            await client.SendMessageAsync(
                message.Chat.Id,
                this.HelpMsg,
                parseMode: FormatStyles.HTML,
                replyParameters: new ReplyParameters
                {
                    MessageId = message.MessageId,
                    AllowSendingWithoutReply = true
                },
                cancellationToken: cancellationToken
            );
            return;
        }

        // Send a message indicating that the bot is processing the command.
        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );

        // Set the language.
        if (isPrivate)
        {
            this.User.LanguageCode = languageCode;
            this.User.AlwaysUseOwnLanguage = force;
            this.currentLocalizer = new BotMessagesLocalizer(languageCode);
            await userRepository.UpdateAsync(this.User, cancellationToken);
        }
        else
        {
            // Check if the user is admin before setting the language.
            var admins = await client.GetChatAdministratorsAsync(
                message.Chat.Id,
                cancellationToken: cancellationToken
            );

            // If the user is not an admin, send an error message.
            if (!admins.Any(admin => admin.User.Id == message.From!.Id))
            {
                await client.SendMessageAsync(
                    message.Chat.Id,
                    this.NotAllowedMsg,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true
                    },
                    cancellationToken: cancellationToken
                );
                return;
            }

            this.Context.Group!.LanguageCode = languageCode;
            this.currentLocalizer = new BotMessagesLocalizer(languageCode);
            await chatRepository.UpdateAsync(this.Context.Group, cancellationToken);
        }

        // Send the success message.
        await client.SendMessageAsync(
            message.Chat.Id,
            this.SavedPreferencesMsg,
            parseMode: FormatStyles.HTML,
            replyParameters: new ReplyParameters
            {
                MessageId = message.MessageId,
                AllowSendingWithoutReply = true
            },
            cancellationToken: cancellationToken
        );
    }

    // Check if the language code is valid.
    private static bool IsLanguageCodeValid(string languageCode)
    {
        try
        {
            CultureInfo.GetCultureInfo(languageCode);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }
}
