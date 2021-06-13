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
    /// <summary>AntiCheat model</summary>
    [Table("AntiCheat")]
    public partial class AntiCheat : IEquatable<AntiCheat>
    {
        /// <summary>Initialize a new instance of AntiCheats</summary>
        public AntiCheat() { }
        internal AntiCheat(string username, long userId)
        {
            Username = username.TrimStart('@');
            AddedByUserId = userId;
        }
        internal AntiCheat(User bot, long userId)
        {
            Id = bot.Id;
            Username = bot.Username;
            AddedByUserId = userId;
        }
        /// <summary>The AntiCheat Id</summary>
        [Key]
        public int AntiCheattd { get; set; }
        /// <summary>The AntiCheat chat Id.</summary>
        public int ChatId { get; set; }
        /// <summary>The user Id who added the AntiCheats registry.</summary>
        public long AddedByUserId { get; set; }
        /// <summary>Optional. Telegram user ID that was added to the AntiCheats registry.</summary>
        public long? Id { get; set; }
        /// <summary>Username of the user who was added to the AntiCheats registry.</summary>
        [Required]
        [StringLength(32)]
        public string Username { get; set; }

        /// <summary>User who added the AntiCheats registry.</summary>
        [ForeignKey(nameof(AddedByUserId))]
        [InverseProperty(nameof(AppUser.AntiCheats))]
        public virtual AppUser AddedByUser { get; set; }
        /// <summary>User who was added to the AntiCheats registry.</summary>
        [ForeignKey(nameof(ChatId))]
        [InverseProperty(nameof(AppChat.AntiCheats))]
        public virtual AppChat Chat { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override bool Equals(object obj)
        {
            return Equals(obj as AntiCheat);
        }

        public bool Equals(AntiCheat other)
        {
            return other != null &&
                   AntiCheattd == other.AntiCheattd &&
                   ChatId == other.ChatId &&
                   AddedByUserId == other.AddedByUserId &&
                   Id == other.Id &&
                   Username == other.Username &&
                   EqualityComparer<AppUser>.Default.Equals(AddedByUser, other.AddedByUser) &&
                   EqualityComparer<AppChat>.Default.Equals(Chat, other.Chat);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AntiCheattd, ChatId, AddedByUserId, Id, Username, AddedByUser, Chat);
        }

        public static bool operator ==(AntiCheat left, AntiCheat right)
        {
            return EqualityComparer<AntiCheat>.Default.Equals(left, right);
        }

        public static bool operator !=(AntiCheat left, AntiCheat right)
        {
            return !(left == right);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
