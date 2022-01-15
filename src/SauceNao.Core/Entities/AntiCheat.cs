// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

#nullable disable

namespace SauceNAO.Core.Entities
{
    /// <summary>AntiCheat model</summary>
    [Table("AntiCheat")]
    public partial class AntiCheat
    {
        /// <summary>Initialize a new instance of AntiCheats</summary>
        public AntiCheat() { }
        internal AntiCheat(ITelegramUser user, long userId)
        {
            if (!user.IsBot)
            {
                throw new ArgumentException("The telegram user mush be a bot.");
            }
            BotId = user.Id;
            AddedByUserId = userId;
        }
        /// <summary>The AntiCheat Primary Key</summary>
        [Key]
        public int Key { get; set; }
        /// <summary>The Group Key.</summary>
        [Required]
        public int ChatKey { get; set; }
        /// <summary>Optional. Unique identifier of the bot that was added to the AntiCheats registry.</summary>
        [Required]
        public long BotId { get; set; }

        /// <summary>The user Id who added the AntiCheats registry.</summary>
        [Required]
        public long AddedByUserId { get; set; }

        /// <summary>User who added the AntiCheats registry.</summary>
        [ForeignKey(nameof(AddedByUserId))]
        [InverseProperty(nameof(UserData.AntiCheats))]
        public virtual UserData AddedByUser { get; set; }
        /// <summary>User who was added to the AntiCheats registry.</summary>
        [ForeignKey(nameof(ChatKey))]
        [InverseProperty(nameof(TelegramGroup.AntiCheats))]
        public virtual TelegramGroup Group { get; set; }
    }
}
