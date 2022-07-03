// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.API;
using SauceNAO.Core.Enums;
using SauceNAO.Core.Extensions;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;

namespace SauceNAO.Core.Models
{
    public sealed class SauceBowl
    {
        public SauceBowl(SearchResponse response, float similarity = 55)
        {
            Similarity = similarity;
            if (response.Header.Status == 0 && response.Results != null)
            {
                IEnumerable<SearchResult> results = response.Results
                    .Where(r => float.Parse(r.Header.Similarity) > similarity);
                if (results.Any())
                {
                    Status = SauceStatus.Found;
                    Sauce = new Sauce();
                    Urls = new List<SauceUrl>();
                    Similarity = similarity;
                    foreach (var r in results)
                    {
                        var data = r.Data;
                        var resultSimilarity = float.Parse(r.Header.Similarity);
                        if (data.ExtUrls != null)
                        {
                            Urls.AddRange(r.Data.ExtUrls, resultSimilarity);
                        }
                        if (string.IsNullOrEmpty(Sauce.Title))
                        {
                            if (data.Title != null)
                            {
                                Sauce.Title = $"{data.Title.AsHtmlBoldText()}\n\n";
                            }
                            else if (data.Source != null)
                            {
                                if (!data.Source.StartsWith("http"))
                                {
                                    Sauce.Title = $"{data.Source.AsHtmlBoldText()}\n\n";
                                }
                            }
                        }
                        if (data.Source != null)
                        {
                            if (data.Source.StartsWith("http"))
                            {
                                Urls.Add(data.Source, resultSimilarity);
                            }
                        }
                        if (!string.IsNullOrEmpty(data.Characters) && string.IsNullOrEmpty(Sauce.Characters))
                        {
                            Sauce.Characters = $"<b>{{0}}:</b> {data.Characters.AsHtmlNormalText()}\n";
                        }
                        if (!string.IsNullOrEmpty(data.Material) && string.IsNullOrEmpty(Sauce.Material))
                        {
                            Sauce.Material = $"<b>{{0}}:</b> {data.Material.AsHtmlNormalText()}\n";
                        }
                        if (!string.IsNullOrEmpty(data.Part) && string.IsNullOrEmpty(Sauce.Part))
                        {
                            Sauce.Part = $"<b>{{0}}:</b> {data.Part.AsHtmlNormalText()}\n";
                        }
                        if (data.Creator != null && string.IsNullOrEmpty(Sauce.By))
                        {
                            if (data.Creator is string)
                            {
                                if (!string.IsNullOrEmpty(data.Creator as string))
                                {
                                    Sauce.By = $"<b>{{0}}:</b> {(data.Creator as string)!.AsHtmlNormalText()}\n";
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(data.Year) && string.IsNullOrEmpty(Sauce.Year))
                        {
                            Sauce.Year = $"<b>{{0}}:</b> {data.Year}\n";
                        }

                        if (!string.IsNullOrEmpty(data.EstTime) && string.IsNullOrEmpty(Sauce.EstTime))
                        {
                            Sauce.EstTime = $"<b>{{0}}:</b> {data.EstTime}\n";
                        }
                    }
                }
                else
                {
                    Status = SauceStatus.NotFound;
                    Message = string.Empty;
                }
            }
            else
            {
                Status = SauceStatus.Error;
                Message = response.Header.Message;
            }
        }
        public SauceBowl(SearchResponseException responseException)
        {
            Status = responseException.InnerException == default ? SauceStatus.BadRequest : SauceStatus.Error;
            Message = responseException.Message;
        }

        internal SauceStatus Status { get; }
        internal string? Message { get; }

        public Sauce? Sauce { get; }
        public ICollection<SauceUrl>? Urls { get; }
        public float? Similarity { get; }
    }
}
