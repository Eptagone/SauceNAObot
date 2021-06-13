// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

#nullable disable

namespace SauceNao.Data.Models
{
    /// <summary>Chat Model</summary>
    [Table("AppChat")]
    public partial class AppChat : IEquatable<AppChat>
    {
        /// <summary>Initialize a new instance of AppChat</summary>
        public AppChat()
        {
            AntiCheats = new HashSet<AntiCheat>();
        }

        internal AppChat(Chat chat, string lang = "en") : this()
        {
            Id = chat.Id;
            Title = chat.Title;
            Username = chat.Username;
            if (string.IsNullOrEmpty(chat.InviteLink) && !string.IsNullOrEmpty(Username))
            {
                InviteLink = "https://t.me/" + Username;
            }
            else
            {
                InviteLink = chat.InviteLink;
            }
            Lang = lang ?? "en";
        }

        /// <summary>The AppChat Id.</summary>
        [Key]
        public int ChatId { get; set; }
        /// <summary>The Chat Id.</summary>
        public long Id { get; set; }
        /// <summary>Chat title.</summary>
        [Required]
        [StringLength(128)]
        public string Title { get; set; }
        /// <summary>Chat Username.</summary>
        [StringLength(32)]
        public string Username { get; set; }
        /// <summary>Optional. Chat InviteLink.</summary>
        public string InviteLink { get; set; }
        /// <summary>Chat Languaje Code.</summary>
        [Required]
        [StringLength(8)]
        public string Lang { get; set; }
        /// <summary>Anticheats of chat.</summary>

        [InverseProperty(nameof(AntiCheat.Chat))]
        public virtual ICollection<AntiCheat> AntiCheats { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override bool Equals(object obj)
        {
            return Equals(obj as AppChat);
        }

        public bool Equals(AppChat other)
        {
            return other != null &&
                   ChatId == other.ChatId &&
                   Id == other.Id &&
                   Title == other.Title &&
                   Username == other.Username &&
                   InviteLink == other.InviteLink &&
                   Lang == other.Lang &&
                   EqualityComparer<ICollection<AntiCheat>>.Default.Equals(AntiCheats, other.AntiCheats);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ChatId, Id, Title, Username, InviteLink, Lang, AntiCheats);
        }

        public void Update(Chat chat)
        {
            Id = chat.Id;
            Title = chat.Title;
            Username = chat.Username;
            if (string.IsNullOrEmpty(chat.InviteLink) && !string.IsNullOrEmpty(Username))
            {
                InviteLink = "https://t.me/" + Username;
            }
            else
            {
                InviteLink = chat.InviteLink;
            }
        }

        public static bool operator ==(AppChat left, AppChat right)
        {
            return EqualityComparer<AppChat>.Default.Equals(left, right);
        }

        public static bool operator !=(AppChat left, AppChat right)
        {
            return !(left == right);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
