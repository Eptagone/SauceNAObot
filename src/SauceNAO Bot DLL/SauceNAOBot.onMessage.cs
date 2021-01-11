// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.BotAPI.Available_Types;

namespace SauceNAO
{
    public partial class SauceNAOBot
    {
        /// <summary>On message update</summary>
        /// <param name="message">Message</param>
        internal async Task OnMessage(Message message)
        {
            // If the message was sent by Telegram(linked channels), ignore it.
            if (message.From.Id == 777000)
            {
                return;
            }
            // Get user info
            var isprivate = message.Chat.Type == ChatType.Private;
            user = await GetUserData(message.From, isprivate).ConfigureAwait(false);
            // Get date
            date = message.Date;
            // Get languaje
            lang = new CultureInfo(string.IsNullOrEmpty(user.Lang) ? "en" : user.Lang);

            // If chat is private
            if (isprivate)
            {
                if(user == default)
                {

                }
                // If the message is not text
                if (string.IsNullOrEmpty(message.Text))
                {
                    // New search
                    await NewSearch(message).ConfigureAwait(false);
                    return;
                }
            }
            // Check if message constains text. Otherwise return.
            if (string.IsNullOrEmpty(message.Text))
            {
                return;
            }
            // Get chat data (group only). Otherwise return default.
            chat = await GetChatData(message.Chat).ConfigureAwait(false);
            // If text == "sauce" or text is a metion for me (bot): Search
            if (message.Text.ToLowerInvariant() == "sauce" || message.Text.StartsWith($"@{Me}"))
            {
                await Sauce(message).ConfigureAwait(false);
                return;
            }
            // If caption == "sauce" or caption is a metion for me (bot): Search
            if (!string.IsNullOrEmpty(message.Caption))
            {
                if (message.Caption.ToLowerInvariant() == "sauce" || message.Caption.Contains($"@{Me}"))
                {
                    // New Search
                    await NewSearch(message).ConfigureAwait(false);
                }
            }
            // If message is not a command: return
            if (!message.Text.StartsWith('/'))
            {
                return;
            }
            string[] args = message.Text.Split(' ');
            string cmd = args[0].TrimStart('/');
            args = args.Skip(1).Where(a => !string.IsNullOrEmpty(a)).ToArray();
            // If command has a mention
            if (cmd.Contains('@'))
            {
                // If command is not for me: return
                if (!cmd.Contains($"@{Me}"))
                {
                    return;
                }
                cmd = cmd.Replace($"@{Me}", string.Empty);
            }
            // Run command
            await OnCommand(message, cmd, args).ConfigureAwait(false);
        }
    }
}
