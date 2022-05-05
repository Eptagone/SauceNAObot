// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using SauceNAO.Core.Entities;
using SauceNAO.Core.Enums;
using SauceNAO.Core.Extensions;
using SauceNAO.Core.Models;
using SauceNAO.Core.Resources;
using System.Globalization;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.AvailableTypes;

using IKB = Telegram.BotAPI.AvailableTypes.InlineKeyboardButton;
using IKM = Telegram.BotAPI.AvailableTypes.InlineKeyboardMarkup;
using SDIR = SauceNAO.Core.Resources.SauceDirectory;

#nullable disable

namespace SauceNAO.Core
{
    public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
    {
        protected override async Task OnCommandAsync(Message message, string commandName, string commandParameters, CancellationToken cancellationToken)
        {
            var args = commandParameters.Split(' ').Where(p => !string.IsNullOrEmpty(p)).ToArray();
            await OnCommandAsync(commandName, args, cancellationToken).ConfigureAwait(false);
        }
        private async Task OnCommandAsync(string cmd, string[] args, CancellationToken cancellationToken)
        {
#if DEBUG
            _logger.LogInformation("New command: {cmd}. Args: {args_count}", cmd, args.Count());
#endif

            switch (cmd)
            {
                case "start":
                case "about":
                    await StartAsync(args, cancellationToken).ConfigureAwait(false);
                    break;
                case "apikey":
                    await ApiKeyAsync(args, cancellationToken).ConfigureAwait(false);
                    break;
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
                case "comprameunagalleta":
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
                case "sauce":
                case "source":
                case "saucenao":
                case "sourcenow":
                case "search":
                case "salsa":
                case "fuente":
                    await SauceAsync(Message.ReplyToMessage, cancellationToken).ConfigureAwait(false);
                    break;
                case "stats":
                case "statistics":
                    await StatisticsAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case "idioma":
                case "setlang":
                    await SetLangAsync(args, cancellationToken).ConfigureAwait(false);
                    break;
                case "setchatlang":
                case "setgrouplang":
                    await SetLangAsync(args, cancellationToken).ConfigureAwait(false);
                    break;
                case "temp":
                case "temporal":
                    await TempAsync(Message.ReplyToMessage, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }
        private async Task StartAsync(string[] args, CancellationToken cancellationToken)
        {
            await Api.SendMessageAsync(
                Message.Chat.Id,
                MSG.About(Language),
                ParseMode.HTML,
                disableWebPagePreview: true,
                replyToMessageId: Message.MessageId,
                allowSendingWithoutReply: true,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        private async Task ApiKeyAsync(string[] args, CancellationToken cancellationToken)
        {
            await Api.SendChatActionAsync(
                Message.Chat.Id,
                ChatAction.Typing,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            void ApiKeyDefault()
            {
                Api.SendMessage(Message.Chat.Id, MSG.ApiKey(Language), ParseMode.HTML, disableWebPagePreview: true, replyToMessageId: Message.MessageId, allowSendingWithoutReply: true);
            }

            if (isPrivate)
            {
                if (args.Length is >= 1 and <= 2)
                {
                    switch (args[0])
                    {
                        case "status":
                        case "view":
                        case "info":
                            if (string.IsNullOrEmpty(User.ApiKey))
                            {
                                await Api.SendMessageAsync(
                                    Message.Chat.Id,
                                    MSG.ApiKeyNone(Language),
                                    replyToMessageId: Message.MessageId,
                                    allowSendingWithoutReply: true,
                                    cancellationToken: cancellationToken).ConfigureAwait(false);
                            }
                            else
                            {
                                await Api.SendMessageAsync(
                                    Message.Chat.Id,
                                    MSG.ApiKeyStatus(Language, User.ApiKey),
                                    ParseMode.HTML,
                                    replyToMessageId: Message.MessageId,
                                    allowSendingWithoutReply: true,
                                    cancellationToken: cancellationToken).ConfigureAwait(false);
                            }
                            break;
                        case "set":
                            if (args.Length != 2)
                            {
                                ApiKeyDefault();
                            }
                            else
                            {
                                await Api.SendMessageAsync(
                                    Message.Chat.Id,
                                    MSG.ApiKeyNew(Language),
                                    ParseMode.HTML,
                                    replyToMessageId: Message.MessageId,
                                    allowSendingWithoutReply: true,
                                    cancellationToken: cancellationToken).ConfigureAwait(false);

                                User.ApiKey = args[1];
                                await _db.Users.UpdateAsync(User, cancellationToken).ConfigureAwait(false);
                            }
                            break;
                        case "del":
                        case "delete":
                        case "rem":
                        case "remove":
                        case "clear":
                        case "cls":
                            if (string.IsNullOrEmpty(User.ApiKey))
                            {
                                await Api.SendMessageAsync(
                                    Message.Chat.Id,
                                    MSG.ApiKeyNone(Language),
                                    replyToMessageId: Message.MessageId,
                                    allowSendingWithoutReply: true,
                                    cancellationToken: cancellationToken).ConfigureAwait(false);
                            }
                            else
                            {
                                await Api.SendMessageAsync(
                                    Message.Chat.Id,
                                    MSG.ApiKeyDeleted(Language),
                                    replyToMessageId: Message.MessageId,
                                    allowSendingWithoutReply: true,
                                    cancellationToken: cancellationToken).ConfigureAwait(false);

                                User.ApiKey = null;
                                await _db.Users.UpdateAsync(User, cancellationToken).ConfigureAwait(false);
                            }
                            break;
                        default:
                            ApiKeyDefault();
                            break;
                    }
                }
                else
                {
                    ApiKeyDefault();
                }
            }
            else
            {
                Api.SendMessage(Message.Chat.Id, MSG.PrivateChatsOnly(Language), replyToMessageId: Message.MessageId, allowSendingWithoutReply: true);
            }
        }
        private async Task AntiCheatsAsync(string[] args, CancellationToken cancellationToken)
        {
            await Api.SendChatActionAsync(
                Message.Chat.Id,
                ChatAction.Typing,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            void AntiCheatsDefault()
            {
                Api.SendMessage(Message.Chat.Id, MSG.Anticheats(Language), ParseMode.HTML, replyToMessageId: Message.MessageId, allowSendingWithoutReply: true);
            }

            if (isPrivate)
            {
                await Api.SendMessageAsync(
                    Message.Chat.Id,
                    MSG.GroupsOnly(Language),
                    replyToMessageId: Message.MessageId,
                    allowSendingWithoutReply: true,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else if (args.Length != 1)
            {
                AntiCheatsDefault();
            }
            else
            {
                switch (args[0])
                {
                    case "add":
                    case "insert":
                    case "new":
                        {
                            var bot = Message.ReplyToMessage?.From;
                            if (bot == default)
                            {
                                AntiCheatsDefault();
                            }
                            else if (bot.IsBot)
                            {
                                if (Group.AntiCheats.Any(a => a.BotId == bot.Id))
                                {
                                    await Api.SendMessageAsync(
                                        Message.Chat.Id,
                                        MSG.AnticheatsAlreadyExists(Language, bot.Username),
                                        replyToMessageId: Message.MessageId,
                                        allowSendingWithoutReply: true,
                                        cancellationToken: cancellationToken).ConfigureAwait(false);
                                }
                                else
                                {
                                    // Bot will be addded
                                    await Api.SendMessageAsync(
                                        Message.Chat.Id,
                                        MSG.AnticheatsAdded(Language, bot.Username),
                                        replyToMessageId: Message.MessageId,
                                        allowSendingWithoutReply: true,
                                        cancellationToken: cancellationToken).ConfigureAwait(false);

                                    var item = new AntiCheat(bot, User.Id);
                                    Group.AntiCheats.Add(item);
                                    await _db.Groups.UpdateAsync(Group, cancellationToken).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                // Target user is not a bot
                                await Api.SendMessageAsync(
                                    Message.Chat.Id,
                                    MSG.AntiCheatsNotABot(Language),
                                    replyToMessageId: Message.MessageId,
                                    allowSendingWithoutReply: true,
                                    cancellationToken: cancellationToken).ConfigureAwait(false);
                            }
                        }
                        break;
                    case "del":
                    case "delete":
                    case "rem":
                    case "remove":
                        {
                            var bot = Message.ReplyToMessage?.From;
                            if (bot == default)
                            {
                                AntiCheatsDefault();
                            }
                            else if (bot.IsBot)
                            {
                                var item = Group.AntiCheats.FirstOrDefault(a => a.BotId == bot.Id);
                                if (item == default)
                                {
                                    await Api.SendMessageAsync(
                                        Message.Chat.Id,
                                        MSG.AnticheatsDoesNotExist(Language, bot.Username),
                                        replyToMessageId: Message.MessageId,
                                        allowSendingWithoutReply: true,
                                        cancellationToken: cancellationToken).ConfigureAwait(false);
                                }
                                else
                                {
                                    // Bot will be removed
                                    await Api.SendMessageAsync(
                                        Message.Chat.Id,
                                        MSG.AnticheatsDeleted(Language, bot.Username),
                                        replyToMessageId: Message.MessageId,
                                        allowSendingWithoutReply: true,
                                        cancellationToken: cancellationToken).ConfigureAwait(false);

                                    Group.AntiCheats.Remove(item);
                                    await _db.Groups.UpdateAsync(Group, cancellationToken).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                // Target user is not a bot
                                await Api.SendMessageAsync(
                                    Message.Chat.Id,
                                    MSG.AntiCheatsNotABot(Language),
                                    replyToMessageId: Message.MessageId,
                                    allowSendingWithoutReply: true,
                                    cancellationToken: cancellationToken).ConfigureAwait(false);
                            }
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
            await Api.SendChatActionAsync(Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (Message.ReplyToMessage == default)
            {
                await Api.SendMessageAsync(Message.Chat.Id, MSG.Creator(Language), ParseMode.HTML, replyToMessageId: Message.MessageId, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await Api.SendMessageAsync(Message.Chat.Id, MSG.Creator(Language), ParseMode.HTML, replyToMessageId: Message.ReplyToMessage.MessageId, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
        private async Task BuyMeACookie(CancellationToken cancellationToken)
        {
            await Api.SendMessageAsync(Message.Chat.Id, MSG.BuyMeACookie(Language), ParseMode.HTML, disableWebPagePreview: true, replyToMessageId: Message.MessageId, allowSendingWithoutReply: true, replyMarkup: new IKM(new IKB[] { IKB.SetUrl(MSG.SupportChat(Language), Properties.SupportChatLink) }), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        private async Task HelpAsync(string[] args, CancellationToken cancellationToken)
        {
            await Api.SendChatActionAsync(Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

            void HelpDefault()
            {
                Api.SendMessage(
                    Message.Chat.Id,
                    MSG.Help(Language),
                    ParseMode.HTML,
                    replyToMessageId: Message.MessageId,
                    allowSendingWithoutReply: true,
                    replyMarkup: new IKM(new IKB[] { IKB.SetUrl(MSG.SupportChat(Language), Properties.SupportChatLink) }));
            }

            if (args.Length != 1)
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
                        await Api.SendMessageAsync(
                            Message.Chat.Id,
                            MSG.Anticheats(Language),
                            replyToMessageId: Message.MessageId,
                            allowSendingWithoutReply: true,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                        break;
                    case "myhistory":
                    case "history":
                    case "historial":
                    case "mihistorial":
                        await Api.SendMessageAsync(
                            Message.Chat.Id,
                            MSG.History(Language, Me.Username),
                            replyToMessageId: Message.MessageId,
                            allowSendingWithoutReply: true,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                        break;
                    default:
                        HelpDefault();
                        break;
                }
            }
        }
        private async Task HistoryAsync(string[] args, CancellationToken cancellationToken)
        {
            await Api.SendChatActionAsync(Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

            async Task HistoryDefault()
            {
                var text = MSG.History(Language, Me.Username);
                var buttonText = MSG.HistoryButton(Language);
                await Api.SendMessageAsync(
                    Message.Chat.Id,
                    text,
                    parseMode: ParseMode.HTML,
                    replyToMessageId: Message.MessageId,
                    allowSendingWithoutReply: true,
                    replyMarkup: new IKM(new IKB[] { IKB.SetSwitchInlineQueryCurrentChat(buttonText, string.Empty) }),
                    cancellationToken: cancellationToken).ConfigureAwait(false);
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
                        {
                            await Api.SendMessageAsync(
                                Message.Chat.Id,
                                MSG.HistoryErased(Language),
                                replyToMessageId: Message.MessageId,
                                allowSendingWithoutReply: true,
                                cancellationToken: cancellationToken).ConfigureAwait(false);

                            User.UserSauces.Clear();
                            await _db.Users.UpdateAsync(User, cancellationToken).ConfigureAwait(false);
                        }
                        break;
                    default:
                        await HistoryDefault().ConfigureAwait(false);
                        break;
                }
            }
        }
        private async Task SetLangAsync(string[] args, CancellationToken cancellationToken)
        {
            await Api.SendChatActionAsync(Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

            var isPrivate = Message.Chat.Type == ChatType.Private;

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
                var text = MSG.SetLang(Language);
                await Api.SendMessageAsync(
                    Message.Chat.Id,
                    text,
                    ParseMode.HTML,
                    replyToMessageId: Message.MessageId,
                    allowSendingWithoutReply: true,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            async Task SetlangSaved()
            {
                var lang = new CultureInfo((isPrivate || Group == default ? User.LanguageCode : Group.LanguageCode) ?? "en");
                await Api.SendMessageAsync(Message.Chat.Id, MSG.SetLangSaved(lang), replyToMessageId: Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            if (args.Length > 0)
            {
                var setLang = args[0].ToLowerInvariant();
                if (setLang.Length > 8 || !CultureIsValid(setLang))
                {
                    await SetLangDefault().ConfigureAwait(false);
                    return;
                }

                if (isPrivate)
                {
                    User.LanguageCode = setLang;
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
                            User.LangForce = true;
                        }
                        else
                        {
                            await SetLangDefault().ConfigureAwait(false);
                            return;
                        }
                    }
                    else
                    {
                        User.LangForce = false;
                    }
                    await SetlangSaved().ConfigureAwait(false);
                    await _db.Users.UpdateAsync(User, cancellationToken).ConfigureAwait(false);
                }
                else if (args.Length == 1)
                {
                    var admins = await Api.GetChatAdministratorsAsync(Group.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (admins.Any(a => a.User.Id == Message.From.Id))
                    {
                        Group.LanguageCode = setLang;
                        await SetlangSaved().ConfigureAwait(false);
                        await _db.Groups.UpdateAsync(Group, cancellationToken).ConfigureAwait(false);
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
            else
            {
                await SetLangDefault().ConfigureAwait(false);
            }
        }
        private async Task SauceAsync(Message message, CancellationToken cancellationToken)
        {
            if (message == default)
            {
                await Api.SendMessageAsync(Message.Chat.Id, MSG.EmptyRequest(Language), replyToMessageId: Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                return;
            }
            await Api.SendChatActionAsync(message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!isPrivate && message.From.IsBot)
            {
                if (Group.AntiCheats.Any(b => b.BotId == message.From.Id && b.Key == Group.Key))
                {
                    await Api.SendMessageAsync(message.Chat.Id, MSG.AnticheatsMessage(Language), replyToMessageId: Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                    return;
                }
            }
            var targetMedia = new TargetMedia(message);
            if (targetMedia.Type == MediaType.Unknown)
            {
                await Api.SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else if (!targetMedia.IsValid)
            {
                await Api.SendMessageAsync(message.Chat.Id, MSG.InvalidPhoto(Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var sauce = _db.Sauces.GetAllSauces().FirstOrDefault(s => s.FileUniqueId == targetMedia.FileUniqueId);
                if (sauce == default)
                {
                    var output = await Api.SendMessageAsync(message.Chat.Id, MSG.Searching(Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);

                    if (!TryGetFilePath(targetMedia, out _))
                    {
                        await UpdateSearchMessageAsync(output, MSG.TooBigFile(Language)).ConfigureAwait(false);
                    }
                    else
                    {
                        if (targetMedia.NeedConversion)
                        {
                            if (!Properties.WebhookMode)
                            {
                                await UpdateSearchMessageAsync(output, MSG.LocalModeFile(Language)).ConfigureAwait(false);
                                return;
                            }
                            else if (!(await TryGetImageFromVideo(targetMedia, output, cancellationToken).ConfigureAwait(false)))
                            {
                                await UpdateSearchMessageAsync(output, MSG.FailedConvertFile(Language)).ConfigureAwait(false);
                                return;
                            }
                        }

                        var sauceResult = await CookSauceAsync(targetMedia.TargetSearchPath, cancellationToken).ConfigureAwait(false);
                        switch (sauceResult.Status)
                        {
                            case SauceStatus.Found:
                                await UpdateSearchMessageAsync(output, sauceResult.Sauce.GetInfo(Language), sauceResult.Urls.ToInlineKeyboardMarkup()).ConfigureAwait(false);
                                // save sauce to db and update user's search history
                                sauce = new SuccessfulSauce(sauceResult, targetMedia, Date);
                                await _db.Sauces.InsertAsync(sauce, cancellationToken).ConfigureAwait(false);
                                await _db.Users.InsertSauceAsync(User.Id, new UserSauce(sauce.Key, Date), cancellationToken).ConfigureAwait(false);
                                break;
                            case SauceStatus.NotFound:
                                if (!Properties.WebhookMode) // Local Mode
                                {
                                    await UpdateSearchMessageAsync(output, MSG.NotFoundLocalMode(Language)).ConfigureAwait(false);
                                }
                                else // Webhook Mode
                                {
                                    if (string.IsNullOrEmpty(targetMedia.TemporalFilePath) && !(await TryDownloadAsync(targetMedia, cancellationToken).ConfigureAwait(false)))
                                    {
                                        await UpdateSearchMessageAsync(output, MSG.TooBigFile(Language)).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UpdateSearchMessageAsync(output, MSG.NotFound(targetMedia.TemporalFilePath, Language)).ConfigureAwait(false);
                                    }
                                }
                                break;
                            case SauceStatus.Error:
                            case SauceStatus.BadRequest:
                                await UpdateSearchMessageAsync(output, MSG.Busy(Language, Properties.SupportChatLink)).ConfigureAwait(false);
                                OnSauceError(sauceResult.Message);
                                break;
                        }
                    }
                }
                else
                {
                    await Api.SendMessageAsync(message.Chat.Id, sauce.GetInfo(Language), ParseMode.HTML, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, replyMarkup: sauce.GetKeyboard(), cancellationToken: cancellationToken).ConfigureAwait(false);

                    var userSauce = User.UserSauces.FirstOrDefault(s => s.UserId == User.Id && s.SauceId == sauce.Key);
                    if (userSauce == default)
                    {
                        userSauce = new UserSauce(sauce.Key, Date);
                        await _db.Users.InsertSauceAsync(User.Id, userSauce, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        userSauce.Date = Date;
                        await _db.Users.UpdateAsync(User, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }
        private async Task TempAsync(Message message, CancellationToken cancellationToken)
        {
            await Api.SendChatActionAsync(Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!Properties.WebhookMode) // Temp command is disabled in Local Mode
            {
                await Api.SendMessageAsync(Message.Chat.Id, MSG.LocalMode(Language), replyToMessageId: Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                return;
            }

            if (message == default)
            {
                await Api.SendMessageAsync(Message.Chat.Id, MSG.EmptyRequest(Language), replyToMessageId: Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                return;
            }

            if (!isPrivate && message.From.IsBot)
            {
                if (Group.AntiCheats.Any(b => b.BotId == message.From.Id && b.ChatKey == Group.Key))
                {
                    await Api.SendMessageAsync(message.Chat.Id, MSG.AnticheatsMessage(Language), replyToMessageId: Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                    return;
                }
            }
            var targetMedia = new TargetMedia(message);
            if (targetMedia.Type == MediaType.Unknown)
            {
                await Api.SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else if (!targetMedia.IsValid)
            {
                await Api.SendMessageAsync(message.Chat.Id, MSG.InvalidPhoto(Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var output = await Api.SendMessageAsync(message.Chat.Id, MSG.GeneratingTmpUrl(Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (targetMedia.NeedConversion && !(await TryGetImageFromVideo(targetMedia, output, cancellationToken).ConfigureAwait(false)))
                {
                    await UpdateSearchMessageAsync(output, MSG.FailedConvertFile(Language)).ConfigureAwait(false);
                }
                else if (!targetMedia.NeedConversion && !(await TryDownloadAsync(targetMedia, cancellationToken).ConfigureAwait(false)))
                {
                    await UpdateSearchMessageAsync(output, MSG.TooBigFile(Language)).ConfigureAwait(false);
                }
                else
                {
                    await UpdateSearchMessageAsync(output,
                        MSG.TemporalUrlDone(Language, targetMedia.TemporalFilePath),
                        new IKM(new IKB[] { IKB.SetUrl("Google", string.Format(SDIR.GoogleImageSearch, targetMedia.TemporalFilePath)),
                                IKB.SetUrl("Yandex", string.Format(SDIR.YandexUrl, targetMedia.TemporalFilePath)) },
                                new IKB[] { IKB.SetUrl("SauceNAO", string.Format(SDIR.SauceNAOsearch, targetMedia.TemporalFilePath)) })).ConfigureAwait(false);
                }
            }
        }
        private async Task StatisticsAsync(CancellationToken cancellationToken)
        {
            await Api.SendChatActionAsync(Message.Chat.Id, ChatAction.Typing, cancellationToken).ConfigureAwait(false);
            var sucefullSearchCount = _db.Sauces.GetAllSauces().Count();
            var usersCount = _db.Users.GetAllUsers().Count();
            var groupCount = _db.Groups.GetAllGroups().Count();
            var stats = MSG.Statistics(Language, sucefullSearchCount, usersCount, groupCount);
            await Api.SendMessageAsync(Message.Chat.Id, stats, ParseMode.HTML, replyToMessageId: Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        private async Task LanguagesAsync(CancellationToken cancellationToken)
        {
            await Api.SendChatActionAsync(Message.Chat.Id, ChatAction.Typing, cancellationToken).ConfigureAwait(false);

            var langs = _db.Users.GetAllUsers().GroupBy(u => u.LanguageCode).OrderByDescending(g => g.Count());

            var values = string.Empty;
            foreach (var l in langs)
            {
                values += string.Format("\n- <b>{0}</b> [{1}]", l.Key ?? "default", l.Count());
            }

            var text = string.Format(MSG.LanguageCodes(Language), values);
            await Api.SendMessageAsync(Message.Chat.Id, text, ParseMode.HTML, replyToMessageId: Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
