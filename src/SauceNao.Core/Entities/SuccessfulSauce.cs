// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Extensions;
using SauceNAO.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json;
using IKM = Telegram.BotAPI.AvailableTypes.InlineKeyboardMarkup;

#nullable disable

namespace SauceNAO.Core.Entities;

/// <summary>Represents a Successful Sauce.</summary>
[Table("Sauce", Schema = "tg")]
public partial class SuccessfulSauce
{
	/// <summary>Initialize a new instance of SuccessfulSauce.</summary>
	public SuccessfulSauce()
	{
		this.UserSauces = new HashSet<UserSauce>();
	}

	internal SuccessfulSauce(SauceBowl sauce, TargetMedia targetMedia, DateTime dateTime)
	{
		this.Info = JsonSerializer.Serialize(sauce.Sauce);
		if (sauce.Urls != default)
		{
			this.Urls = JsonSerializer.Serialize(sauce.Urls);
		}
		this.Similarity = (float)sauce.Similarity;
		this.Date = dateTime;
		this.FileId = targetMedia.FileId;
		this.FileUniqueId = targetMedia.FileUniqueId;
		this.Type = targetMedia.Type.ToString("F");
	}
	/// <summary>Sauce Key</summary>
	[Key]
	public int Key { get; set; }
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

	internal string GetInfo(CultureInfo lang)
	{
		if (string.IsNullOrEmpty(this.Info))
		{
			return "unknown name";
		}
		else
		{
			var info = JsonSerializer.Deserialize<Sauce>(this.Info);
			return info.GetInfo(lang);
		}
	}
	internal IKM GetKeyboard()
	{
		if (string.IsNullOrEmpty(this.Urls))
		{
			return default;
		}
		else
		{
			var urlList = JsonSerializer.Deserialize<IEnumerable<SauceUrl>>(this.Urls);
			return urlList.ToInlineKeyboardMarkup();
		}
	}
}
