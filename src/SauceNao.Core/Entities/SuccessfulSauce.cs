// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Extensions;
using SauceNAO.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json;
using IKM = Telegram.BotAPI.AvailableTypes.InlineKeyboardMarkup;

namespace SauceNAO.Core.Entities;

/// <summary>
/// Represents a Successful Sauce.
/// </summary>
[Table("Sauce", Schema = "tg")]
public partial class SuccessfulSauce
{
	/// <summary>
	/// Initialize a new instance of SuccessfulSauce.
	/// </summary>
	public SuccessfulSauce()
	{
		this.UserSauces = new HashSet<UserSauce>();
		this.Urls = new HashSet<SauceUrl>();
	}

	internal SuccessfulSauce(SauceBowl sauce, TargetMedia targetMedia, DateTime dateTime) : this()
	{
		this.Info = JsonSerializer.Serialize(sauce.Sauce);
		if (sauce.Urls != null)
		{
			this.Urls = sauce.Urls;
		}
		this.Similarity = (float)sauce.Similarity!;
		this.Date = dateTime;
		this.FileId = targetMedia.FileId;
		this.FileUniqueId = targetMedia.FileUniqueId;
		this.Type = targetMedia.Type.ToString("F");
	}

	/// <summary>
	/// Sauce Key
	/// </summary>
	[Key]
	public int Key { get; set; }

	/// <summary>
	/// Unique File Id of this Sauce Media
	/// </summary>
	[Required]
	[StringLength(64)]
	public string FileUniqueId { get; set; } = null!;

	/// <summary>
	/// Sauce Type.
	/// </summary>
	[Required]
	[StringLength(10)]
	public string Type { get; set; } = null!;

	/// <summary>
	/// File Id of this Sauce Media
	/// </summary>
	[Required]
	public string FileId { get; set; } = null!;

	/// <summary>
	/// Sauce Info Data
	/// </summary>
	[Required]
	public string Info { get; set; } = null!;

	/// <summary>
	/// Sauce Similarity.
	/// </summary>
	public float Similarity { get; set; }

	/// <summary>
	/// Sauce date.
	/// </summary>
	public DateTime Date { get; set; }

	/// <summary>
	/// Sauce Urls
	/// </summary>
	[InverseProperty(nameof(SauceUrl.Sauce))]
	public ICollection<SauceUrl> Urls { get; set; }

	/// <summary>
	/// User sauces.
	/// </summary>
	[InverseProperty(nameof(UserSauce.Sauce))]
	public virtual ICollection<UserSauce> UserSauces { get; set; }

	/// <summary>
	/// Generate a the Sauce summary.
	/// </summary>
	/// <param name="lang"></param>
	/// <returns></returns>
	internal string GetInfo(CultureInfo lang)
	{
		if (string.IsNullOrEmpty(this.Info))
		{
			return "unknown name";
		}
		else
		{
			var info = JsonSerializer.Deserialize<Sauce>(this.Info);
			return info == null ? "unknown name" : info.GetInfo(lang);
		}
	}

	/// <summary>
	/// Try to get the keyboard.
	/// </summary>
	internal IKM? GetKeyboard()
	{
		return this.Urls.Any() ? this.Urls.ToInlineKeyboardMarkup() : null;
	}
}
