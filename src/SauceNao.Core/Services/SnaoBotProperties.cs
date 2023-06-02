// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SauceNAO.Core.API;
using SauceNAO.Core.Tools;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

using File = System.IO.File;

namespace SauceNAO.Core.Services;

/// <summary>Represents a set of basic bot configuration properties.</summary>
public sealed class SnaoBotProperties : IBotProperties
{
	/// <summary>
	/// Initialize a new instance of <see cref="SnaoBotProperties"/>.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="configuration">The configuration.</param>
	public SnaoBotProperties(ILogger<SnaoBotProperties> logger, IConfiguration configuration)
	{
		// Load configuration options
		var options = configuration
				.GetRequiredSection("SNAO")
				.Get<SnaoOptions>() ?? throw new ArgumentNullException(nameof(configuration), "Missing config section: 'SNAO'");

		// ---------------- TELEGRAM BOT ----------------
		var botToken = options.BotToken ?? throw new ArgumentNullException(nameof(configuration), "Missing config entry: 'SNAO:BotToken'");

		this.Api = new BotClient(botToken);
		this.User = this.Api.GetMe();
		this.CommandHelper = new BotCommandHelper(this, true);

		// ---------------- SAUCENAO ----------------
		this.SnaoService = new SauceNaoApiService(
			OutputType.JsonApi,
			options.ApiKey,
			false,
			default,
			default,
			999,
			default,
			Dedupe.AllImplementedDedupeMethodsSuchAsBySeriesName);
		// Save the support chat link
		this.SupportChatLink = options.SupportChatLink ?? throw new ArgumentNullException(nameof(configuration), "Missing config value: 'SNAO:SupportChatLink'");

		// ---------------- TEMP FILES AND WEBHOOK ----------------
		var webhookInfo = this.Api.GetWebhookInfo();
		// If a previous webhook was configured, it will be deleted.
		if (!string.IsNullOrEmpty(webhookInfo.Url))
		{
			// Drop pending updates if there are more than 2000 pending updates or if it was specified in the configuration.
			this.Api.DeleteWebhook(options.DropPendingUpdates || webhookInfo.PendingUpdateCount > 2000);
		}

		// Enable or disable local temp files.
		this.LocalMode = string.IsNullOrEmpty(options.ApplicationUrl);

		// If local mode is not enabled, configure the temporal files url.
		if (!this.LocalMode)
		{
			// Save the temporary files url.
			this.TempFilesUrl = string.Format("{0}/temp/{{0}}", options.ApplicationUrl);

			// If the FFmpeg executable was not specified, local mode will be enabled.
			if (string.IsNullOrEmpty(options.FFmpegExec))
			{
				logger.LogWarning("Local Mode enabled by default because the FFmpeg executable was not specified.");
				this.LocalMode = true;
			}
			else
			{
				this.FFmpeg = new FFmpeg(options.FFmpegExec);

				// If the secret token was specified, the webhook will be configured.
				if (!string.IsNullOrEmpty(options.SecretToken))
				{
					var webhook = string.Format("{0}/bot", options.ApplicationUrl);

					// Webhook configuration
					var webhookConfig = new SetWebhookArgs(webhook)
					{
						SecretToken = options.SecretToken,
						MaxConnections = 60,
						AllowedUpdates = new string[] {
							AllowedUpdate.Message,
							AllowedUpdate.InlineQuery,
							AllowedUpdate.MyChatMember,
							AllowedUpdate.ChatMember
						}
					};
					// If a certificate was specified, it will be added to the webhook configuration.
					var certPath = options.CertificatePath;
					if (!string.IsNullOrEmpty(certPath))
					{
						var certBytes = File.ReadAllBytes(certPath);
						var filename = Path.GetFileName(certPath);
						var cert = new InputFile(certBytes, filename);

						webhookConfig.Certificate = cert;
					}

					// Set the webhook.
					this.Api.SetWebhook(webhookConfig);
				}
			}
		}

		// ---------------- COMMANDS ----------------
		this.Api.DeleteMyCommands();

		// Commands for Local Mode
		if (this.LocalMode)
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
		// Commands for non-local mode
		else
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
	}

	/// <summary>Sauce Nao Api Service</summary>
	public SauceNaoApiService SnaoService { get; }

	/// <summary>Address to recover files. Used to recover temporary files. Example: https://saucenaobot.azurewebsites.net/temp/{0}</summary>
	public string? TempFilesUrl { get; }
	public FFmpeg? FFmpeg { get; }
	public string SupportChatLink { get; }

	/// <summary>
	/// Local Mode is used to disable some features that are not available in local mode like temp files.
	/// </summary>
	public bool LocalMode { get; }

	public BotClient Api { get; }
	public User User { get; }
	public BotCommandHelper CommandHelper { get; }

	IBotCommandHelper IBotProperties.CommandHelper => this.CommandHelper;
}
