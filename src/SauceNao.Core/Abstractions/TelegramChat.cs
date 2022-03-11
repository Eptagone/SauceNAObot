// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Abstractions
{
    public abstract class TelegramChat : ITelegramChat
    {
        public TelegramChat()
        {
        }

        public TelegramChat(ITelegramChat chat)
        {
            Id = chat.Id;
            Title = chat.Title;
            Username = chat.Username;
            Description = chat.Description;
            InviteLink = chat.InviteLink;
            LinkedChatId = chat.LinkedChatId;
            Type = chat.Type;
        }

        public long Id { get; set; }
        [Required]
        [StringLength(128)]
        public string Title { get; set; } = null!;
        public string Type { get; set; } = null!;
        [StringLength(32)]
        public string? Username { get; set; }
        public string? Description { get; set; }
        public string? InviteLink { get; set; }
        public long? LinkedChatId { get; set; }
    }
}
