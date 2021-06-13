// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNao.Services
{
    public partial class SauceNaoBot : TelegramBotAsync
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override async Task OnMessageAsync(Message message, CancellationToken cancellationToken)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // If the message was sent by Telegram (linked channels), ignore it.
            if (message.From.Id == 777000)
            {
                return;
            }
            // Save message instance
            cMessage = message;
            // Message has text
            var hasText = !string.IsNullOrEmpty(message.Text);
            // Message has caption
            var hasCaption = !hasText && !string.IsNullOrEmpty(message.Caption);
            // Is a private chat
            cIsPrivate = message.Chat.Type == ChatType.Private;
            // Local function: LoadData
            async Task LoadDataAsync()
            {
                // Load user data
                cUser = await GetUserDataAsync(message.From, true);
                dateTime = DateTimeOffset.FromUnixTimeSeconds(message.Date).DateTime;
                if (cIsPrivate)
                {
                    // Set lang
                    cLang = new CultureInfo(cUser.Lang ?? "en");
                }
                else
                {
                    // Load group data
                    cGroup = await GetChatDataAsync(message.Chat);
                    // Set lang
                    cLang = new CultureInfo(cUser.LangForce ? cUser.Lang : cGroup.Lang ?? "en");
                }
            }
            // If chat is private and message is not text
            if (cIsPrivate && !hasText)
            {
                if (message.ViaBot == default || message.ViaBot.Id != Me.Id)
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
                if ((message.Text.ToLowerInvariant() == "sauce" || message.Text.StartsWith($"@{Me.Username}")) && message.ReplyToMessage != default)
                {
                    await LoadDataAsync().ConfigureAwait(false);
                    await SauceAsync(message.ReplyToMessage, cancellationToken).ConfigureAwait(false);
                }
                // If message is a command: process command
                else if (message.Text.StartsWith('/'))
                {
                    var splitText = message.Text.Split(' ');
                    var command = splitText.First();
                    var @params = splitText.Skip(1).ToArray();
                    var match = Regex.Match(command, cmdPattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        await LoadDataAsync().ConfigureAwait(false);
                        command = match.Groups.Values.Last().Value; // Get command name
                        await OnCommandAsync(command, @params, cancellationToken);
                    }
                }
            }
            // Check if message constains caption.
            else if (hasCaption)
            {
                // If caption == "sauce" or caption is a metion for me (bot): Search
                if (message.Caption.ToLowerInvariant() == "sauce" || message.Caption.Contains($"@{Me.Username}"))
                {
                    await LoadDataAsync().ConfigureAwait(false);
                    // New Search
                    await SauceAsync(message, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
