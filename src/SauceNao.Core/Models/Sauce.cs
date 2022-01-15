// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Resources;
using System.Globalization;

namespace SauceNAO.Core.Models
{
    public sealed class Sauce : ISauce
    {
        public string? Title { get; set; }
        public string? Characters { get; set; }
        public string? Material { get; set; }
        public string? Part { get; set; }
        public string? Year { get; set; }
        public string? EstTime { get; set; }
        public string? By { get; set; }

        internal string GetInfo(CultureInfo lang)
        {
            string info = string.Empty;
            if (!string.IsNullOrEmpty(Title))
            {
                info += Title;
            }
            if (!string.IsNullOrEmpty(Characters))
            {
                var characters = Characters;
                info += string.Format(characters, characters.Contains(',') ? MSG.ResultCharacters(lang) : MSG.ResultCharacter(lang));
            }
            if (!string.IsNullOrEmpty(Material))
            {
                info += string.Format(Material, MSG.ResultMaterial(lang));
            }
            if (!string.IsNullOrEmpty(Part))
            {
                info += string.Format(Part, MSG.ResultPart(lang));
            }
            if (!string.IsNullOrEmpty(By))
            {
                info += string.Format(By, MSG.ResultCreator(lang));
            }
            if (!string.IsNullOrEmpty(Year))
            {
                info += string.Format(Year, MSG.ResultYear(lang));
            }
            if (!string.IsNullOrEmpty(EstTime))
            {
                info += string.Format(EstTime, MSG.ResultTime(lang));
            }
            return string.IsNullOrEmpty(info) ? "unknown name" : info;
        }
    }
}
