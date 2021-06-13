// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNao.Data.Models;
using SauceNao.Enums;
using SauceNao.Models;
using SauceNao.Resources;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using IKB = Telegram.BotAPI.AvailableTypes.InlineKeyboardButton;
using IKM = Telegram.BotAPI.AvailableTypes.InlineKeyboardMarkup;
using SDIR = SauceNao.Resources.SauceDirectory;

namespace SauceNao.Services
{
    public partial class SauceNaoBot : TelegramBotAsync
    {
        private async Task OnCommandAsync(string cmd, string[] args, CancellationToken cancellationToken)
        {
            switch (cmd)
            {
                case "antitrampas":
                case "notrampas":
                case "anticheats":
                case "cheatcontrol":
                    await AntiCheatsAsync(args, cancellationToken).ConfigureAwait(false);
                    break;
                case "botcreator":
                case "creator":
                    await BotCreatorAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case "support":
                case "supportme":
                case "buymeacookie":
                case "buymeacoffee":
                    await BuyMeACookie(cancellationToken).ConfigureAwait(false);
                    break;
                case "help":
                case "ayuda":
                    await HelpAsync(args, cancellationToken).ConfigureAwait(false);
                    break;
                case "history":
                case "myhistory":
                case "mysauce":
                case "mysauces":
                case "mihistorial":
                case "historial":
                    await HistoryAsync(args, cancellationToken).ConfigureAwait(false);
                    break;
                case "stats":
                case "statistics":
                    await StatisticsAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case "sauce":
                case "source":
                case "saucenao":
                case "sourcenow":
                case "search":
                case "salsa":
                case "fuente":
                    await SauceAsync(cMessage.ReplyToMessage, cancellationToken).ConfigureAwait(false);
                    break;
                case "idioma":
                case "setlang":
                    await SetLangAsync(args, true, cancellationToken).ConfigureAwait(false);
                    break;
                case "setchatlang":
                case "setgrouplang":
                    await SetLangAsync(args, false, cancellationToken).ConfigureAwait(false);
                    break;
                case "temp":
                case "temporal":
                    await TempAsync(cMessage.ReplyToMessage, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }
        private async Task AntiCheatsAsync(string[] args, CancellationToken cancellationToken)
        {
            await Bot.SendChatActionAsync(cMessage.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

            void AntiCheatsDefault()
            {
                Bot.SendMessage(cMessage.Chat.Id, MSG.Anticheats(cLang), ParseMode.HTML, replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true);
            }

            if (cIsPrivate)
            {
                await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.GroupsOnly(cLang), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else if (args.Length == 0 || args.Length > 2)
            {
                AntiCheatsDefault();
            }
            else
            {
                var isNotReply = cMessage.ReplyToMessage == default;
                if (isNotReply && args.Length == 1)
                {
                    AntiCheatsDefault();
                    return;
                }
                AntiCheat antiCheat;
                switch (args[0])
                {
                    case "add":
                        antiCheat = args.Length == 2
                            ? new AntiCheat(args[1], cMessage.From.Id)
                            : cMessage.ReplyToMessage.From.IsBot
                                ? new AntiCheat(cMessage.ReplyToMessage.From, cMessage.From.Id)
                                : default;
                        if (antiCheat != default)
                        {
                            await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.AnticheatsAdded(cLang, antiCheat.Username), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                            if (!DB.AntiCheats.AsNoTracking().Any(a => a.Chat.Id == cGroup.Id && a.Username == antiCheat.Username))
                            {
                                cGroup.AntiCheats.Add(antiCheat);
                                await DB.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            AntiCheatsDefault();
                        }
                        break;
                    case "del":
                    case "rem":
                    case "remove":
                        var remUsername = args.Length == 2
                            ? args[1].TrimStart('@')
                            : cMessage.ReplyToMessage.From.Username;
                        antiCheat = DB.AntiCheats.FirstOrDefault(a => a.Chat.Id == cGroup.Id && a.Username == remUsername);
                        if (antiCheat != default)
                        {
                            await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.AnticheatsDeleted(cLang, antiCheat.Username), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                            DB.Remove(antiCheat);
                            await DB.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                        }
                        break;
                    default:
                        AntiCheatsDefault();
                        break;
                }
            }
        }
        private async Task BotCreatorAsync(CancellationToken cancellationToken)
        {
            await Bot.SendChatActionAsync(cMessage.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (cMessage.ReplyToMessage == default)
            {
                await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.Creator(cLang), ParseMode.HTML, replyToMessageId: cMessage.MessageId, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.Creator(cLang), ParseMode.HTML, replyToMessageId: cMessage.ReplyToMessage.MessageId, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
        private async Task BuyMeACookie(CancellationToken cancellationToken)
        {
            await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.BuyMeACookie(cLang), ParseMode.HTML, replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, replyMarkup: new IKM(new IKB[] { IKB.SetUrl(MSG.SupportChat(cLang), SupportChatUrl) }), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        private async Task HelpAsync(string[] args, CancellationToken cancellationToken)
        {
            await Bot.SendChatActionAsync(cMessage.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

            void HelpDefault()
            {
                Bot.SendMessage(cMessage.Chat.Id, MSG.Help(cLang), ParseMode.HTML, replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, replyMarkup: new IKM(new IKB[] { IKB.SetUrl(MSG.SupportChat(cLang), SupportChatUrl) }));
            }

            if (args.Length == 0 || args.Length > 1)
            {
                HelpDefault();
            }
            else
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "anticheats":
                    case "anti-cheats":
                    case "anticheat":
                    case "anti-cheat":
                    case "antitrampas":
                    case "cheatcontrol":
                        await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.Anticheats(cLang), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken);
                        break;
                    case "myhistory":
                    case "history":
                    case "historial":
                    case "mihistorial":
                        await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.History(cLang, Me.Username), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken);
                        break;
                    default:
                        HelpDefault();
                        break;
                }
            }
        }
        private async Task HistoryAsync(string[] args, CancellationToken cancellationToken)
        {
            await Bot.SendChatActionAsync(cMessage.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

            async Task HistoryDefault()
            {
                await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.History(cLang, Me.Username), parseMode: ParseMode.HTML, replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, replyMarkup: new IKM(new IKB[] { IKB.SetSwitchInlineQueryCurrentChat(MSG.HistoryButton(cLang), string.Empty) }), cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            if (args.Length != 1)
            {
                await HistoryDefault().ConfigureAwait(false);
            }
            else
            {
                switch (args[0])
                {
                    case "clear":
                    case "clean":
                    case "limpiar":
                    case "borrar":
                    case "vaciar":
                        await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.HistoryErased(cLang), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                        var userHistory = DB.UserSauces.AsNoTracking().Where(u => u.User.Id == cUser.Id);
                        DB.RemoveRange(userHistory);
                        await DB.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                        break;
                    default:
                        await HistoryDefault().ConfigureAwait(false);
                        break;
                }
            }
        }
        private async Task SauceAsync(Message message, CancellationToken cancellationToken)
        {
            if (message == default)
            {
                await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.EmptyRequest(cLang), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                return;
            }
            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!cIsPrivate && message.From.IsBot)
            {
                if (DB.AntiCheats.AsNoTracking().Any(b => b.Id == message.From.Id && b.ChatId == cGroup.ChatId))
                {
                    await Bot.SendMessageAsync(message.Chat.Id, MSG.AnticheatsMessage(cLang), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                    return;
                }
            }
            var targetMedia = new TargetMedia(message);
            if (targetMedia.Type == MediaType.Unknown)
            {
                await Bot.SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(cLang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else if (!targetMedia.IsValid)
            {
                await Bot.SendMessageAsync(message.Chat.Id, MSG.InvalidPhoto(cLang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var sauce = DB.SuccessfulSauces.AsNoTracking().FirstOrDefault(s => s.FileUniqueId == targetMedia.FileUniqueId);
                if (sauce == default)
                {
                    var output = await Bot.SendMessageAsync(message.Chat.Id, MSG.Searching(cLang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (!TryGetFilePath(targetMedia, out _))
                    {
                        await UpdateSearchMessageAsync(output, MSG.TooBigFile(cLang)).ConfigureAwait(false);
                    }
                    else
                    {
                        if (targetMedia.NeedConversion && !TryGetImageFromVideo(targetMedia))
                        {
                            await UpdateSearchMessageAsync(output, MSG.FailedConvertFile(cLang)).ConfigureAwait(false);
                        }
                        else
                        {
                            var sauceResult = await GetSauceAsync(targetMedia.TargetSearchPath);
                            switch (sauceResult.Status)
                            {
                                case SauceStatus.Found:
                                    await UpdateSearchMessageAsync(output, sauceResult.Info.GetInfo(cLang), sauceResult.Urls.ToInlineKeyboardMarkup()).ConfigureAwait(false);
                                    // save sauce to db and update user's search history
                                    sauce = new SuccessfulSauce(sauceResult, targetMedia, dateTime);
                                    await DB.AddAsync(sauce, cancellationToken).ConfigureAwait(false);
                                    cUser.UserSauces.Add(new UserSauce(sauce, dateTime));
                                    await DB.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                                    break;
                                case SauceStatus.NotFound:
                                    if (string.IsNullOrEmpty(targetMedia.TemporalFilePath) && !TryDownload(targetMedia))
                                    {
                                        await UpdateSearchMessageAsync(output, MSG.TooBigFile(cLang)).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UpdateSearchMessageAsync(output, MSG.NotFound(targetMedia.TemporalFilePath, cLang)).ConfigureAwait(false);
                                    }
                                    break;
                                case SauceStatus.Error:
                                case SauceStatus.BadRequest:
                                case SauceStatus.SauceError:
                                    await UpdateSearchMessageAsync(output, MSG.Busy(cLang)).ConfigureAwait(false);
                                    OnSauceError(sauceResult.Message);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    await Bot.SendMessageAsync(message.Chat.Id, sauce.GetInfo(cLang), ParseMode.HTML, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, replyMarkup: sauce.GetKeyboard(), cancellationToken: cancellationToken).ConfigureAwait(false);
                    var timeRange = sauce.Date - dateTime;
                    if (timeRange.TotalDays < 7)
                    {
                        DB.Remove(sauce);
                    }
                    else
                    {
                        var userSauce = DB.UserSauces.FirstOrDefault(s => s.UserId == cUser.Id && s.SauceId == sauce.Id);
                        if (userSauce == default)
                        {
                            userSauce = new UserSauce(sauce, dateTime);
                            cUser.UserSauces.Add(userSauce);
                        }
                        else
                        {
                            userSauce.Date = dateTime;
                        }
                    }
                    await DB.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }
        private async Task SetLangAsync(string[] args, bool isPrivate, CancellationToken cancellationToken)
        {
            bool CultureIsValid(string languajeCode)
            {
                var isValid = true;
                try
                {
                    _ = new CultureInfo(languajeCode);
                }
                catch
                {
                    isValid = false;
                }
                return isValid;
            }
            async Task SetLangDefault()
            {
                await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.SetLang(cLang), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            async Task SetlangSaved()
            {
                var lang = new CultureInfo((isPrivate || cGroup == default ? cUser.Lang : cGroup.Lang) ?? "en");
                await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.SetLangSaved(lang), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            if (args.Length > 0)
            {
                if (isPrivate)
                {
                    var setLang = args[0].ToLowerInvariant();
                    if (setLang.Length > 8 || !CultureIsValid(setLang))
                    {
                        await SetLangDefault().ConfigureAwait(false);
                    }
                    else
                    {
                        cUser.Lang = setLang;
                        if (args.Length == 2)
                        {
                            var force = args[1].ToLowerInvariant() switch
                            {
                                var f when
                                    f == "-force" ||
                                    f == "force" ||
                                    f == "-forzar" ||
                                    f == "forzar"
                                    => true,
                                _ => false,
                            };
                            if (force)
                            {
                                cUser.LangForce = true;
                            }
                            else
                            {
                                await SetLangDefault().ConfigureAwait(false);
                                return;
                            }
                        }
                        else
                        {
                            cUser.LangForce = false;
                        }
                        await SetlangSaved().ConfigureAwait(false);
                        await DB.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                else if (args.Length == 1)
                {
                    var setLang = args[0].ToLowerInvariant();
                    if (setLang.Length > 8 || !CultureIsValid(setLang))
                    {
                        await SetLangDefault().ConfigureAwait(false);
                    }
                    else
                    {
                        var admins = await Bot.GetChatAdministratorsAsync(cGroup.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
                        if (admins.Any(a => a.User.Id == cMessage.From.Id))
                        {
                            cGroup.Lang = setLang;
                            await SetlangSaved().ConfigureAwait(false);
                            await DB.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            await SetLangDefault().ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    await SetLangDefault().ConfigureAwait(false);
                }
            }
            else
            {
                await SetLangDefault().ConfigureAwait(false);
            }
        }
        private async Task StatisticsAsync(CancellationToken cancellationToken)
        {
            var oldDate = new DateTime(dateTime.Ticks).AddDays(-7);
            var sucefullSearchCount = DB.SuccessfulSauces.AsNoTracking().Where(s => s.Date > oldDate).Count();
            var usersCount = DB.Users.AsNoTracking().Count();
            var groupCount = DB.Chats.AsNoTracking().Count();
            var stats = MSG.Statistics(cLang, sucefullSearchCount, usersCount, groupCount);
            await Bot.SendMessageAsync(cMessage.Chat.Id, stats, ParseMode.HTML, replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        private async Task TempAsync(Message message, CancellationToken cancellationToken)
        {
            if (message == default)
            {
                await Bot.SendMessageAsync(cMessage.Chat.Id, MSG.EmptyRequest(cLang), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                return;
            }
            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!cIsPrivate && message.From.IsBot)
            {
                if (DB.AntiCheats.AsNoTracking().Any(b => b.Id == message.From.Id && b.ChatId == cGroup.ChatId))
                {
                    await Bot.SendMessageAsync(message.Chat.Id, MSG.AnticheatsMessage(cLang), replyToMessageId: cMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                    return;
                }
            }
            var targetMedia = new TargetMedia(message);
            if (targetMedia.Type == MediaType.Unknown)
            {
                await Bot.SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(cLang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else if (!targetMedia.IsValid)
            {
                await Bot.SendMessageAsync(message.Chat.Id, MSG.InvalidPhoto(cLang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var output = await Bot.SendMessageAsync(message.Chat.Id, MSG.GeneratingTmpUrl(cLang), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (targetMedia.NeedConversion && !TryGetImageFromVideo(targetMedia))
                {
                    await UpdateSearchMessageAsync(output, MSG.FailedConvertFile(cLang)).ConfigureAwait(false);
                }
                else if (!targetMedia.NeedConversion && !TryDownload(targetMedia))
                {
                    await UpdateSearchMessageAsync(output, MSG.TooBigFile(cLang)).ConfigureAwait(false);
                }
                else
                {
                    await UpdateSearchMessageAsync(output,
                        MSG.TmpUrlDone(cLang, targetMedia.TemporalFilePath),
                        new IKM(new IKB[] { IKB.SetUrl("Google", string.Format(SDIR.GoogleImageSearch, targetMedia.TemporalFilePath)),
                                IKB.SetUrl("Yandex", string.Format(SDIR.YandexUrl, targetMedia.TemporalFilePath)) },
                                new IKB[] { IKB.SetUrl("SauceNAO", string.Format(SDIR.SauceNAOsearch, targetMedia.TemporalFilePath)) })).ConfigureAwait(false);
                }
            }
        }
    }
}
