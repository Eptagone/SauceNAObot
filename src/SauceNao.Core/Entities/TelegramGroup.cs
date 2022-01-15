// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Entities
{
    /// <summary>Telegram Group</summary>
    [Table("Group")]
    public partial class TelegramGroup : ITelegramChat
    {
        /// <summary>Initialize a new instance of AppChat</summary>
        public TelegramGroup()
        {
            Title = string.Empty;
            Type = string.Empty;
            AntiCheats = new HashSet<AntiCheat>();
        }

        public TelegramGroup(ITelegramChat chat)
        {
            if (chat == default)
            {
                throw new ArgumentNullException(nameof(chat));
            }
            Id = chat.Id;
            Title = chat.Title ?? "Chat desconocido";
            Description = chat.Description ?? string.Empty;
            Username = chat.Username ?? string.Empty;
            InviteLink = chat.InviteLink ?? (string.IsNullOrEmpty(Username) ? string.Empty : $"https://t.me/{Username}");
            Type = chat.Type;

            AntiCheats = new HashSet<AntiCheat>();
        }

        /// <summary>The AppChat Id.</summary>
        [Key]
        public int Key { get; set; }
        public long Id { get; set; }
        [Required]
        [StringLength(128)]
        public string Title { get; set; }
        public string? Description { get; set; }
        /// <summary>Chat Username.</summary>
        [StringLength(32)]
        public string? Username { get; set; }
        /// <summary>Optional. Chat InviteLink.</summary>
        public string? InviteLink { get; set; }
        /// <summary>Chat Language Code.</summary>
        [StringLength(8)]
        public string? LanguageCode { get; set; }
        public long? LinkedChatId { get; set; }
        public string Type { get; set; }

        /// <summary>Anticheats of chat.</summary>
        [InverseProperty(nameof(AntiCheat.Group))]
        public virtual ICollection<AntiCheat> AntiCheats { get; set; }

        /// <summary>
        /// Update current chat instance using another chat model.
        /// </summary>
        /// <param name="chat">Chat model to compare changes.</param>
        /// <returns>True, if the chat information has changed and has been updated.</returns>
        public virtual bool UpdateInfo(ITelegramChat chat)
        {
            if (chat == default)
            {
                throw new ArgumentNullException(nameof(chat));
            }

            bool output = false;
            long id = chat.Id;
            string title = chat.Title ?? "Unknown chat's title";
            string description = chat.Description ?? string.Empty;
            string username = chat.Username ?? string.Empty;
            string inviteLink = chat.InviteLink ?? (string.IsNullOrEmpty(Username) ? string.Empty : $"https://t.me/{Username}");
            if (id == chat.Id || Title != title || Description != description || Username != username || InviteLink != inviteLink || Type != chat.Type)
            {
                output = true;
                Id = id;
                Title = title;
                Description = description;
                Username = username;
                InviteLink = inviteLink;
                Type = chat.Type;
            }
            return output;
        }
    }
}
