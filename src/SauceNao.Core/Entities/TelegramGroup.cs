// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Entities
{
    /// <summary>Telegram Group</summary>
    [Table("Group", Schema = "tg")]
    public partial class TelegramGroup : TelegramChat
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
        /// <summary>Chat Language Code.</summary>
        [StringLength(8)]
        public string? LanguageCode { get; set; }

        /// <summary>Anticheats of chat.</summary>
        [InverseProperty(nameof(AntiCheat.Group))]
        public virtual ICollection<AntiCheat> AntiCheats { get; set; }
    }
}
