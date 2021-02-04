// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.API;
using SauceNAO.Enumerators;
using SauceNAO.Models;
using SauceNAO.Resources;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using File = Telegram.BotAPI.AvailableTypes.File;
using IKB = Telegram.BotAPI.AvailableTypes.InlineKeyboardButton;
using IKM = Telegram.BotAPI.AvailableTypes.InlineKeyboardMarkup;
using SDIR = SauceNAO.Resources.SauceDirectory;

namespace SauceNAO
{
    public partial class SauceNAOBot
    {
        /// <summary>On bot command.</summary>
        /// <param name="message">Message</param>
        /// <param name="cmd">Command</param>
        /// <param name="args">Parameters</param>
        private async Task OnCommand(Message message, string cmd, string[] args)
        {
            switch (cmd)
            {
                case "anticheats":
                    await AntiCHeats(message, args).ConfigureAwait(false);
                    break;
                case "addex":
                    await Addex(message).ConfigureAwait(false);
                    break;
                case "delex":
                    await Delex(message).ConfigureAwait(false);
                    break;
                case "clean":
                    var clean = DB.History.Where(h => h.Id == user.Id);
                    DB.RemoveRange(clean);
                    await DB.SaveChangesAsync().ConfigureAwait(false);
                    await Bot.SendMessageAsync(message.Chat.Id, MSG.HistoryDelConfirm(lang), replyMarkup: new IKM(new IKB[] { IKB.SetCallbackData(MSG.Confirm(lang), "yes"), IKB.SetCallbackData(MSG.Cancel(lang), "no") })).ConfigureAwait(false);
                    break;
                case "help":
                    Help(message, args, lang);
                    break;
                case "mysauce":
                case "mysauces":
                    await Bot.SendMessageAsync(
                        message.Chat.Id, MSG.History(lang),
                        replyMarkup: new IKM(new IKB[] { IKB.SetSwitchInlineQueryCurrentChat("Sauce", string.Empty) })).ConfigureAwait(false);
                    break;
                case "sauce":
                case "source":
                case "saucenao":
                case "sourcenow":
                case "search":
                    await Sauce(message).ConfigureAwait(false);
                    break;
                case "start":
                    await Bot.SendMessageAsync(message.Chat.Id, MSG.About(lang), parseMode: ParseMode.HTML).ConfigureAwait(false);
                    if (!user.Start)
                    {
                        user.Start = true;
                        await DB.SaveChangesAsync().ConfigureAwait(false);
                    }
                    break;
                case "temp":
                    await Temp(message).ConfigureAwait(false);
                    break;
            }
        }

        public async Task Addex(Message message)
        {
            // If chat is private, return.
            if (chat == default)
            {
                return;
            }
            // Get chat member.
            var member = await Bot.GetChatMemberAsync(chat.Id, user.Id)
                .ConfigureAwait(false);
            // If member is Owner or Administrator, run command
            if (member.Status == "creator" || member.Status == "administrator")
            {
                if (message.ReplyToMessage != default)
                {
                    // Get target user
                    var tuser = message.ReplyToMessage.From;
                    // If target user is bot, add bot to exclude list
                    if (tuser.IsBot)
                    {
                        if (!DB.Whitelists.Any(w => w.ChatKey == chat.Key && w.Id == tuser.Id))
                        {
                            var ebot = new Models.Whitelist(tuser);
                            chat.Whitelist.Add(ebot);
                            await DB.SaveChangesAsync().ConfigureAwait(false);
                        }
                        await Bot.SendMessageAsync(chat.Id, MSG.AnticheatsAdded(lang, tuser.Username)).ConfigureAwait(false);
                    }
                }
            }
        }

        private async Task AntiCHeats(Message message, string[] args)
        {
            if (args.Length == 0)
            {
                await Bot.SendMessageAsync(message.Chat.Id, MSG.Anticheats(lang), replyToMessageId: message.MessageId)
                        .ConfigureAwait(false);
            }
            else
            {
                // If chat is private, return.
                if (chat == default)
                {
                    return;
                }
                // Get chat member.
                var member = await Bot.GetChatMemberAsync(chat.Id, user.Id)
                    .ConfigureAwait(false);
                // If member is Owner or Administrator, run command
                if (member.Status == "creator" || member.Status == "administrator")
                {
                    switch (args[0])
                    {
                        case "on":
                            chat.AntiCheats = true;
                            await Bot.SendMessageAsync(message.Chat.Id, MSG.AnticheatsOn(lang), replyToMessageId: message.MessageId)
                            .ConfigureAwait(false);
                            DB.Entry(chat).State = EntityState.Modified;
                            await DB.SaveChangesAsync()
                                .ConfigureAwait(false);
                            break;
                        case "off":
                            chat.AntiCheats = false;
                            await Bot.SendMessageAsync(message.Chat.Id, MSG.AnticheatsOff(lang), replyToMessageId: message.MessageId)
                            .ConfigureAwait(false);
                            DB.Entry(chat).State = EntityState.Modified;
                            await DB.SaveChangesAsync()
                                .ConfigureAwait(false);
                            break;
                        default:
                            await Bot.SendMessageAsync(message.Chat.Id, MSG.Anticheats(lang), replyToMessageId: message.MessageId)
                        .ConfigureAwait(false);
                            break;
                    }
                }
            }
        }

