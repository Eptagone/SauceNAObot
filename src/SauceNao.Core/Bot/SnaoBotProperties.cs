// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Configuration;
using SauceNAO.Core.API;
using SauceNAO.Core.Tools;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

using File = System.IO.File;

namespace SauceNAO.Core;

/// <summary>Represents a set of basic bot configuration properties.</summary>
public sealed class SnaoBotProperties : IBotProperties
{
	/// <summary>
	/// Initialize a new instance of <see cref="SnaoBotProperties"/>.
	/// </summary>
	/// <param name="configuration"></param>
	public SnaoBotProperties(IConfiguration configuration)
	{
		// ---------------- TELEGRAM BOT ----------------
		var telegram = configuration.GetSection("Telegram");

		if (telegram == null)
		{
			throw new ArgumentNullException(nameof(configuration), "Missing config section: 'Telegram'");
		}

		var botToken = telegram["BotToken"];

		if (botToken == null)
		{
			throw new ArgumentNullException(nameof(configuration), "Missing config entry: 'Telegram:BotToken'");
		}

		this.Api = new BotClient(botToken);
		this.User = this.Api.GetMe();
		this.CommandHelper = new BotCommandHelper(this, true);

		// ---------------- SAUCENAO ----------------
		var snao = configuration.GetSection("SauceNAO");
		var apiKey = snao["ApiKey"];

		this.SnaoService = new SauceNaoApiService(
			OutputType.JsonApi,
			apiKey,
			false,
			default,
			default,
			999,
			default,
			Dedupe.AllImplementedDedupeMethodsSuchAsBySeriesName);

		if (telegram["SupportChatLink"] == null)
		{
			throw new ArgumentNullException(nameof(configuration), "Missing config value: 'Telegram:SupportChatLink'");
		}

		this.SupportChatLink = telegram["SupportChatLink"]!;

		// ---------------- WEBHOOK ----------------
		var webhookInfo = this.Api.GetWebhookInfo();
		if (!string.IsNullOrEmpty(webhookInfo.Url))
		{
			this.Api.DeleteWebhook(webhookInfo.PendingUpdateCount > 1000);
		}

		var applicationUrl = configuration["ApplicationUrl"];

		if (!string.IsNullOrEmpty(applicationUrl))
		{
			var secretToken = configuration["AccessToken"];
			var ffmpegExec = configuration["FFmpegExec"];
			var webhook = string.Format("{0}/bot", applicationUrl);

			if (ffmpegExec == null)
			{
				throw new ArgumentNullException(nameof(configuration), "Missing config value: 'FFmpegExec'");

			}

			this.TempFilesUrl = string.Format("{0}/temp/{{0}}", applicationUrl);
			this.FFmpeg = new FFmpeg(ffmpegExec);

			// Webhook configuration
			var webhookConfig = new SetWebhookArgs(webhook)
			{
				SecretToken = secretToken,
				MaxConnections = 60,
				AllowedUpdates = new string[] {
					AllowedUpdate.Message,
					AllowedUpdate.InlineQuery,
					AllowedUpdate.MyChatMember,
					AllowedUpdate.ChatMember
				}
			};

			// If a certificate was specified, it will be configured.
			var certPath = configuration["Certificate"];
			if (!string.IsNullOrEmpty(certPath))
			{
				var certBytes = File.ReadAllBytes(certPath);
				var filename = Path.GetFileName(certPath);
				var cert = new InputFile(certBytes, filename);

				webhookConfig.Certificate = cert;
			}

			this.Api.SetWebhook(webhookConfig);
			this.WebhookMode = true;
		}

		// ---------------- COMMANDS ----------------
		this.Api.DeleteMyCommands();

		if (this.WebhookMode) // Webhook Mode
		{
			// General Commands
			this.Api.SetMyCommands(new[] {
				new BotCommand("buymeacookie", "Buy me a Cookie"),
				new BotCommand("help", "Help"),
				new BotCommand("sauce", "Look for the image source"),
				new BotCommand("temp", "Create a temporary link")
			});

			// General Commands - ES
			this.Api.SetMyCommands(new[] {
				new BotCommand("comprameunagalleta", "Comprame una galleta"),
				new BotCommand("ayuda", "Ayuda"),
				new BotCommand("salsa", "Busca la salsa de la imagen"),
				new BotCommand("temp", "Crea un enlace temporal")
			}, languageCode: "es");

			// Private Commands
			this.Api.SetMyCommands(new[] {
				new BotCommand("buymeacookie", "Buy me a Cookie"),
				new BotCommand("help", "Help"),
				new BotCommand("history", "See your sauce history"),
				new BotCommand("sauce", "Look for the image source"),
				new BotCommand("setlang", "Set your language preferences"),
				new BotCommand("temp", "Create a temporary link")
			}, new BotCommandScopeAllPrivateChats());

			// Private Commands - ES
			this.Api.SetMyCommands(new[] {
				new BotCommand("donar", "Comprame una galleta"),
				new BotCommand("ayuda", "Ayuda"),
				new BotCommand("historial", "Mira tu historial de salsas"),
				new BotCommand("salsa", "Busca la salsa de la imagen"),
				new BotCommand("setlang", "Cambia tus preferencias de idioma"),
				new BotCommand("temp", "Crea un enlace temporal")
			}, new BotCommandScopeAllPrivateChats(), languageCode: "es");

			// Commands for chat administrators
			this.Api.SetMyCommands(new[] {
				new BotCommand("anticheats", "AntiCheats feature"),
				new BotCommand("buymeacookie", "Buy me a Cookie"),
				new BotCommand("help", "Help"),
				new BotCommand("sauce", "Look for the image source"),
				new BotCommand("setlang", "Set language preferences for current chat"),
				new BotCommand("temp", "Create a temporary link")
			}, new BotCommandScopeAllChatAdministrators());

			// Commands for chat addministrators - ES
			this.Api.SetMyCommands(new[] {
				new BotCommand("antitrampas", "Función ANti-Trampas"),
				new BotCommand("comprameunagalleta", "Comprame una galleta"),
				new BotCommand("ayuda", "Ayuda"),
				new BotCommand("salsa", "Busca la salsa de la imagen"),
				new BotCommand("setlang", "Cambia las preferencias de idioma para el chat actual"),
				new BotCommand("temp", "Crea un enlace temporal")
			}, new BotCommandScopeAllChatAdministrators(), "es");
		}
		else // Local Mode
		{
			// General Commands
			this.Api.SetMyCommands(new[] {
				new BotCommand("buymeacookie", "Buy me a Cookie"),
				new BotCommand("help", "Help"),
				new BotCommand("sauce", "Look for the image source")
			});

			// General Commands - ES
			this.Api.SetMyCommands(new[] {
				new BotCommand("comprameunagalleta", "Comprame una galleta"),
				new BotCommand("ayuda", "Ayuda"),
				new BotCommand("salsa", "Busca la salsa de la imagen")
			}, languageCode: "es");

			// Private Commands
			this.Api.SetMyCommands(new[] {
				new BotCommand("buymeacookie", "Buy me a Cookie"),
				new BotCommand("help", "Help"),
				new BotCommand("history", "See your sauce history"),
				new BotCommand("sauce", "Look for the image source"),
				new BotCommand("setlang", "Set your language preferences")
			}, new BotCommandScopeAllPrivateChats());

			// Private Commands - ES
			this.Api.SetMyCommands(new[] {
				new BotCommand("donar", "Comprame una galleta"),
				new BotCommand("ayuda", "Ayuda"),
				new BotCommand("historial", "Mira tu historial de salsas"),
				new BotCommand("salsa", "Busca la salsa de la imagen"),
				new BotCommand("setlang", "Cambia tus preferencias de idioma")
			}, new BotCommandScopeAllPrivateChats(), languageCode: "es");

			// Commands for chat administrators
			this.Api.SetMyCommands(new[] {
				new BotCommand("anticheats", "AntiCheats feature"),
				new BotCommand("buymeacookie", "Buy me a Cookie"),
				new BotCommand("help", "Help"),
				new BotCommand("sauce", "Look for the image source"),
				new BotCommand("setlang", "Set language preferences for current chat")
			}, new BotCommandScopeAllChatAdministrators());

			// Commands for chat addministrators - ES
			this.Api.SetMyCommands(new[] {
				new BotCommand("antitrampas", "Función ANti-Trampas"),
				new BotCommand("comprameunagalleta", "Comprame una galleta"),
				new BotCommand("ayuda", "Ayuda"),
				new BotCommand("salsa", "Busca la salsa de la imagen"),
				new BotCommand("setlang", "Cambia las preferencias de idioma para el chat actual")
			}, new BotCommandScopeAllChatAdministrators(), "es");
		}
	}

	/// <summary>Sauce Nao Api Service</summary>
	public SauceNaoApiService SnaoService { get; }

	/// <summary>Address to recover files. Used to recover temporary files. Example: https://saucenaobot.azurewebsites.net/temp/{0}</summary>
	public string? TempFilesUrl { get; }
	public FFmpeg? FFmpeg { get; }
	public string SupportChatLink { get; }
	public bool WebhookMode { get; }

	public BotClient Api { get; }
	public User User { get; }
	public BotCommandHelper CommandHelper { get; }

	IBotCommandHelper IBotProperties.CommandHelper => this.CommandHelper;
}
