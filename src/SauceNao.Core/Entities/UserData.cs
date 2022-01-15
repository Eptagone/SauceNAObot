// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Entities
{
    /// <summary>Application User Model</summary>
    [Table("User")]
    public partial class UserData : ITelegramUser
    {
        /// <summary>Initialize a new instance of AppUser</summary>
        public UserData()
        {
            AntiCheats = new HashSet<AntiCheat>();
            UserSauces = new HashSet<UserSauce>();
        }

        public UserData(ITelegramUser user) : this()
        {
            if (user == default)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            LanguageCode = user.LanguageCode;
        }

        /// <summary>User Id</summary>
        [Key]
        public long Id { get; set; }
        /// <summary>Firstname</summary>
        [Required]
        [StringLength(64)]
        public string FirstName { get; set; } = null!;
        /// <summary>Lastname</summary>
        [StringLength(64)]
        public string? LastName { get; set; }
        /// <summary>Username</summary>
        [StringLength(32)]
        public string? Username { get; set; }
        /// <summary>Languaje Code</summary>
        [StringLength(8)]
        public string? LanguageCode { get; set; }
        /// <summary>True, if the user prefers to use their own language in all chats.</summary>
        public bool LangForce { get; set; }
        /// <summary>True, if the user started a private chat with SauceNao bot.</summary>
        public bool Start { get; set; }
        /// <summary>Private user's apikey</summary>
        public string? ApiKey { get; set; }

        [NotMapped]
        bool ITelegramUser.IsBot { get => false; set { } }

        /// <summary>AntiCheats added by user.</summary>
        [InverseProperty(nameof(AntiCheat.AddedByUser))]
        public virtual ICollection<AntiCheat> AntiCheats { get; set; }
        /// <summary>Sauces searched by user.</summary>
        [InverseProperty(nameof(UserSauce.User))]
        public virtual ICollection<UserSauce> UserSauces { get; set; }

        public bool Update(ITelegramUser user)
        {
            var hasChanges = false;
            if (user == default)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (FirstName != user.FirstName || LastName != user.LastName || Username != user.Username)
            {
                FirstName = user.FirstName;
                LastName = user.LastName;
                Username = user.Username;
                hasChanges = true;
            }
            if (LanguageCode == default && user.LanguageCode != LanguageCode)
            {
                LanguageCode = user.LanguageCode;
                hasChanges = true;
            }
            return hasChanges;
        }
    }
}
