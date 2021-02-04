// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Models
{
    [Table("Users")]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class AppUser
    {
        public AppUser() { }
        public AppUser(User user)
        {
            Id = user.Id;
            Name = $"{user.FirstName}{(user.LastName == null ? string.Empty : $" {user.LastName}")}";
            Username = user.Username ?? string.Empty;
            Lang = user.LanguageCode ?? string.Empty;
        }
        internal AppUser(User user, bool start) : this(user)
        {
            Start = start;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Lang { get; set; }
        public bool Start { get; set; }

        public List<UserSauce> History { get; } = new List<UserSauce>();

        /// <summary>True, if tuser data is not equals to current user data.</summary>
        /// <param name="tuser">User</param>
        internal bool NotEquals(User tuser)
        {
            string _name = $"{tuser.FirstName}{(tuser.LastName == null ? string.Empty : $" {tuser.LastName}")}";
            string _username = tuser.Username ?? string.Empty;
            string _lang = tuser.LanguageCode ?? string.Empty;
            if (Name == _name && _username == Username && Lang == _lang)
            {
                return false;
            }
            else
            {
                Name = _name; Username = _username; Lang = _lang;
                return true;
            }
        }
    }
}
