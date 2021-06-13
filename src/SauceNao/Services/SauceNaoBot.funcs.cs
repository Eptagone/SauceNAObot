// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNao.API.Models;
using SauceNao.Data.Models;
using SauceNao.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.UpdatingMessages;
using File = System.IO.File;

namespace SauceNao.Services
{
    public partial class SauceNaoBot : TelegramBotAsync
    {
        /// <summary>Get chat data. If chat is a private chat, return default.</summary>
        /// <param name="tchat">Chat</param>
        private async Task<AppChat> GetChatDataAsync(Chat tchat)
        {
            if (tchat.Type == "private")
            {
                return default;
            }
            var chat = await DB.Chats.FirstOrDefaultAsync(c => c.Id == tchat.Id).ConfigureAwait(false);
            if (chat == default)
            {
                ChatMember creator = default;
                try
                {
                    var admins = await Bot.GetChatAdministratorsAsync<IEnumerable<ChatMember>>(tchat.Id);
                    creator = admins.FirstOrDefault(a => a.Status == ChatMemberStatus.Creator);
                }
                finally
                {
                    chat = creator == default
                        ? new AppChat(tchat)
                        : new AppChat(tchat, creator.User.LanguageCode);
                }
                await DB.AddAsync(chat).ConfigureAwait(false);
                await DB.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                if (!chat.Equals(tchat))
                {
                    chat.Update(tchat);
                    DB.Entry(chat).State = EntityState.Modified;
                    await DB.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            return chat;
        }

        /// <summary>Get user data.</summary>
        /// <param name="tuser">Telegram user.</param>
        /// <param name="isPrivate">Is private.</param>
        private async Task<AppUser> GetUserDataAsync(User tuser, [Optional] bool isPrivate)
        {
            var user = await DB.Users.FirstOrDefaultAsync(u => u.Id == tuser.Id)
                .ConfigureAwait(false);
            if (user == default)
            {
                user = new AppUser(tuser, isPrivate);
                await DB.AddAsync(user).ConfigureAwait(false);
                await DB.SaveChangesAsync().ConfigureAwait(false);
                return user;
            }
            else
            {
                var start = !user.Start && isPrivate;
                if (!user.Equals(tuser) || start)
                {
                    user.Update(tuser, start);
                    await DB.SaveChangesAsync().ConfigureAwait(false);
                }
                return user;
            }
        }

        private async Task UpdateSearchMessageAsync(Message message, string newtext, [Optional] InlineKeyboardMarkup keyboard)
        {
            EditMessageTextArgs args = new()
            {
                ChatId = message.Chat.Id,
                MessageId = message.MessageId,
                Text = string.IsNullOrEmpty(newtext) ? "unknown name" : newtext,
                ParseMode = ParseMode.HTML,
                DisableWebPagePreview = true,
                ReplyMarkup = keyboard
            };

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
                else
                {
                    OnBotExceptionAsync(exp, default).Wait();
                }
            }
            catch (Exception exp)
            {
                OnExceptionAsync(exp, default).Wait();
            }
        }
        private bool TryGetFilePath(TargetMedia targetMedia, out string ext)
        {
            var okey = true;
            ext = string.Empty;
            try
            {
                var file = Bot.GetFile(targetMedia.TargetFileId);
                targetMedia.FilePath = string.Format(BotClient.BaseFilesUrl, Bot.Token, file.FilePath);
                ext = Path.GetExtension(file.FilePath);
            }
            catch (BotRequestException exp)
            {
                okey = false;
                OnBotExceptionAsync(exp, default).Wait();
            }
            return okey;
        }
        private bool TryDownload(TargetMedia target)
        {
            var okey = true;
            if (TryGetFilePath(target, out string ext))
            {
                var fileName = target.FileUniqueId + ext;
                var tempPath = $"{Path.GetTempPath()}{fileName}";
                var tempFile = DB.TemporalFiles.AsNoTracking().FirstOrDefault(f => f.FileUniqueId == target.FileUniqueId);
                if (File.Exists(tempPath))
                {
                    target.TemporalFilePath = string.Format(TempFilesPath, target.FileUniqueId);
                }
                else
                {
                    var fileBytes = Client.GetByteArrayAsync(target.FilePath).Result;
                    try
                    {
                        File.WriteAllBytes(tempPath, fileBytes);
                        target.TemporalFilePath = string.Format(TempFilesPath, target.FileUniqueId);
                    }
                    catch (IOException exp)
                    {
                        if (exp.Message == "There is not enough space on the disk")
                        {
                            //CleanUp();
                            Task.Delay(500);
                            File.WriteAllBytes(tempPath, fileBytes);
                            target.TemporalFilePath = string.Format(TempFilesPath, target.FileUniqueId);
                        }
                        else
                        {
                            okey = false;
                            OnExceptionAsync(exp, default).Wait();
                        }
                    }
                }
                if (okey && tempFile == default)
                {
                    tempFile = new TemporalFile(target.FileUniqueId, fileName, dateTime);
                    DB.Add(tempFile);
                    DB.SaveChanges();
                }
            }
            else
            {
                okey = false;
            }
            return okey;
        }
        private bool TryGetImageFromVideo(TargetMedia targetMedia)
        {
            if (string.IsNullOrEmpty(targetMedia.FilePath))
            {
                if (!TryGetFilePath(targetMedia, out _))
                {
                    return false;
                }
            }
            var fileName = $"{targetMedia.FileUniqueId}.jpg";
            string outputPath = $"{Path.GetTempPath()}{fileName}";
            var tempFile = DB.TemporalFiles.AsNoTracking().FirstOrDefault(f => f.FileUniqueId == targetMedia.FileUniqueId);
            // Si el archivo ya existe entonces regresar la ruta al mismo.
            if (File.Exists(outputPath))
            {
                targetMedia.TemporalFilePath = string.Format(TempFilesPath, targetMedia.FileUniqueId);
            }
            else
            {
                // Descargar archivo y guardar
                var ffmpegArgs = "-vf \"select=eq(n\\,0)\" -frames:v 1";
                if (ffmpeg.Run(targetMedia.FilePath, outputPath, ffmpegArgs))
                {
                    targetMedia.TemporalFilePath = string.Format(TempFilesPath, targetMedia.FileUniqueId);
                }
                else
                {
                    return false;
                }
            }
            if (tempFile == default)
            {
                tempFile = new TemporalFile(targetMedia.FileUniqueId, fileName, dateTime);
                DB.Add(tempFile);
                DB.SaveChanges();
            }
            return true;
        }
        private static async Task<Sauce> GetSauceAsync(string filePath)
        {
            Sauce sauce;
            try
            {
                var response = await SnaoService.SearchAsync(filePath);
                sauce = new Sauce(response);
            }
            catch (ResponseException exp)
            {
                sauce = new Sauce(exp);
            }
            return sauce;
        }
    }
}
