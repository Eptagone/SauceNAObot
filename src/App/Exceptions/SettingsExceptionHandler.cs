// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.App.Resources;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;

namespace SauceNAO.App.Exceptions;

/// <summary>
/// Handles exceptions related to user/group settings
/// </summary>
sealed class SettingsExceptionHandler(
    IContextProvider contextProvider,
    ITelegramBotClient client,
    IBetterStringLocalizer<SettingsExceptionHandler> localizer
) : IMessageExceptionHandler
{
    public async Task<bool> TryHandleAsync(
        MessageException exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is InvalidLanguageCodeException ilce)
        {
            await contextProvider.LoadAsync(ilce.ReceivedMessage, cancellationToken);
            await client.SendMessageAsync(
                ilce.ReceivedMessage.Chat.Id,
                localizer["InvalidLanguageCode"],
                cancellationToken: cancellationToken
            );
            return true;
        }
        return false;
    }
}
