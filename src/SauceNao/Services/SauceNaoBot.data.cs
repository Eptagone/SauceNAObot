// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNao.Data.Models;
using System;
using System.Globalization;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNao.Services
{
    public partial class SauceNaoBot : TelegramBotAsync
    {
        /// <summary>Current update instance.</summary>
        private Update cUpdate;
        /// <summary>Current message instance.</summary>
        private Message cMessage;
        /// <summary>Current user data instance.</summary>
        private AppUser cUser;
        /// <summary>Current chat data instance.</summary>
        private AppChat cGroup;
        /// <summary>Current date instance.</summary>
        private DateTime dateTime;
        /// <summary>Current lang instance.</summary>
        private CultureInfo cLang;
        /// <summary>Current chat is a private chat.</summary>
        private bool cIsPrivate;
    }
}
