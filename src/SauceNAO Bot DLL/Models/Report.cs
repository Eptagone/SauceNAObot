// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System;
using System.Text.Json;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Models
{
    public class Report
    {
        public Report() { }
        internal Report(Exception exp)
        {
            Description = exp.Message;
            Details = exp.ToString();
        }
        internal Report(Exception exp, Message message, string details)
        {
            Description = exp.Message;
            Details = details;
            JsonMessage = JsonSerializer.Serialize(message);
        }
        internal Report(BotRequestException exp, Update update)
        {
            ErrorCode = exp.ErrorCode;
            Description = exp.Message;
            JsonMessage = JsonSerializer.Serialize(update);
        }
        internal Report(BotRequestException exp, Message message, string details)
        {
            ErrorCode = exp.ErrorCode;
            Description = exp.Message;
            Details = details;
            JsonMessage = JsonSerializer.Serialize(message);
        }

        internal Report(Message message, string text, string details)
        {
            Description = text;
            Details = details;
            JsonMessage = JsonSerializer.Serialize(message);
        }

        public int Id { get; set; }

        public string IsBotException { get; set; }
        public int ErrorCode { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string JsonMessage { get; set; }
    }
}
