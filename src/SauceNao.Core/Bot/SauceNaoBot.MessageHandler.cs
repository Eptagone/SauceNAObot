// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Globalization;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core
{
    public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
    {
        protected override async Task OnMessageAsync(Message message, CancellationToken cancellationToken)
        {
            // If the message was sent by Telegram (linked channels), ignore it.
            if (message.From.Id == 777000)
            {
                return;
            }
            // Save message instance
            base.Message = message ?? throw new ArgumentNullException(nameof(message));
            // Message has text
            var hasText = !string.IsNullOrEmpty(message.Text);
            // Message has caption
            var hasCaption = !hasText && !string.IsNullOrEmpty(message.Caption);
            // Is a private chat
            isPrivate = message.Chat.Type == ChatType.Private;

            // Local function: LoadData
            async Task LoadDataAsync()
            {
                // Load user data
                User = await db.Users.GetUserAsync(message.From, cancellationToken).ConfigureAwait(false);
                Date = DateTimeOffset.FromUnixTimeSeconds(message.Date).DateTime;
                if (isPrivate)
                {
                    // Set lang
                    Language = new CultureInfo(User.LanguageCode ?? "en");
                }
                else
                {
                    // Load group data
                    Group = await db.Groups.GetGroupAsync(message.Chat, cancellationToken).ConfigureAwait(false);
                    // Set lang
                    Language = new CultureInfo(User.LangForce ? User.LanguageCode ?? "en" : Group.LanguageCode ?? "en");
                }
            }

            // If chat is private and message is not text
            if (isPrivate && !hasText)
            {
                if (message.ViaBot?.Id != Me.Id)
                {
                    await LoadDataAsync().ConfigureAwait(false);
                    // New search
                    await SauceAsync(message, cancellationToken).ConfigureAwait(false);
                }
                return;
            }
            // Check if message constains text.
            if (hasText)
            {
                // If text == "sauce" or text is a metion for me (bot): Search
                if ((message.Text.ToLowerInvariant() == "sauce" || message.Text.Contains($"@{Me.Username}")) && message.ReplyToMessage != default && !message.Text.StartsWith('/'))
                {
                    await LoadDataAsync().ConfigureAwait(false); ;
                    await SauceAsync(message.ReplyToMessage, cancellationToken).ConfigureAwait(false);
                }
                // If message is a command: process command
                else
                {
                    var cmdMatch = Properties.CommandHelper.Match(message.Text);
                    if (cmdMatch.Success)
                    {
                        await LoadDataAsync().ConfigureAwait(false); ;
                        var parameters = string.IsNullOrEmpty(cmdMatch.Params) ? Array.Empty<string>() : cmdMatch.Params.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        await OnCommandAsync(cmdMatch.Name, parameters, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            // Check if message constains caption.
            else if (hasCaption)
            {
                var caption = message.Caption;
                var lowerCaption = caption.ToLowerInvariant();
                // If caption == "sauce" or caption is a metion for me (bot): Search
                if (lowerCaption == "sauce" || caption.Contains($"@{Me.Username}") || lowerCaption == "/sauce")
                {
                    await LoadDataAsync().ConfigureAwait(false); ;
                    // New Search
                    await SauceAsync(message, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
