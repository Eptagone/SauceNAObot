// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using SauceNAO.Core.Entities;
using System.Globalization;
using Telegram.BotAPI;

#nullable disable

namespace SauceNAO.Core
{
    /// <summary>The SauceNao bot class.</summary>
    public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
    {
        private static readonly HttpClient httpClient = new();

        private readonly ISauceDatabase _db;
        private readonly ILogger<SauceNaoBot> _logger;

        /// <summary>New SauceNAOBot instance.</summary>
        /// <param name="logger">The Logger</param>
        /// <param name="context">SauceNAO Context.</param>
        public SauceNaoBot(ISauceDatabase db, SnaoBotProperties botConfiguration, ILogger<SauceNaoBot> logger) : base(botConfiguration)
        {
            this._db = db;
            this._logger = logger;
        }

        /// <summary>Current user data instance.</summary>
        private UserData User;
        /// <summary>Current chat data instance.</summary>
        private TelegramGroup Group;
        /// <summary>Current date instance.</summary>
        private DateTime Date;
        /// <summary>Current lang instance.</summary>
        private CultureInfo Language;
        /// <summary>Current chat is a private chat.</summary>
        private bool isPrivate;
    }
}
