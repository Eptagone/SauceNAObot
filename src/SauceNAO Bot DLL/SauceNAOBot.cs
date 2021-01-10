// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using SauceNAO.Data;
using System;
using System.Net.Http.Headers;
using Telegram.BotAPI;
using Telegram.BotAPI.Available_Methods;
using Telegram.BotAPI.Available_Types;
using Telegram.BotAPI.Getting_updates;

namespace SauceNAO
{
    public partial class SauceNAOBot
    {
        private const string BotToken = "BOT:TOKEN";
        /// <summary>Public saucenao ApiKey</summary>
        public const string PublicApiKey = "";

        private readonly SauceNAOContext DB;
        private static readonly BotClient Bot = new BotClient(BotToken);
        private static readonly string Me = Bot.GetMe().Username;

        /// <summary>New SauceNAOBot instance.</summary>
        /// <param name="context">SauceNAO Context.</param>
        public SauceNAOBot(SauceNAOContext context)
        {
            DB = context;
        }

        /// <summary>My Commands</summary>
        private static readonly BotCommand[] BotCommands = new BotCommand[]
            {
                new BotCommand(){Command = "anticheats", Description = "AntiCheats feature"},
                new BotCommand(){Command = "help", Description = "Help"},
                new BotCommand(){Command = "mysauce", Description = "See your sauce history"},
                new BotCommand(){Command = "sauce", Description = "Look for the image source"},
                new BotCommand(){Command = "temp", Description = "Create a temporary link to an image"}
            };
        /// <summary>Webhook setup</summary>
        /// <param name="webhook_urlpath"></param>
        public static void WebHookSetup(string baseUrl, string securityToken)
        {
            // Setup Utilities HttpClient
            Utilities.BaseUrl = baseUrl; // Save base url
            Utilities.TempFilesPath = baseUrl + "temp?file={0}"; // Save temp files url
            Utilities.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Utilities.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            Utilities.Client.BaseAddress = new Uri(baseUrl);
            Utilities.Client.Timeout = new TimeSpan(0, 1, 30);

            // Setup Bot
            BotClient.SetHttpClient(Utilities.Client);
            Bot.DeleteWebhook(true); // Delete old webhook
            Bot.SetMyCommands(BotCommands);

            Bot.SetWebhook($"{baseUrl}{securityToken}", allowed_updates: Array.Empty<string>(), max_connections: 40); // Set new webhook
        }
    }
}
