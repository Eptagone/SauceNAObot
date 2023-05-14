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
using IBB = Telegram.BotAPI.InlineButtonBuilder;
using SDIR = SauceNAO.Core.Resources.SauceDirectory;

#nullable disable

namespace SauceNAO.Core;

public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
{
	protected override async Task OnCommandAsync(Message message, string commandName, string commandParameters, CancellationToken cancellationToken)
	{
		var args = commandParameters.Split(' ').Where(p => !string.IsNullOrEmpty(p)).ToArray();
		await this.OnCommandAsync(commandName, args, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Process command.
	/// </summary>
	/// <param name="cmd">The command name.</param>
	/// <param name="args">List of arguments.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	private async Task OnCommandAsync(string cmd, string[] args, CancellationToken cancellationToken)
	{
#if DEBUG
		this._logger.LogInformation("New command: {cmd}. Args: {args_count}", cmd, args.Length);
#endif

		switch (cmd)
		{
			case "start":
			case "about":
				await this.StartAsync(args, cancellationToken).ConfigureAwait(false);
				break;
			case "apikey":
				await this.ApiKeyAsync(args, cancellationToken).ConfigureAwait(false);
				break;
			case "antitrampas":
			case "notrampas":
			case "anticheats":
			case "cheatcontrol":
				await this.AntiCheatsAsync(args, cancellationToken).ConfigureAwait(false);
				break;
			case "botcreator":
			case "creator":
				await this.BotCreatorAsync(cancellationToken).ConfigureAwait(false);
				break;
			case "support":
			case "supportme":
			case "buymeacookie":
			case "buymeacoffee":
			case "comprameunagalleta":
				await this.BuyMeACookie(cancellationToken).ConfigureAwait(false);
				break;
			case "help":
			case "ayuda":
				await this.HelpAsync(args, cancellationToken).ConfigureAwait(false);
				break;
			case "history":
			case "myhistory":
			case "mysauce":
			case "mysauces":
			case "mihistorial":
			case "historial":
				await this.HistoryAsync(args, cancellationToken).ConfigureAwait(false);
				break;
			case "sauce":
			case "source":
			case "saucenao":
			case "sourcenow":
			case "search":
			case "salsa":
			case "fuente":
				await this.SauceAsync(this.Message.ReplyToMessage, cancellationToken).ConfigureAwait(false);
				break;
			case "stats":
			case "statistics":
				await this.StatisticsAsync(cancellationToken).ConfigureAwait(false);
				break;
			case "idioma":
			case "setlang":
				await this.SetLangAsync(args, cancellationToken).ConfigureAwait(false);
				break;
			case "setchatlang":
			case "setgrouplang":
				await this.SetLangAsync(args, cancellationToken).ConfigureAwait(false);
				break;
			case "temp":
			case "temporal":
				await this.TempAsync(this.Message.ReplyToMessage, cancellationToken).ConfigureAwait(false);
				break;
			case "langs":
				await this.LanguagesAsync(cancellationToken).ConfigureAwait(false);
				break;
		}
	}

	private async Task StartAsync(string[] _, CancellationToken cancellationToken)
	{
		await this.Api.SendMessageAsync(
			this.Message.Chat.Id,
			MSG.About(this.Language),
			parseMode: ParseMode.HTML,
			disableWebPagePreview: true,
			replyToMessageId: this.Message.MessageId,
			allowSendingWithoutReply: true,
			cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	private async Task ApiKeyAsync(string[] args, CancellationToken cancellationToken)
	{
		// Send the 'typing' action
		await this.Api.SendChatActionAsync(
			this.Message.Chat.Id,
			ChatAction.Typing,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		// The default action for the 'apikey' command.
		void ApiKeyDefault()
		{
			this.Api.SendMessage(this.Message.Chat.Id, MSG.ApiKey(this.Language), parseMode: ParseMode.HTML, disableWebPagePreview: true, replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true);
		}

		if (this.isPrivate)
		{
			if (args.Length is >= 1 and <= 2)
			{
				switch (args[0])
				{
					case "status":
					case "view":
					case "info":
						if (string.IsNullOrEmpty(this.User.ApiKey))
						{
							await this.Api.SendMessageAsync(
								this.Message.Chat.Id,
								MSG.ApiKeyNone(this.Language),
								replyToMessageId: this.Message.MessageId,
								allowSendingWithoutReply: true,
								cancellationToken: cancellationToken).ConfigureAwait(false);
						}
						else
						{
							await this.Api.SendMessageAsync(
								this.Message.Chat.Id,
								MSG.ApiKeyStatus(this.Language, this.User.ApiKey),
								parseMode: ParseMode.HTML,
								replyToMessageId: this.Message.MessageId,
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
							await this.Api.SendMessageAsync(
								this.Message.Chat.Id,
								MSG.ApiKeyNew(this.Language),
								parseMode: ParseMode.HTML,
								replyToMessageId: this.Message.MessageId,
								allowSendingWithoutReply: true,
								cancellationToken: cancellationToken).ConfigureAwait(false);

							this.User.ApiKey = args[1];
							await this._db.Users.UpdateAsync(this.User, cancellationToken).ConfigureAwait(false);
						}
						break;
					case "del":
					case "delete":
					case "rem":
					case "remove":
					case "clear":
					case "cls":
						if (string.IsNullOrEmpty(this.User.ApiKey))
						{
							await this.Api.SendMessageAsync(
								this.Message.Chat.Id,
								MSG.ApiKeyNone(this.Language),
								replyToMessageId: this.Message.MessageId,
								allowSendingWithoutReply: true,
								cancellationToken: cancellationToken).ConfigureAwait(false);
						}
						else
						{
							await this.Api.SendMessageAsync(
								this.Message.Chat.Id,
								MSG.ApiKeyDeleted(this.Language),
								replyToMessageId: this.Message.MessageId,
								allowSendingWithoutReply: true,
								cancellationToken: cancellationToken).ConfigureAwait(false);

							this.User.ApiKey = null;
							await this._db.Users.UpdateAsync(this.User, cancellationToken).ConfigureAwait(false);
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
			this.Api.SendMessage(this.Message.Chat.Id, MSG.PrivateChatsOnly(this.Language), replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true);
		}
	}
	private async Task AntiCheatsAsync(string[] args, CancellationToken cancellationToken)
	{
		await this.Api.SendChatActionAsync(
			this.Message.Chat.Id,
			ChatAction.Typing,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		void AntiCheatsDefault()
		{
			this.Api.SendMessage(this.Message.Chat.Id, MSG.Anticheats(this.Language), parseMode: ParseMode.HTML, replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true);
		}

		if (this.isPrivate)
		{
			await this.Api.SendMessageAsync(
				this.Message.Chat.Id,
				MSG.GroupsOnly(this.Language),
				replyToMessageId: this.Message.MessageId,
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
						var bot = this.Message.ReplyToMessage?.From;
						if (bot == default)
						{
							AntiCheatsDefault();
						}
						else if (bot.IsBot)
						{
							if (this.Group.AntiCheats.Any(a => a.BotId == bot.Id))
							{
								await this.Api.SendMessageAsync(
									this.Message.Chat.Id,
									MSG.AnticheatsAlreadyExists(this.Language, bot.Username),
									replyToMessageId: this.Message.MessageId,
									allowSendingWithoutReply: true,
									cancellationToken: cancellationToken).ConfigureAwait(false);
							}
							else
							{
								// Bot will be addded
								await this.Api.SendMessageAsync(
									this.Message.Chat.Id,
									MSG.AnticheatsAdded(this.Language, bot.Username),
									replyToMessageId: this.Message.MessageId,
									allowSendingWithoutReply: true,
									cancellationToken: cancellationToken).ConfigureAwait(false);

								var item = new AntiCheat(bot, this.User.Id);
								this.Group.AntiCheats.Add(item);
								await this._db.Groups.UpdateAsync(this.Group, cancellationToken).ConfigureAwait(false);
							}
						}
						else
						{
							// Target user is not a bot
							await this.Api.SendMessageAsync(
								this.Message.Chat.Id,
								MSG.AntiCheatsNotABot(this.Language),
								replyToMessageId: this.Message.MessageId,
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
						var bot = this.Message.ReplyToMessage?.From;
						if (bot == default)
						{
							AntiCheatsDefault();
						}
						else if (bot.IsBot)
						{
							var item = this.Group.AntiCheats.FirstOrDefault(a => a.BotId == bot.Id);
							if (item == default)
							{
								await this.Api.SendMessageAsync(
									this.Message.Chat.Id,
									MSG.AnticheatsDoesNotExist(this.Language, bot.Username),
									replyToMessageId: this.Message.MessageId,
									allowSendingWithoutReply: true,
									cancellationToken: cancellationToken).ConfigureAwait(false);
							}
							else
							{
								// Bot will be removed
								await this.Api.SendMessageAsync(
									this.Message.Chat.Id,
									MSG.AnticheatsDeleted(this.Language, bot.Username),
									replyToMessageId: this.Message.MessageId,
									allowSendingWithoutReply: true,
									cancellationToken: cancellationToken).ConfigureAwait(false);

								this.Group.AntiCheats.Remove(item);
								await this._db.Groups.UpdateAsync(this.Group, cancellationToken).ConfigureAwait(false);
							}
						}
						else
						{
							// Target user is not a bot
							await this.Api.SendMessageAsync(
								this.Message.Chat.Id,
								MSG.AntiCheatsNotABot(this.Language),
								replyToMessageId: this.Message.MessageId,
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

	/// <summary>
	/// Send to the user a message with the bot creator.
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	private async Task BotCreatorAsync(CancellationToken cancellationToken)
	{
		await this.Api.SendChatActionAsync(this.Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);
		if (this.Message.ReplyToMessage == default)
		{
			await this.Api.SendMessageAsync(this.Message.Chat.Id, MSG.Creator(this.Language), parseMode: ParseMode.HTML, replyToMessageId: this.Message.MessageId, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		else
		{
			await this.Api.SendMessageAsync(this.Message.Chat.Id, MSG.Creator(this.Language), parseMode: ParseMode.HTML, replyToMessageId: this.Message.ReplyToMessage.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
	}

	private async Task BuyMeACookie(CancellationToken cancellationToken)
	{
		await this.Api.SendMessageAsync(this.Message.Chat.Id, MSG.BuyMeACookie(this.Language), parseMode: ParseMode.HTML, disableWebPagePreview: true, replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true, replyMarkup: new IKM(new IKB[] { IBB.SetUrl(MSG.SupportChat(this.Language), this.Properties.SupportChatLink) }), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Send to the user a message with help information.
	/// </summary>
	/// <param name="args"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	private async Task HelpAsync(string[] args, CancellationToken cancellationToken)
	{
		await this.Api.SendChatActionAsync(this.Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

		void HelpDefault()
		{
			this.Api.SendMessage(
				this.Message.Chat.Id,
				MSG.Help(this.Language),
				parseMode: ParseMode.HTML,
				replyToMessageId: this.Message.MessageId,
				allowSendingWithoutReply: true,
				replyMarkup: new IKM(new IKB[] { IBB.SetUrl(MSG.SupportChat(this.Language), this.Properties.SupportChatLink) }));
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
					await this.Api.SendMessageAsync(
						this.Message.Chat.Id,
						MSG.Anticheats(this.Language),
						replyToMessageId: this.Message.MessageId,
						allowSendingWithoutReply: true,
						cancellationToken: cancellationToken).ConfigureAwait(false);
					break;
				case "myhistory":
				case "history":
				case "historial":
				case "mihistorial":
					await this.Api.SendMessageAsync(
						this.Message.Chat.Id,
						MSG.History(this.Language, this.Me.Username),
						replyToMessageId: this.Message.MessageId,
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
		await this.Api.SendChatActionAsync(this.Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

		async Task HistoryDefault()
		{
			var text = MSG.History(this.Language, this.Me.Username);
			var buttonText = MSG.HistoryButton(this.Language);
			await this.Api.SendMessageAsync(
				this.Message.Chat.Id,
				text,
				parseMode: ParseMode.HTML,
				replyToMessageId: this.Message.MessageId,
				allowSendingWithoutReply: true,
				replyMarkup: new IKM(new IKB[] { IBB.SetSwitchInlineQueryCurrentChat(buttonText, string.Empty) }),
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
						await this.Api.SendMessageAsync(
							this.Message.Chat.Id,
							MSG.HistoryErased(this.Language),
							replyToMessageId: this.Message.MessageId,
							allowSendingWithoutReply: true,
							cancellationToken: cancellationToken).ConfigureAwait(false);

						this._db.Users.ClearSauces(this.User.Id);
						await this._db.Users.UpdateAsync(this.User, cancellationToken).ConfigureAwait(false);
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
		await this.Api.SendChatActionAsync(this.Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

		var isPrivate = this.Message.Chat.Type == ChatType.Private;

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
			var text = MSG.SetLang(this.Language);
			await this.Api.SendMessageAsync(
				this.Message.Chat.Id,
				text,
				parseMode: ParseMode.HTML,
				replyToMessageId: this.Message.MessageId,
				allowSendingWithoutReply: true,
				cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		async Task SetlangSaved()
		{
			var lang = new CultureInfo((isPrivate || this.Group == default ? this.User.LanguageCode : this.Group.LanguageCode) ?? "en");
			await this.Api.SendMessageAsync(this.Message.Chat.Id, MSG.SetLangSaved(lang), replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
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
				this.User.LanguageCode = setLang;
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
						this.User.LangForce = true;
					}
					else
					{
						await SetLangDefault().ConfigureAwait(false);
						return;
					}
				}
				else
				{
					this.User.LangForce = false;
				}
				await SetlangSaved().ConfigureAwait(false);
				await this._db.Users.UpdateAsync(this.User, cancellationToken).ConfigureAwait(false);
			}
			else if (args.Length == 1)
			{
				var admins = await this.Api.GetChatAdministratorsAsync(this.Group.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
				if (admins.Any(a => a.User.Id == this.Message.From.Id))
				{
					this.Group.LanguageCode = setLang;
					await SetlangSaved().ConfigureAwait(false);
					await this._db.Groups.UpdateAsync(this.Group, cancellationToken).ConfigureAwait(false);
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
			await this.Api.SendMessageAsync(this.Message.Chat.Id, MSG.EmptyRequest(this.Language), replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
			return;
		}
		await this.Api.SendChatActionAsync(message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);
		if (!this.isPrivate && message.From.IsBot)
		{
			if (this.Group.AntiCheats.Any(b => b.BotId == message.From.Id && b.Key == this.Group.Key))
			{
				await this.Api.SendMessageAsync(message.Chat.Id, MSG.AnticheatsMessage(this.Language), replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
				return;
			}
		}
		var targetMedia = new TargetMedia(message);
		if (targetMedia.Type == MediaType.Unknown)
		{
			await this.Api.SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(this.Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		else if (!targetMedia.IsValid)
		{
			await this.Api.SendMessageAsync(message.Chat.Id, MSG.InvalidPhoto(this.Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		else
		{
			var sauce = this._db.Sauces.GetAllSauces().FirstOrDefault(s => s.FileUniqueId == targetMedia.FileUniqueId);
			if (sauce == default)
			{
				var output = await this.Api.SendMessageAsync(message.Chat.Id, MSG.Searching(this.Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);

				if (!this.TryGetFilePath(targetMedia, out _))
				{
					await this.UpdateSearchMessageAsync(output, MSG.TooBigFile(this.Language)).ConfigureAwait(false);
				}
				else
				{
					if (targetMedia.NeedConversion)
					{
						if (!this.Properties.WebhookMode)
						{
							await this.UpdateSearchMessageAsync(output, MSG.LocalModeFile(this.Language)).ConfigureAwait(false);
							return;
						}
						else if (!(await this.TryGetImageFromVideo(targetMedia, output, cancellationToken).ConfigureAwait(false)))
						{
							await this.UpdateSearchMessageAsync(output, MSG.FailedConvertFile(this.Language)).ConfigureAwait(false);
							return;
						}
					}

					var sauceResult = await this.CookSauceAsync(targetMedia.TargetSearchPath, cancellationToken).ConfigureAwait(false);
					switch (sauceResult.Status)
					{
						case SauceStatus.Found:
							await this.UpdateSearchMessageAsync(output, sauceResult.Sauce.GetInfo(this.Language), sauceResult.Urls.ToInlineKeyboardMarkup()).ConfigureAwait(false);
							// save sauce to db and update user's search history
							sauce = new SuccessfulSauce(sauceResult, targetMedia, this.Date);
							await this._db.Sauces.InsertAsync(sauce, cancellationToken).ConfigureAwait(false);
							await this._db.Users.InsertSauceAsync(this.User.Id, new UserSauce(sauce.Key, this.Date), cancellationToken).ConfigureAwait(false);
							break;
						case SauceStatus.NotFound:
							if (!this.Properties.WebhookMode) // Local Mode
							{
								await this.UpdateSearchMessageAsync(output, MSG.NotFoundLocalMode(this.Language)).ConfigureAwait(false);
							}
							else // Webhook Mode
							{
								if (string.IsNullOrEmpty(targetMedia.TemporalFilePath) && !(await this.TryDownloadAsync(targetMedia, cancellationToken).ConfigureAwait(false)))
								{
									await this.UpdateSearchMessageAsync(output, MSG.TooBigFile(this.Language)).ConfigureAwait(false);
								}
								else
								{
									await this.UpdateSearchMessageAsync(output, MSG.NotFound(targetMedia.TemporalFilePath, this.Language)).ConfigureAwait(false);
								}
							}
							break;
						case SauceStatus.Error:
						case SauceStatus.BadRequest:
							await this.UpdateSearchMessageAsync(output, MSG.Busy(this.Language, this.Properties.SupportChatLink)).ConfigureAwait(false);
							this.OnSauceError(sauceResult.Message);
							break;
					}
				}
			}
			else
			{
				await this.Api.SendMessageAsync(message.Chat.Id, sauce.GetInfo(this.Language), parseMode: ParseMode.HTML, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, replyMarkup: sauce.GetKeyboard(), cancellationToken: cancellationToken).ConfigureAwait(false);

				var userSauce = this.User.UserSauces.FirstOrDefault(s => s.UserId == this.User.Id && s.SauceId == sauce.Key);
				if (userSauce == default)
				{
					userSauce = new UserSauce(sauce.Key, this.Date);
					await this._db.Users.InsertSauceAsync(this.User.Id, userSauce, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					userSauce.Date = this.Date;
					await this._db.Users.UpdateAsync(this.User, cancellationToken).ConfigureAwait(false);
				}
			}
		}
	}
	
	private async Task TempAsync(Message message, CancellationToken cancellationToken)
	{
		await this.Api.SendChatActionAsync(this.Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

		if (!this.Properties.WebhookMode) // Temp command is disabled in Local Mode
		{
			await this.Api.SendMessageAsync(this.Message.Chat.Id, MSG.LocalMode(this.Language), replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
			return;
		}

		if (message == default)
		{
			await this.Api.SendMessageAsync(this.Message.Chat.Id, MSG.EmptyRequest(this.Language), replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
			return;
		}

		if (!this.isPrivate && message.From.IsBot)
		{
			if (this.Group.AntiCheats.Any(b => b.BotId == message.From.Id && b.ChatKey == this.Group.Key))
			{
				await this.Api.SendMessageAsync(message.Chat.Id, MSG.AnticheatsMessage(this.Language), replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
				return;
			}
		}
		var targetMedia = new TargetMedia(message);
		if (targetMedia.Type == MediaType.Unknown)
		{
			await this.Api.SendMessageAsync(message.Chat.Id, MSG.EmptyRequest(this.Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		else if (!targetMedia.IsValid)
		{
			await this.Api.SendMessageAsync(message.Chat.Id, MSG.InvalidPhoto(this.Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		else
		{
			var output = await this.Api.SendMessageAsync(message.Chat.Id, MSG.GeneratingTmpUrl(this.Language), replyToMessageId: message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
			if (targetMedia.NeedConversion && !(await this.TryGetImageFromVideo(targetMedia, output, cancellationToken).ConfigureAwait(false)))
			{
				await this.UpdateSearchMessageAsync(output, MSG.FailedConvertFile(this.Language)).ConfigureAwait(false);
			}
			else if (!targetMedia.NeedConversion && !(await this.TryDownloadAsync(targetMedia, cancellationToken).ConfigureAwait(false)))
			{
				await this.UpdateSearchMessageAsync(output, MSG.TooBigFile(this.Language)).ConfigureAwait(false);
			}
			else
			{
				await this.UpdateSearchMessageAsync(output,
					MSG.TemporalUrlDone(this.Language, targetMedia.TemporalFilePath),
					new IKM(new IKB[] { IBB.SetUrl("Google", string.Format(SDIR.GoogleImageSearch, targetMedia.TemporalFilePath)),
							IBB.SetUrl("Yandex", string.Format(SDIR.YandexUrl, targetMedia.TemporalFilePath)) },
							new IKB[] { IBB.SetUrl("SauceNAO", string.Format(SDIR.SauceNAOsearch, targetMedia.TemporalFilePath)) })).ConfigureAwait(false);
			}
		}
	}
	
	private async Task StatisticsAsync(CancellationToken cancellationToken)
	{
		await this.Api.SendChatActionAsync(this.Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);
		var sucefullSearchCount = this._db.Sauces.GetAllSauces().Count();
		var usersCount = this._db.Users.GetAllUsers().Count();
		var groupCount = this._db.Groups.GetAllGroups().Count();
		var stats = MSG.Statistics(this.Language, sucefullSearchCount, usersCount, groupCount);
		await this.Api.SendMessageAsync(this.Message.Chat.Id, stats, parseMode: ParseMode.HTML, replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	private async Task LanguagesAsync(CancellationToken cancellationToken)
	{
		await this.Api.SendChatActionAsync(this.Message.Chat.Id, ChatAction.Typing, cancellationToken: cancellationToken).ConfigureAwait(false);

		var langs = this._db.Users.GetAllUsers()
			.GroupBy(u => u.LanguageCode)
			.Select(g => new { g.Key, Count = g.Count() })
			.OrderByDescending(g => g.Count);

		var values = string.Empty;
		foreach (var l in langs)
		{
			values += string.Format("\n- <b>{0}</b> [{1}]", l.Key ?? "default", l.Count);
		}

		var text = string.Format(MSG.LanguageCodes(this.Language), values);
		await this.Api.SendMessageAsync(this.Message.Chat.Id, text, parseMode: ParseMode.HTML, replyToMessageId: this.Message.MessageId, allowSendingWithoutReply: true, cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
