// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace SauceNao.Data.Models
{
    /// <summary>Represents a sauce that was searched by a user. A user history item.</summary>
    [Table("UserSauce")]
    public partial class UserSauce : IEquatable<UserSauce>
    {
        /// <summary>Initialize a nes instance of UserSauce.</summary>
        public UserSauce() { }

        internal UserSauce(SuccessfulSauce sauce, DateTime dateTime)
        {
            Date = dateTime;
            Sauce = sauce;
        }

        /// <summary>User sauce Id.</summary>
        [Key]
        public int Id { get; set; }
        /// <summary>Sauce Id.</summary>
        public int SauceId { get; set; }
        /// <summary>User Id.</summary>
        public long UserId { get; set; }
        /// <summary>Sauce Date.</summary>
        public DateTime Date { get; set; }

        /// <summary>The Sauce.</summary>
        [ForeignKey(nameof(SauceId))]
        [InverseProperty(nameof(SuccessfulSauce.UserSauces))]
        public virtual SuccessfulSauce Sauce { get; set; }
        /// <summary>The user.</summary>
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(AppUser.UserSauces))]
        public virtual AppUser User { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override bool Equals(object obj)
        {
            return Equals(obj as UserSauce);
        }

        public bool Equals(UserSauce other)
        {
            return other != null &&
                   Id == other.Id &&
                   SauceId == other.SauceId &&
                   UserId == other.UserId &&
                   Date == other.Date &&
                   EqualityComparer<SuccessfulSauce>.Default.Equals(Sauce, other.Sauce) &&
                   EqualityComparer<AppUser>.Default.Equals(User, other.User);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, SauceId, UserId, Date, Sauce, User);
        }

        public static bool operator ==(UserSauce left, UserSauce right)
        {
            return EqualityComparer<UserSauce>.Default.Equals(left, right);
        }

        public static bool operator !=(UserSauce left, UserSauce right)
        {
            return !(left == right);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
