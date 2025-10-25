// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Globalization;
using SauceNAO.Application.Resources;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.Application.Features.Languages;

[BotCommand(
    "setlang",
    "Set your language preferences.",
    ["idioma", "setchatlang", "setlanguage", "setchatlanguage"]
)]
[LocalizedBotCommand("es", "Establece tus preferencias de idioma")]
[BotCommandVisibility(BotCommandVisibility.PrivateChats | BotCommandVisibility.Administrators)]
class SetLangCommand(
    ITelegramBotClient client,
    IBetterStringLocalizer localizer,
    IBotHelper helper,
    IUserRepository userManager,
    IChatRepostory groupManager
) : IBotCommandHandler
{
    private string HelpMsg => localizer["SetLangHelp"];
    private string SavedPreferencesMsg => localizer["SetLangSaved"];

    /// <inheritdoc />
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        var isPrivate = message.Chat.Type == ChatTypes.Private;
        if (!isPrivate)
        {
            // Check if the user is admin before setting the language.
            var admins = await client.GetChatAdministratorsAsync(
                message.Chat.Id,
                cancellationToken: cancellationToken
            );

            // If the user is not an admin, send an error message.
            if (!admins.Any(admin => admin.User.Id == message.From!.Id))
            {
                return;
            }
        }

        // Send an action indicating that the bot is processing the command.
        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );

        var (user, group, currentUserLanguage) = await helper.UpsertDataFromMessageAsync(
            message,
            cancellationToken
        );

        // Read parameters
        var languageCode = args.ElementAtOrDefault(0);
        var forceArg = args.ElementAtOrDefault(1);
        var force = forceArg == "--force" || forceArg == "true";

        // If the language code was not passed or if it's not valid, send a help message
        if (string.IsNullOrEmpty(languageCode) || !IsLanguageCodeValid(languageCode))
        {
            if (!string.IsNullOrEmpty(currentUserLanguage))
            {
                localizer.ChangeCulture(currentUserLanguage);
            }

            await client.SendMessageAsync(
                message.Chat.Id,
                this.HelpMsg,
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

        localizer.ChangeCulture(languageCode);
        // Save the new language to the user or group
        if (group is null)
        {
            user.LanguageCode = languageCode;
            user.AlwaysUseOwnLanguage = force;
            await userManager.UpdateAsync(user, cancellationToken);
        }
        else
        {
            group.LanguageCode = languageCode;
            await groupManager.UpdateAsync(group, cancellationToken);
        }

        // Send the success message.
        await client.SendMessageAsync(
            message.Chat.Id,
            this.SavedPreferencesMsg,
            parseMode: FormatStyles.HTML,
            replyParameters: new ReplyParameters
            {
                MessageId = message.MessageId,
                AllowSendingWithoutReply = true,
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