        public async Task Delex(Message message)
        {
            // If chat is private, return.
            if (chat == default)
            {
                return;
            }
            // Get chat member.
            var member = await Bot.GetChatMemberAsync(chat.Id, user.Id)
                .ConfigureAwait(false);
            // If member is Owner or Administrator, run command
            if (member.Status == "creator" || member.Status == "administrator")
            {
                if (message.ReplyToMessage != default)
                {
                    // Get target user
                    var tuser = message.ReplyToMessage.From;
                    // If target user is bot, remove bot from exclude list
                    if (tuser.IsBot)
                    {
                        await Bot.SendMessageAsync(chat.Id, MSG.AnticheatsDeleted(lang, tuser.Username)).ConfigureAwait(false);
                        // Get item
                        var item = await DB.Whitelists.FirstOrDefaultAsync(w => w.ChatKey == chat.Key && w.Id == tuser.Id).ConfigureAwait(false);
                        // If item exists, remove
                        if (item != default)
                        {
                            DB.Remove(item);
                            await DB.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        public static async void Help(Message message, string[] args, [Optional] CultureInfo lang)
        {
            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing).ConfigureAwait(false);
            if (args.Length != 0)
            {
                if (args.Length > 1)
                {
                    goto helpdefault;
                }
                else
                {
                    switch (args[0])
                    {
                        case "anticheats":
                            await Bot.SendMessageAsync(message.Chat.Id, MSG.Anticheats(lang), replyToMessageId: message.MessageId)
                                .ConfigureAwait(false);
                            break;
                        default:
                            goto helpdefault;
                    }
                }
                return;
            }
        helpdefault:
            await Bot.SendMessageAsync(message.Chat.Id, MSG.Help(lang), parseMode: ParseMode.HTML, replyToMessageId: message.MessageId)
            .ConfigureAwait(false);
        }

        private async Task Sauce(Message message)
        {
            // Check Anticheats
            if (chat != default)
            {
                if (message.ReplyToMessage != default)
                {
                    if (message.ReplyToMessage.From.IsBot)
                    {
                        var whitelist = DB.Whitelists.Where(w => w.ChatKey == chat.Key);
                        // If AntiCHeats On and bot is not on exclude list, return
                        if (chat.AntiCheats && !whitelist.Any(b => b.Id == message.ReplyToMessage.From.Id))
                        {
                            await Bot.SendMessageAsync(message.Chat.Id, MSG.AnticheatsMessage(lang), replyToMessageId: message.MessageId)
                                .ConfigureAwait(false);
                            return;
                        }
                    }
                }
            }
            // If reply message is null
            if (message.ReplyToMessage == null)
            {
                await Bot.SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(lang), replyToMessageId: message.MessageId)
                    .ConfigureAwait(false);
            }
            else // reply message is not null
            {
                await NewSearch(message.ReplyToMessage).ConfigureAwait(false);
            }
        }

        private async Task NewSearch(Message message)
        {
            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing)
                .ConfigureAwait(false);
            // Get target media
            TargetFile target = Utilities.GetMedia(message);
            // if target don't have media send message about a empty request
            if (!target.HasMedia)
            {
                await Bot
                    .SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(lang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true)
                    .ConfigureAwait(false);
                return;
            }
            // if target have media but target is a not a valid media, send message about a invalid media
            if (!target.Ok)
            {
                await Bot
                    .SendMessageAsync(message.Chat.Id, MSG.InvalidPhoto(lang), allowSendingWithoutReply: true, replyToMessageId: message.MessageId)
                    .ConfigureAwait(false);
                return;
            }
            // Send message about a processing request
            Message output = await Bot
                .SendMessageAsync(message.Chat.Id, MSG.Searching(lang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true)
                .ConfigureAwait(false);

            string tfilepath;
            string filename;
            try
            {
                File file = await Bot
                    .GetFileAsync(target.FileId)
                    .ConfigureAwait(false);
                tfilepath = string.Format(
                    BotClient.BaseFilesUrl,
                    Bot.Token,
                    file.FilePath);
                filename = Path
                    .GetFileName(file.FilePath);
            }
            catch (BotRequestException exp)
            {
                if (exp.Message == "Bad Request: file is too big")
                {
                    await UpdateSearchMessage(output, MSG.TooBigFile(lang))
                        .ConfigureAwait(false);
                }
                return;
            }
            // Check if this file has already been searched and recover the sauce information.
            var result = await DB.Searches.FirstOrDefaultAsync(s => s.File == target.UniqueId).ConfigureAwait(false);
            if (result != default)
            {
                // If sauce date is not too old, recover
                if ((date - result.SearchDate) < 36288000) // 36288000 == 1 week
                {
                    if (result.Okey)
                    {
                        var urls = result.Data;
                        await UpdateSearchMessage(output, result.ReadInfo(lang), new IKM(Utilities.ButtonsFromUrls(urls))).ConfigureAwait(false);
                        // Update user's search history
                        var usersauce = await DB.History.FirstOrDefaultAsync(h => h.Id == user.Id && h.SauceFile == result.File).ConfigureAwait(false);
                        if (usersauce == default)
                        {
                            usersauce = new UserSauce(result, date);
                            user.History.Add(usersauce);
                        }
                        else
                        {
                            usersauce.Date = date;
                            DB.Entry(usersauce).State = EntityState.Modified;
                        }
                        await DB.SaveChangesAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        string url = result.TmpUrl;
                        await UpdateSearchMessage(output, MSG.NotFound(url, lang))
                            .ConfigureAwait(false);
                    }
                    return;
                }
                else
                {
                    var oldhistory = DB.History.Where(h => h.SauceFile == result.File);
                    DB.RemoveRange(oldhistory);
                    DB.Remove(result);
                    await DB.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            // Continue
            string tmpurl = string.Empty;
            string ext = Path.GetExtension(filename);
            // If file is a video, it will be converted to a image
            if (!target.IsThumb && (ext == ".mp4" || ext == ".avi" || ext == ".mkv" || message.Animation != null || message.Video != null))
            {
                // while convert
                try
                {
                    tmpurl = await FFmpeg
                        .VideoToImage(tfilepath, target.UniqueId)
                        .ConfigureAwait(false);
                }
                catch (Exception exp)
                {
                    await UpdateSearchMessage(output, MSG.FailedConvertFile(lang))
                        .ConfigureAwait(false);
                    await OnException(exp, message, "An exception occurred while converting a file.").ConfigureAwait(false);
                }
                if (string.IsNullOrEmpty(tmpurl))
                {
                    await UpdateSearchMessage(output, MSG.FailedConvertFile(lang))
                        .ConfigureAwait(false);
                    return;
                }
                tfilepath = tmpurl;
            }
            // Get SNAO Service
            SauceNAOService snao = new SauceNAOService(PublicApiKey);
            // Search
            var sauce = await snao.SearchAsync(tfilepath)
                .ConfigureAwait(false);
            // If results are found send results
            switch (sauce.Status)
            {
                case StatusResult.Found:
                    // set data
                    IKM data = new IKM(Utilities.ButtonsFromUrls(sauce.Data));
                    // Finish search
                    sauce.Finish(target, date);
                    await UpdateSearchMessage(output, sauce.ReadInfo(lang), data)
                        .ConfigureAwait(false);
                    // save sauce to db
                    await DB.AddAsync(sauce).ConfigureAwait(false);
                    await DB.SaveChangesAsync().ConfigureAwait(false);
                    // Update user's search history
                    var usersauce = await DB.History.FirstOrDefaultAsync(h => h.Id == user.Id && h.SauceFile == sauce.File).ConfigureAwait(false);
                    if (usersauce == default)
                    {
                        usersauce = new UserSauce(sauce, date);
                        user.History.Add(usersauce);
                    }
                    else
                    {
                        usersauce.Date = date;
                        DB.Entry(usersauce).State = EntityState.Modified;
                    }
                    await DB.SaveChangesAsync().ConfigureAwait(false);
                    break;
                case StatusResult.NotFound:
                    if (string.IsNullOrEmpty(tmpurl))
                    {
                        string tmpname = $"{target.UniqueId}{ext}";
                        if (System.IO.File.Exists($"{Path.GetTempPath()}{tmpname}"))
                        {
                            tmpurl = string.Format(Utilities.TempFilesPath, tmpname);
                        }
                        else
                        {
                            byte[] filedata = await Utilities
                                .DownloadFileAsync(new Uri(tfilepath))
                                .ConfigureAwait(false);
                            tmpurl = await Utilities
                                .GenerateTmpFile(tmpname, filedata)
                                .ConfigureAwait(false);
                        }
                    }
                    string text = MSG.NotFound(tmpurl, lang);
                    await UpdateSearchMessage(output, text)
                        .ConfigureAwait(false);
                    //sauce.Finish(target, tmpurl, date);
                    //DB.Add(sauce);
                    //await DB.SaveChangesAsync().ConfigureAwait(false);
                    break;
                case StatusResult.Error:
                case StatusResult.BadRequest:
                case StatusResult.SauceError:
                    // Busy
                    await UpdateSearchMessage(output, MSG.Busy(lang))
                        .ConfigureAwait(false);
                    // Process Error or BadRequest
                    if (sauce.Status == StatusResult.Error)
                    {
                        await OnException(sauce.Response.Exception, message, "Sauce Error").ConfigureAwait(false);
                    }
                    else if (sauce.Status == StatusResult.BadRequest)
                    {
                        if (sauce.Response.StatusCode != HttpStatusCode.TooManyRequests)
                        {
                            var report = new Report(message, "Sauce BadRequest.", $"HttpRequestFailed [{sauce.Response.StatusCode}]: {sauce.Message}");
                            await DB.AddAsync(report).ConfigureAwait(false);
                            await DB.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        if (!(sauce.Message.Contains("Daily Search Limit Exceeded") || sauce.Message.Contains("Too many failed search attempts") || sauce.Message.Contains("Search Rate Too High")))
                        {
                            var report = new Report(message, "SauceNAO say: ", sauce.Message);
                            await DB.AddAsync(report).ConfigureAwait(false);
                            await DB.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                    break;
            }
        }

        private async Task Temp(Message message)
        {
            // Check Anticheats
            if (chat != default)
            {
                if (message.ReplyToMessage.From.IsBot)
                {
                    var whitelist = DB.Whitelists.Where(w => w.ChatKey == chat.Key);
                    // If AntiCHeats On and bot is not on exclude list, return
                    if (chat.AntiCheats && !whitelist.Any(b => b.Id == message.ReplyToMessage.From.Id))
                    {
                        await Bot.SendMessageAsync(message.Chat.Id, MSG.AnticheatsMessage(lang), replyToMessageId: message.MessageId)
                            .ConfigureAwait(false);
                        return;
                    }

                }
            }
            await TempGen(message, lang).ConfigureAwait(false);
        }
        private async Task TempGen(Message message, CultureInfo lang)
        {
            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing)
                        .ConfigureAwait(false);
            if (message.ReplyToMessage == null)
            {
                await Bot.SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(lang), ParseMode.HTML, replyToMessageId: message.MessageId, allowSendingWithoutReply: true).ConfigureAwait(false);
                return;
            }
            TargetFile target = Utilities.GetMedia(message.ReplyToMessage);
            if (!target.HasMedia)
            {
                await Bot.SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(lang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true).ConfigureAwait(false);
                return;
            }
            if (target.HasMedia && !target.Ok)
            {
                await Bot.SendMessageAsync(message.Chat.Id, MSG.InvalidPhoto(lang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true).ConfigureAwait(false);
                return;
            }
            string tmpurl = string.Empty;
            Message output = await Bot.SendMessageAsync(message.Chat.Id, MSG.GeneratingTmpUrl(lang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true).ConfigureAwait(false);
            try
            {
                File file = await Bot.GetFileAsync(target.FileId).ConfigureAwait(false);
                string origin = string.Format(BotClient.BaseFilesUrl, Bot.Token, file.FilePath);
                string ext = Path.GetExtension(file.FilePath);
                if (!target.IsThumb && (ext == ".mp4" || ext == ".avi" || ext == ".mkv" || message.Animation != null || message.Video != null))
                {
                    try
                    {
                        tmpurl = await FFmpeg.VideoToImage(origin, target.UniqueId).ConfigureAwait(false);
                    }
                    catch (Exception exp)
                    {
                        await UpdateSearchMessage(output, MSG.FailedConvertFile(lang)).ConfigureAwait(false);
                        await OnException(exp, message, "An exception occurred while converting a file.").ConfigureAwait(false);
                    }
                    if (string.IsNullOrEmpty(tmpurl))
                    {
                        await UpdateSearchMessage(output, MSG.FailedConvertFile(lang)).ConfigureAwait(false);
                        return;
                    }
                }
                else
                {
                    string filename = $"{file.FileUniqueId}{Path.GetExtension(file.FilePath)}";
                    tmpurl = await Utilities.GenerateTmpFile(filename, origin).ConfigureAwait(false);
                }
            }
            catch (BotRequestException exp)
            {
                if (exp.Message == "Bad Request: file is too big")
                {
                    await UpdateSearchMessage(output, MSG.TooBigFile(lang)).ConfigureAwait(false);
                }
                return;
            }
            string text = MSG.TmpUrlDone(lang, tmpurl);
            await UpdateSearchMessage(output, text, new IKM(new IKB[] { IKB.SetUrl("Google", string.Format(SDIR.GoogleImageSearch, tmpurl)), IKB.SetUrl("Yandex", string.Format(SDIR.YandexUrl, tmpurl)) }, new IKB[] { IKB.SetUrl("SauceNAO", string.Format(SDIR.SauceNAOsearch, tmpurl)) })).ConfigureAwait(false);
        }
    }
}
