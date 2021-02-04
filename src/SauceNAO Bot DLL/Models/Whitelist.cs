// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Models
{
    public sealed class Whitelist
    {
        public Whitelist() { }
        internal Whitelist(User user)
        {
            Id = user.Id;
            Name = $"{user.FirstName}{(user.LastName == null ? string.Empty : $" {user.LastName}")}";
            Username = user.Username ?? string.Empty;
        }

        [Key]
        public int Key { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }

        [ForeignKey("Chat")]
        public int ChatKey { get; set; }
        public AppChat Chat { get; set; }
    }
}
