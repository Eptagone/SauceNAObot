// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SauceNAO.Models
{
    public class UserSauce
    {
        public UserSauce() { }
        internal UserSauce(Sauce sauce, uint date)
        {
            Date = date;
            SauceFile = sauce.File;
        }

        [Key]
        public int Key { get; set; }
        public uint Date { get; set; }
        public string SauceFile { get; set; }

        [ForeignKey("User")]
        public int Id { get; set; }
        public AppUser User { get; set; }
    }
}
