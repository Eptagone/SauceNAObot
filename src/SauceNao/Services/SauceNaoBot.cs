// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using SauceNao.API;
using SauceNao.Data;
using System;
using System.Net.Http;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNao.Services
{
    /// <summary>The SauceNao bot class.</summary>
    public partial class SauceNaoBot : TelegramBotAsync
    {
        private const string SupportChatUrl = "https://t.me/joinchat/8NJMCbRmiTk2Yjkx";
        /// <summary>Address to recover files. Used to recover temporary files. Example: https://saucenaobot.azurewebsites.net/temp/{0}</summary>
        private static string TempFilesPath { get; set; }
        private static HttpClient Client { get; set; } = new HttpClient();
        private static readonly FFmpeg ffmpeg = new(@"FFmpeg/ffmpeg.exe");

        internal static BotClient Bot;
        private static User Me;
        private static SauceNaoApiService SnaoService;
        private static string cmdPattern;

        private readonly SauceNaoContext DB;
        private readonly ILogger<SauceNaoBot> Logger;

        /// <summary>New SauceNAOBot instance.</summary>
        /// <param name="logger">The Logger</param>
        /// <param name="context">SauceNAO Context.</param>
        public SauceNaoBot(ILogger<SauceNaoBot> logger, SauceNaoContext context)
        {
            Logger = logger;
            DB = context;
        }

        /// <summary>Bot setup</summary>
        /// <param name="botToken">Bot Token</param>
        /// <param name="apiKey">Your saucenao ApiKey</param>
        /// <param name="webhookUrl">Webhook url</param>
        /// <param name="webhookToken">The webhook Token</param>
        public static void InitialSetup(string botToken, string apiKey, string webhookUrl, string webhookToken)
        {
            // Setup Utilities HttpClient
            TempFilesPath = webhookUrl + "temp/{0}"; // Save temp files url
            Client.BaseAddress = new Uri(webhookUrl);
            Client.Timeout = new TimeSpan(0, 1, 30);

            Bot = new BotClient(botToken, Client); // Initialize the Bot Client instance
            Me = Bot.GetMe(); // Get bot user.
            cmdPattern = string.Format(@"^\/(?<cmd>\w*)(?:$|@{0}$)", Me.Username);
            SnaoService = new SauceNaoApiService(
                OutputType.JsonApi,
                apiKey,
                false,
                default,
                default,
                999,
                default,
                Dedupe.AllImplementedDedupeMethodsSuchAsBySeriesName);

            Bot.SetMyCommands( // Set Commands
                new BotCommand("anticheats", "AntiCheats feature"),
                new BotCommand("buymeacookie", "Buy me a Cookie"),
                new BotCommand("help", "Help"),
                new BotCommand("history", "See your sauce history"),
                new BotCommand("sauce", "Look for the image source"),
                new BotCommand("stats", "Global usage statistics"),
                new BotCommand("temp", "Create a temporary link to an image"));

            Bot.DeleteWebhook(true); // Delete old webhook
            Bot.SetWebhook(webhookUrl + webhookToken, maxConnections: 40, allowedUpdates: Array.Empty<string>(), dropPendingUpdates: true); // Set new webhook
        }
    }
}
