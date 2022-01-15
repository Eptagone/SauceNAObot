// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace SauceNAO.Core.Entities
{
    /// <summary>Represents a sauce that was searched by a user. A user history item.</summary>
    [Table("UserSauce")]
    public partial class UserSauce
    {
        /// <summary>Initialize a nes instance of UserSauce.</summary>
        public UserSauce() { }

        internal UserSauce(int sauceId, DateTime dateTime)
        {
            Date = dateTime;
            SauceId = sauceId;
        }

        /// <summary>User sauce Key.</summary>
        [Key]
        public int Key { get; set; }
        /// <summary>Sauce Id.</summary>
        [Required]
        public int SauceId { get; set; }
        /// <summary>User Id.</summary>
        [Required]
        public long UserId { get; set; }
        /// <summary>Sauce Date.</summary>
        public DateTime Date { get; set; }

        /// <summary>The Sauce.</summary>
        [ForeignKey(nameof(SauceId))]
        [InverseProperty(nameof(SuccessfulSauce.UserSauces))]
        public virtual SuccessfulSauce Sauce { get; set; }
        /// <summary>The user.</summary>
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(UserData.UserSauces))]
        public virtual UserData User { get; set; }
    }
}
