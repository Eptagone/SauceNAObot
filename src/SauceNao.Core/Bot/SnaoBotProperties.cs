// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.API;
using SauceNAO.Core.Tools;
using System.Runtime.InteropServices;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

using File = System.IO.File;

namespace SauceNAO.Core
{
    /// <summary>Represents a set of basic bot configuration properties.</summary>
    public sealed class SnaoBotProperties : BotProperties
    {
        /// <summary>Initialize a new instance of <see cref="SnaoBotProperties"/>.</summary>
        /// <param name="bot">The Telegram Bot Client.</param>
        /// <param name="apiKey">Saucenao ApiKey.</param>
        /// <param name="tempFilesUrl">Temporal files Url.</param>
        /// <param name="ffmpegPath">Ffmpeg path.</param>
        /// <param name="supportChatLink">Support chat link.</param>
        public SnaoBotProperties(BotClient bot, string apiKey, string supportChatLink) : base(bot)
        {
            SnaoService = new SauceNaoApiService(
                OutputType.JsonApi,
                apiKey,
                false,
                default,
                default,
                999,
                default,
                Dedupe.AllImplementedDedupeMethodsSuchAsBySeriesName);

            SupportChatLink = supportChatLink;
        }

        /// <summary>Initialize a new instance of <see cref="SnaoBotProperties"/>.</summary>
        /// <param name="botToken">Token granted by BotFather. Required to access the Telegram bot API.</param>
        /// <param name="apiKey">Saucenao ApiKey.</param>
        /// <param name="ffmpegPath">Ffmpeg path.</param>
        /// <param name="supportChatLink">Support chat link.</param>
        /// <param name="httpClient">Provide a specific HttpClient for this instance of BotClient.</param>
        public SnaoBotProperties(string botToken, string apiKey, string supportChatLink, [Optional] HttpClient httpClient) : this(new BotClient(botToken, httpClient), apiKey, supportChatLink)
        {
        }

        /// <summary>Initialize a new instance of <see cref="SnaoBotProperties"/>.</summary>
        /// <param name="bot">The Telegram Bot Client.</param>
        /// <param name="apiKey">Saucenao ApiKey.</param>
        /// <param name="tempFilesUrl">Temporal files Url.</param>
        /// <param name="ffmpegPath">Ffmpeg path.</param>
        /// <param name="supportChatLink">Support chat link.</param>
        public SnaoBotProperties(BotClient bot, string apiKey, string tempFilesUrl, string ffmpegPath, string supportChatLink) : this(bot, apiKey, supportChatLink)
        {
            FFmpeg = new FFmpeg(ffmpegPath);
            TempFilesUrl = tempFilesUrl;
            WebhookMode = true;
        }

        /// <summary>Initialize a new instance of <see cref="SnaoBotProperties"/>.</summary>
        /// <param name="botToken">Token granted by BotFather. Required to access the Telegram bot API.</param>
        /// <param name="apiKey">Saucenao ApiKey.</param>
        /// <param name="tempFilesUrl">Temporal files Url.</param>
        /// <param name="ffmpegPath">Ffmpeg path.</param>
        /// <param name="supportChatLink">Support chat link.</param>
        /// <param name="httpClient">Provide a specific HttpClient for this instance of BotClient.</param>
        public SnaoBotProperties(string botToken, string apiKey, string tempFilesUrl, string ffmpegPath, string supportChatLink, [Optional] HttpClient httpClient) : this(new BotClient(botToken, httpClient), apiKey, tempFilesUrl, ffmpegPath, supportChatLink)
        {
        }

        /// <summary>Sauce Nao Api Service</summary>
        public SauceNaoApiService SnaoService { get; }

        /// <summary>Address to recover files. Used to recover temporary files. Example: https://saucenaobot.azurewebsites.net/temp/{0}</summary>
        public string? TempFilesUrl { get; }
        public FFmpeg? FFmpeg { get; }
        public string SupportChatLink { get; }
        public bool WebhookMode { get; }

