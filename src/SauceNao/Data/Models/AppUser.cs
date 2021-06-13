// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Telegram.BotAPI.AvailableTypes;

#nullable disable

namespace SauceNao.Data.Models
{
    /// <summary>Application User Model</summary>
    [Table("AppUser")]
    public partial class AppUser : IEquatable<AppUser>
    {
        /// <summary>Initialize a new instance of AppUser</summary>
        public AppUser()
        {
            AntiCheats = new HashSet<AntiCheat>();
            UserSauces = new HashSet<UserSauce>();
        }

        internal AppUser(User user, [Optional] bool start) : this()
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Lang = user.LanguageCode;
            Start = start;
        }

        /// <summary>User Id</summary>
        [Key]
        public long Id { get; set; }
        /// <summary>Firstname</summary>
        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }
        /// <summary>Lastname</summary>
        [StringLength(64)]
        public string LastName { get; set; }
        /// <summary>Username</summary>
        [StringLength(32)]
        public string Username { get; set; }
        /// <summary>Languaje Code</summary>
        [StringLength(8)]
        public string Lang { get; set; }
        /// <summary>True, if the user prefers to use their own language in all chats.</summary>
        public bool LangForce { get; set; }
        /// <summary>True, if the user started a private chat with SauceNao bot.</summary>
        public bool Start { get; set; }

        /// <summary>AntiCheats added by user.</summary>
        [InverseProperty(nameof(AntiCheat.AddedByUser))]
        public virtual ICollection<AntiCheat> AntiCheats { get; set; }
        /// <summary>Sauces searched by user.</summary>
        [InverseProperty(nameof(UserSauce.User))]
        public virtual ICollection<UserSauce> UserSauces { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override bool Equals(object obj)
        {
            return Equals(obj as AppUser);
        }

        public bool Equals(AppUser other)
        {
            return other != null &&
                   Id == other.Id &&
                   FirstName == other.FirstName &&
                   LastName == other.LastName &&
                   Username == other.Username &&
                   Lang == other.Lang &&
                   LangForce == other.LangForce &&
                   Start == other.Start &&
                   EqualityComparer<ICollection<AntiCheat>>.Default.Equals(AntiCheats, other.AntiCheats) &&
                   EqualityComparer<ICollection<UserSauce>>.Default.Equals(UserSauces, other.UserSauces);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(FirstName);
            hash.Add(LastName);
            hash.Add(Username);
            hash.Add(Lang);
            hash.Add(LangForce);
            hash.Add(Start);
            hash.Add(AntiCheats);
            hash.Add(UserSauces);
            return hash.ToHashCode();
        }

        public void Update(User user, [Optional] bool start)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Start = Start ? Start : start;
        }

        public static bool operator ==(AppUser left, AppUser right)
        {
            return EqualityComparer<AppUser>.Default.Equals(left, right);
        }

        public static bool operator !=(AppUser left, AppUser right)
        {
            return !(left == right);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
