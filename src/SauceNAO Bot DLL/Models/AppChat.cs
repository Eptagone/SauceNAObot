// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Models
{
    [Table("Chats")]
    public sealed class AppChat
    {
        public AppChat() { }
        public AppChat(Chat chat)
        {
            Id = chat.Id;
            Title = chat.Title ?? string.Empty;
            Username = chat.Username ?? string.Empty;
            InviteLink = chat.InviteLink ?? (string.IsNullOrEmpty(Username) ? string.Empty : $"https://t.me/{Username}");
            IsChannel = chat.Type == ChatType.Channel;
        }

        [Key]
        public int Key { get; set; }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public string InviteLink { get; set; }
        public bool IsChannel { get; set; }
        public bool AntiCheats { get; set; }

        public List<Whitelist> Whitelist { get; } = new List<Whitelist>();

        internal bool NotEquals(Chat chat)
        {
            string _title = chat.Title ?? string.Empty;
            string _username = chat.Username ?? string.Empty;
            string _invitelink = chat.InviteLink ?? (string.IsNullOrEmpty(Username) ? string.Empty : $"https://t.me/{Username}");
            if (Title == _title && Username == _username && InviteLink == _invitelink)
            {
                return false;
            }
            else
            {
                Title = _title;
                Username = _username;
                InviteLink = _invitelink;
                return true;
            }
        }
    }
}
