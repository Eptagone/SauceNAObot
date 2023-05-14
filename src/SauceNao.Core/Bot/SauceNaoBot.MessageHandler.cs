// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using SauceNAO.Core.Extensions;
using System.Globalization;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core;

public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
{
	protected override async Task OnMessageAsync(Message message, CancellationToken cancellationToken)
	{
		if (message.From != null)
		{
			// If the message was sent by Telegram (linked channels), ignore it.
			if (message.From.Id == 777000)
			{
				return;
			}
#if DEBUG
			this._logger.LogInformation("A new message was received from user: {user_fullname} [{user_id}]. Chat: {chat_title} [{chat_id}]", message.From.GetFullname(), message.From.Id, message.Chat.Title ?? "Private chat", message.Chat.Id);
#endif
			// Save message instance
			this.Message = message ?? throw new ArgumentNullException(nameof(message));
			// Message has text
			var hasText = !string.IsNullOrEmpty(message.Text);
			// Message has caption
			var hasCaption = !hasText && !string.IsNullOrEmpty(message.Caption);
			// Is a private chat
			this.isPrivate = message.Chat.Type == ChatType.Private;

			// Local function: LoadData
			async Task LoadDataAsync()
			{
				// Load user data
				this.User = await this._db.Users.GetUserAsync(message.From, cancellationToken).ConfigureAwait(false);
				this.Date = DateTimeOffset.FromUnixTimeSeconds(message.Date).DateTime;
				if (this.isPrivate)
				{
					// Set lang
					this.Language = new CultureInfo(this.User.LanguageCode ?? "en");
				}
				else
				{
					// Load group data
					this.Group = await this._db.Groups.GetGroupAsync(message.Chat, cancellationToken).ConfigureAwait(false);
					// Set lang
					this.Language = new CultureInfo(this.User.LangForce ? this.User.LanguageCode ?? "en" : this.Group.LanguageCode ?? "en");
				}
			}

			// If chat is private and message is not text
			if (this.isPrivate && !hasText)
			{
				if (message.ViaBot?.Id != this.Me.Id)
				{
					await LoadDataAsync().ConfigureAwait(false);
					// New search
					await this.SauceAsync(message, cancellationToken).ConfigureAwait(false);
				}
				return;
			}
			// Check if message constains text.
			if (hasText)
			{
				// If text == "sauce" or text is a metion for me (bot): Search
				if ((message.Text!.ToLowerInvariant() == "sauce" || message.Text.Contains($"@{this.Me.Username}")) && message.ReplyToMessage != default && !message.Text.StartsWith('/'))
				{
					await LoadDataAsync().ConfigureAwait(false); ;
					await this.SauceAsync(message.ReplyToMessage, cancellationToken).ConfigureAwait(false);
				}
				// If message is a command: process command
				else
				{
					var cmdMatch = this.Properties.CommandHelper.Match(message.Text);
					if (cmdMatch.Success)
					{
						await LoadDataAsync().ConfigureAwait(false); ;
						var parameters = string.IsNullOrEmpty(cmdMatch.Params) ? Array.Empty<string>() : cmdMatch.Params.Split(' ', StringSplitOptions.RemoveEmptyEntries);
						await this.OnCommandAsync(cmdMatch.Name, parameters, cancellationToken).ConfigureAwait(false);
					}
				}
			}
			// Check if message constains caption.
			else if (hasCaption)
			{
				var caption = message.Caption;
				var lowerCaption = caption!.ToLowerInvariant();
				// If caption == "sauce" or caption is a metion for me (bot): Search
				if (lowerCaption == "sauce" || caption.Contains($"@{this.Me.Username}") || lowerCaption == "/sauce")
				{
					await LoadDataAsync().ConfigureAwait(false); ;
					// New Search
					await this.SauceAsync(message, cancellationToken).ConfigureAwait(false);
				}
			}
		}
	}
}
