// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.API;
using SauceNAO.Core.Enums;
using SauceNAO.Core.Extensions;

namespace SauceNAO.Core.Models;

public sealed class SauceBowl
{
	public SauceBowl(SearchResponse response, float similarity = 55)
	{
		this.Similarity = similarity;
		if (response.Header.Status == 0 && response.Results != null)
		{
			IEnumerable<SearchResult> results = response.Results
				.Where(r => float.Parse(r.Header.Similarity) > similarity);
			if (results.Any())
			{
				this.Status = SauceStatus.Found;
				this.Sauce = new Sauce();
				this.Urls = new List<SauceUrl>();
				this.Similarity = similarity;
				foreach (var r in results)
				{
					var data = r.Data;
					var resultSimilarity = float.Parse(r.Header.Similarity);
					if (data.ExtUrls != null)
					{
						this.Urls.AddRange(r.Data.ExtUrls, resultSimilarity);
					}
					if (string.IsNullOrEmpty(this.Sauce.Title))
					{
						if (data.Title != null)
						{
							this.Sauce.Title = $"{data.Title.AsHtmlBoldText()}\n\n";
						}
						else if (data.Source != null)
						{
							if (!data.Source.StartsWith("http"))
							{
								this.Sauce.Title = $"{data.Source.AsHtmlBoldText()}\n\n";
							}
						}
					}
					if (data.Source != null)
					{
						if (data.Source.StartsWith("http"))
						{
							this.Urls.Add(data.Source, resultSimilarity);
						}
					}
					if (!string.IsNullOrEmpty(data.Characters) && string.IsNullOrEmpty(this.Sauce.Characters))
					{
						this.Sauce.Characters = $"<b>{{0}}:</b> {data.Characters.AsHtmlNormalText()}\n";
					}
					if (!string.IsNullOrEmpty(data.Material) && string.IsNullOrEmpty(this.Sauce.Material))
					{
						this.Sauce.Material = $"<b>{{0}}:</b> {data.Material.AsHtmlNormalText()}\n";
					}
					if (!string.IsNullOrEmpty(data.Part) && string.IsNullOrEmpty(this.Sauce.Part))
					{
						this.Sauce.Part = $"<b>{{0}}:</b> {data.Part.AsHtmlNormalText()}\n";
					}
					if (data.Creator != null && string.IsNullOrEmpty(this.Sauce.By))
					{
						if (data.Creator is string)
						{
							if (!string.IsNullOrEmpty(data.Creator as string))
							{
								this.Sauce.By = $"<b>{{0}}:</b> {(data.Creator as string)!.AsHtmlNormalText()}\n";
							}
						}
					}
					if (!string.IsNullOrEmpty(data.Year) && string.IsNullOrEmpty(this.Sauce.Year))
					{
						this.Sauce.Year = $"<b>{{0}}:</b> {data.Year}\n";
					}

					if (!string.IsNullOrEmpty(data.EstTime) && string.IsNullOrEmpty(this.Sauce.EstTime))
					{
						this.Sauce.EstTime = $"<b>{{0}}:</b> {data.EstTime}\n";
					}
				}
			}
			else
			{
				this.Status = SauceStatus.NotFound;
				this.Message = string.Empty;
			}
		}
		else
		{
			this.Status = SauceStatus.Error;
			this.Message = response.Header.Message;
		}
	}
	public SauceBowl(SearchResponseException responseException)
	{
		this.Status = responseException.InnerException == default ? SauceStatus.BadRequest : SauceStatus.Error;
		this.Message = responseException.Message;
	}

	internal SauceStatus Status { get; }
	internal string? Message { get; }

	public Sauce? Sauce { get; }
	public ICollection<SauceUrl>? Urls { get; }
	public float? Similarity { get; }
}