        public void Initialize([Optional] string? webhookUrl, [Optional] string? certPath)
        {
            Api.DeleteWebhook(true); // Delete old webhook
            if (!string.IsNullOrEmpty(webhookUrl))
            {
                var options = new SetWebhookArgs()
                {
                    Url = webhookUrl,
                    MaxConnections = 60,
                    AllowedUpdates = new string[] {
                        AllowedUpdate.Message,
                        AllowedUpdate.InlineQuery,
                        AllowedUpdate.MyChatMember,
                        AllowedUpdate.ChatMember
                    }
                };

                if (!string.IsNullOrEmpty(certPath))
                {
                    var certBytes = File.ReadAllBytes(certPath);
                    var filename = Path.GetFileName(certPath);
                    var cert = new InputFile(certBytes, filename);
                    options.Certificate = cert;
                }

                Api.SetWebhook(options); // Set new webhook
            }
        }
        public void SetBotCommands()
        {
            // Delete old commands
            Api.DeleteMyCommands();

            if (!WebhookMode) // Local Mode
            {
                // General Commands
                Api.SetMyCommands(new[] {
                    new BotCommand("buymeacookie", "Buy me a Cookie"),
                    new BotCommand("help", "Help"),
                    new BotCommand("sauce", "Look for the image source")
                });

                // General Commands - ES
                Api.SetMyCommands(new[] {
                    new BotCommand("comprameunagalleta", "Comprame una galleta"),
                    new BotCommand("ayuda", "Ayuda"),
                    new BotCommand("salsa", "Busca la salsa de la imagen")
                }, languageCode: "es");

                // Private Commands
                Api.SetMyCommands(new[] {
                    new BotCommand("buymeacookie", "Buy me a Cookie"),
                    new BotCommand("help", "Help"),
                    new BotCommand("history", "See your sauce history"),
                    new BotCommand("sauce", "Look for the image source"),
                    new BotCommand("setlang", "Set your language preferences")
                }, new BotCommandScopeAllPrivateChats());

                // Private Commands - ES
                Api.SetMyCommands(new[] {
                    new BotCommand("donar", "Comprame una galleta"),
                    new BotCommand("ayuda", "Ayuda"),
                    new BotCommand("historial", "Mira tu historial de salsas"),
                    new BotCommand("salsa", "Busca la salsa de la imagen"),
                    new BotCommand("setlang", "Cambia tus preferencias de idioma")
                }, new BotCommandScopeAllPrivateChats(), languageCode: "es");

                // Commands for chat administrators
                Api.SetMyCommands(new[] {
                    new BotCommand("anticheats", "AntiCheats feature"),
                    new BotCommand("buymeacookie", "Buy me a Cookie"),
                    new BotCommand("help", "Help"),
                    new BotCommand("sauce", "Look for the image source"),
                    new BotCommand("setlang", "Set language preferences for current chat")
                }, new BotCommandScopeAllChatAdministrators());

                // Commands for chat addministrators - ES
                Api.SetMyCommands(new[] {
                    new BotCommand("antitrampas", "Función ANti-Trampas"),
                    new BotCommand("comprameunagalleta", "Comprame una galleta"),
                    new BotCommand("ayuda", "Ayuda"),
                    new BotCommand("salsa", "Busca la salsa de la imagen"),
                    new BotCommand("setlang", "Cambia las preferencias de idioma para el chat actual")
                }, new BotCommandScopeAllChatAdministrators(), "es");
            }
            else // Webhook Mode
            {
                // General Commands
                Api.SetMyCommands(new[] {
                    new BotCommand("buymeacookie", "Buy me a Cookie"),
                    new BotCommand("help", "Help"),
                    new BotCommand("sauce", "Look for the image source"),
                    new BotCommand("temp", "Create a temporary link")
                });

                // General Commands - ES
                Api.SetMyCommands(new[] {
                    new BotCommand("comprameunagalleta", "Comprame una galleta"),
                    new BotCommand("ayuda", "Ayuda"),
                    new BotCommand("salsa", "Busca la salsa de la imagen"),
                    new BotCommand("temp", "Crea un enlace temporal")
                }, languageCode: "es");

                // Private Commands
                Api.SetMyCommands(new[] {
                    new BotCommand("buymeacookie", "Buy me a Cookie"),
                    new BotCommand("help", "Help"),
                    new BotCommand("history", "See your sauce history"),
                    new BotCommand("sauce", "Look for the image source"),
                    new BotCommand("setlang", "Set your language preferences"),
                    new BotCommand("temp", "Create a temporary link")
                }, new BotCommandScopeAllPrivateChats());

                // Private Commands - ES
                Api.SetMyCommands(new[] {
                    new BotCommand("donar", "Comprame una galleta"),
                    new BotCommand("ayuda", "Ayuda"),
                    new BotCommand("historial", "Mira tu historial de salsas"),
                    new BotCommand("salsa", "Busca la salsa de la imagen"),
                    new BotCommand("setlang", "Cambia tus preferencias de idioma"),
                    new BotCommand("temp", "Crea un enlace temporal")
                }, new BotCommandScopeAllPrivateChats(), languageCode: "es");

                // Commands for chat administrators
                Api.SetMyCommands(new[] {
                    new BotCommand("anticheats", "AntiCheats feature"),
                    new BotCommand("buymeacookie", "Buy me a Cookie"),
                    new BotCommand("help", "Help"),
                    new BotCommand("sauce", "Look for the image source"),
                    new BotCommand("setlang", "Set language preferences for current chat"),
                    new BotCommand("temp", "Create a temporary link")
                }, new BotCommandScopeAllChatAdministrators());

                // Commands for chat addministrators - ES
                Api.SetMyCommands(new[] {
                    new BotCommand("antitrampas", "Función ANti-Trampas"),
                    new BotCommand("comprameunagalleta", "Comprame una galleta"),
                    new BotCommand("ayuda", "Ayuda"),
                    new BotCommand("salsa", "Busca la salsa de la imagen"),
                    new BotCommand("setlang", "Cambia las preferencias de idioma para el chat actual"),
                    new BotCommand("temp", "Crea un enlace temporal")
                }, new BotCommandScopeAllChatAdministrators(), "es");
            }
        }
    }
}
