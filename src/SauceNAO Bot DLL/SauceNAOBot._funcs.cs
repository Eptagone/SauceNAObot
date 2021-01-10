// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SauceNAO.Models;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.Available_Types;
using Telegram.BotAPI.Updating_messages;

namespace SauceNAO
{
    public partial class SauceNAOBot
    {
        /// <summary>Get chat data. If chat is a private chat, return default.</summary>
        /// <param name="tchat">Chat</param>
        private async Task<AppChat> GetChatData(Chat tchat)
        {
            if (tchat.Type == "private")
            {
                return default;
            }
            var chat = await DB.Chats.FirstOrDefaultAsync(c => c.Id == tchat.Id)
                .ConfigureAwait(false);
            if (chat == default)
            {
                chat = new AppChat(tchat);
                await DB.AddAsync(chat).ConfigureAwait(false);
                await DB.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                if (chat.NotEquals(tchat))
                {
                    DB.Entry(chat).State = EntityState.Modified;
                    await DB.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            return chat;
        }
        /// <summary>Get user data.</summary>
        /// <param name="tuser">Telegram user.</param>
        private async Task<AppUser> GetUserData(User tuser)
        {
            var user = await DB.Users.FirstOrDefaultAsync(u => u.Id == tuser.Id)
                .ConfigureAwait(false);
            if (user == default)
            {
                user = new AppUser(tuser);
                await DB.AddAsync(user).ConfigureAwait(false);
                await DB.SaveChangesAsync().ConfigureAwait(false);
                return user;
            }
            else
            {
                if (user.NotEquals(tuser))
                {
                    DB.Entry(user).State = EntityState.Modified;
                    await DB.SaveChangesAsync().ConfigureAwait(false);
                }
                return user;
            }
        }
        /// <summary>Get user data.</summary>
        /// <param name="tuser">Telegram User.</param>
        /// <param name="isPrivate">Is private.</param>
        private async Task<AppUser> GetUserData(User tuser, bool isPrivate)
        {
            var user = await DB.Users.FirstOrDefaultAsync(u => u.Id == tuser.Id)
                .ConfigureAwait(false);
            if (user == default)
            {
                user = new AppUser(tuser, isPrivate);
                await DB.AddAsync(user).ConfigureAwait(false);
                return user;
            }
            else
            {
                var start = (user.Start == false) && (isPrivate == true);
                if (user.NotEquals(tuser) || start)
                {
                    user.Start = start ? start : user.Start;
                    DB.Entry(user).State = EntityState.Modified;
                    await DB.SaveChangesAsync().ConfigureAwait(false);
                }
                return user;
            }
        }

        internal async Task UpdateSearchMessage(Message message, string newtext, [Optional] InlineKeyboardMarkup keyboard)
        {
            EditMessageTextArgs args = new EditMessageTextArgs
            {
                Chat_id = message.Chat.Id,
                Message_id = message.Message_id,
                Text = string.IsNullOrEmpty(newtext) ? "unknown name" : newtext,
                Parse_mode = ParseMode.HTML,
                Disable_web_page_preview = true
            };
            if (keyboard != null)
            {
                args.Reply_markup = keyboard;
            }

            try
            {
                await Bot.EditMessageTextAsync<Message>(args).ConfigureAwait(false);
            }
            catch (BotRequestException exp)
            {
                if (exp.Message == "Bad Request: message must be non-empty")
                {
                    args.Text = "unknown name";
                    await Bot.EditMessageTextAsync<Message>(args).ConfigureAwait(false);
                }
            }
            catch (Exception exp)
            {
                await OnException(exp, message, $"An unknown error occurred while trying to update a message.\nNew text: {newtext}\nkeyboard: {JsonConvert.SerializeObject(keyboard)}").ConfigureAwait(false);
            }
        }
    }
}
