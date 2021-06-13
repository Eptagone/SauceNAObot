// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNao.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json;
using IKM = Telegram.BotAPI.AvailableTypes.InlineKeyboardMarkup;

#nullable disable

namespace SauceNao.Data.Models
{
    /// <summary>Represents a Successful Sauce.</summary>
    [Table("SuccessfulSauce")]
    [Index(nameof(FileUniqueId), Name = "UQ_FileUniqueId", IsUnique = true)]
    public partial class SuccessfulSauce : IEquatable<SuccessfulSauce>
    {
        /// <summary>Initialize a new instance of SuccessfulSauce.</summary>
        public SuccessfulSauce()
        {
            UserSauces = new HashSet<UserSauce>();
        }

        internal SuccessfulSauce(Sauce sauce, TargetMedia targetMedia, DateTime dateTime)
        {
            Info = JsonSerializer.Serialize(sauce.Info);
            if (sauce.Urls != default)
            {
                Urls = JsonSerializer.Serialize(sauce.Urls);
            }
            Similarity = sauce.Similarity;
            Date = dateTime;
            FileId = targetMedia.FileId;
            FileUniqueId = targetMedia.FileUniqueId;
            Type = targetMedia.Type.ToString("F");
        }
        /// <summary>Sauce Id</summary>
        [Key]
        public int Id { get; set; }
        /// <summary>Unique File Id of this Sauce Media</summary>
        [Required]
        [StringLength(64)]
        public string FileUniqueId { get; set; }
        /// <summary>Sauce Type.</summary>
        [Required]
        [StringLength(10)]
        public string Type { get; set; }
        /// <summary>File Id of this Sauce Media</summary>
        [Required]
        public string FileId { get; set; }
        /// <summary>Sauce Info Data</summary>
        [Required]
        public string Info { get; set; }
        /// <summary>Sauce Urls</summary>
        [Required]
        public string Urls { get; set; }
        /// <summary>Sauce Similarity.</summary>
        public float Similarity { get; set; }
        /// <summary>Sauce date.</summary>
        public DateTime Date { get; set; }

        /// <summary>User sauces.</summary>
        [InverseProperty(nameof(UserSauce.Sauce))]
        public virtual ICollection<UserSauce> UserSauces { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override bool Equals(object obj)
        {
            return Equals(obj as SuccessfulSauce);
        }

        public bool Equals(SuccessfulSauce other)
        {
            return other != null &&
                   Id == other.Id &&
                   FileUniqueId == other.FileUniqueId &&
                   Type == other.Type &&
                   FileId == other.FileId &&
                   Info == other.Info &&
                   Urls == other.Urls &&
                   Similarity == other.Similarity &&
                   Date == other.Date &&
                   EqualityComparer<ICollection<UserSauce>>.Default.Equals(UserSauces, other.UserSauces);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(FileUniqueId);
            hash.Add(Type);
            hash.Add(FileId);
            hash.Add(Info);
            hash.Add(Urls);
            hash.Add(Similarity);
            hash.Add(Date);
            hash.Add(UserSauces);
            return hash.ToHashCode();
        }

        internal string GetInfo(CultureInfo lang)
        {
            if (string.IsNullOrEmpty(Info))
            {
                return "unknown name";
            }
            else
            {
                var info = JsonSerializer.Deserialize<SauceInfo>(Info);
                return info.GetInfo(lang);
            }
        }
        internal IKM GetKeyboard()
        {
            if (string.IsNullOrEmpty(Urls))
            {
                return default;
            }
            else
            {
                var urlList = JsonSerializer.Deserialize<SauceUrlList>(Urls);
                return urlList.ToInlineKeyboardMarkup();
            }
        }

        public static bool operator ==(SuccessfulSauce left, SuccessfulSauce right)
        {
            return EqualityComparer<SuccessfulSauce>.Default.Equals(left, right);
        }

        public static bool operator !=(SuccessfulSauce left, SuccessfulSauce right)
        {
            return !(left == right);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
