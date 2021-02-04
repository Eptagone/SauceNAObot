// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

namespace SauceNAO.Models
{
    public sealed class TargetFile
    {
        /// <summary>True, if is a valid File.</summary>
        public bool Ok { get; set; }
        /// <summary>True, if contains media.</summary>
        public bool HasMedia { get; set; }
        /// <summary>Original file Id.</summary>
        public string OriginalFileId { get; set; }
        /// <summary>Unique file Id.</summary>
        public string FileId { get; set; }
        /// <summary>FileUniqueId.</summary>
        public string UniqueId { get; set; }
        /// <summary>Type.</summary>
        public string Type { get; set; }
        ///<summary>Is thumb.</summary>
        public bool IsThumb { get; set; }
    }
}
