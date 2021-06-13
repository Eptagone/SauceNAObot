// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace SauceNao.Data.Models
{
    /// <summary>Represents a Temporal File</summary>
    [Table("TemporalFile")]
    public partial class TemporalFile : IEquatable<TemporalFile>
    {
        /// <summary>Initialize a new instance of TemporalFile.</summary>
        public TemporalFile() { }
        internal TemporalFile(string fileUniqueId, string filePath, DateTime date)
        {
            FileUniqueId = fileUniqueId;
            FilePath = filePath;
            Date = date;
        }
        /// <summary>Temporal File Id.</summary>
        [Key]
        public int Id { get; set; }
        /// <summary>File Unique Id.</summary>
        [Required]
        [StringLength(64)]
        public string FileUniqueId { get; set; }
        /// <summary>File Path.</summary>
        [StringLength(256)]
        public string FilePath { get; set; }
        /// <summary>File date.</summary>
        public DateTime Date { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override bool Equals(object obj)
        {
            return Equals(obj as TemporalFile);
        }

        public bool Equals(TemporalFile other)
        {
            return other != null &&
                   Id == other.Id &&
                   FileUniqueId == other.FileUniqueId &&
                   FilePath == other.FilePath &&
                   Date == other.Date;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FileUniqueId, FilePath, Date);
        }

        public static bool operator ==(TemporalFile left, TemporalFile right)
        {
            return EqualityComparer<TemporalFile>.Default.Equals(left, right);
        }

        public static bool operator !=(TemporalFile left, TemporalFile right)
        {
            return !(left == right);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
