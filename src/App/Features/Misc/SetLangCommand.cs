// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Globalization;
using SauceNAO.App.Resources;
using SauceNAO.Core.Data;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Features.Misc;

[
    BotCommand(
        "setlang",
        "Set your language preferences",
        ["setchatlang", "setlanguage", "setchatlanguage"]
    ),
    LocalizedBotCommand("es", "Establecer tus preferencias de idioma"),
    BotCommandVisibility(BotCommandVisibility.PrivateChats | BotCommandVisibility.Administrators),
]
sealed class SetLangCommand(
    IContextProvider contextProvider,
    ITelegramBotClient client,
    IUserRepository users,
    IChatRepository groups,
    IBetterStringLocalizer<SetLangCommand> localizer
) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        if (message.Chat.Type != ChatTypes.Private)
        {
            // Check if the user is admin before setting the language.
            var admins = await client.GetChatAdministratorsAsync(
                message.Chat.Id,
                cancellationToken: cancellationToken
            );

            // If the user is not an admin, ignore the command.
            if (!admins.Any(admin => admin.User.Id == message.From!.Id))
            {
                return;
            }
        }

        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );
        // Read parameters
        var languageCode = args.ElementAtOrDefault(0);
        if (languageCode is null)
        {
            await client.SendMessageAsync(
                message.Chat.Id,
                localizer["SetLangHelp"],
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
        if (!IsLanguageCodeValid(languageCode))
        {
            throw new InvalidLanguageCodeException(message, languageCode);
        }

        var (user, group) = await contextProvider.LoadAllAsync(message, cancellationToken);
        localizer.ChangeCulture(languageCode);
        if (group is not null)
        {
            group.LanguageCode = languageCode;
            await groups.UpdateAsync(group, cancellationToken);
        }
        else
        {
            user.LanguageCode = languageCode;
            user.UseFixedLanguage = true;
            await users.UpdateAsync(user, cancellationToken);
        }

        await client.SendMessageAsync(
            message.Chat.Id,
            localizer["LanguageSet"],
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
