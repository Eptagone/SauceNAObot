// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNao.API.Models;
using SauceNao.Enums;
using System.Collections.Generic;
using System.Linq;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;

namespace SauceNao.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class Sauce
    {
        public Sauce() { }
        public Sauce(Response response, float similarity = 55)
        {
            if (response.Results == null)
            {
                Status = SauceStatus.SauceError;
                Message = response.Header.Message;
            }
            else
            {
                IEnumerable<Result> results = response.Results
                    .Where(r => float.Parse(r.Header.Similarity) > similarity);
                if (!results.Any())
                {
                    Status = SauceStatus.NotFound;
                    Message = string.Empty;
                }
                else
                {
                    Status = SauceStatus.Found;
                    Info = new SauceInfo();
                    Urls = new SauceUrlList();
                    Similarity = similarity;
                    foreach (var r in results)
                    {
                        var data = r.Data;
                        var resultSimilarity = float.Parse(r.Header.Similarity);
                        if (data.ExtUrls != null)
                        {
                            Urls.AddRange(r.Data.ExtUrls, resultSimilarity);
                        }
                        if (string.IsNullOrEmpty(Info.Title))
                        {
                            if (data.Title != null)
                            {
                                Info.Title = $"<b>{StyleFixer.Default.FixToHTML(data.Title)}</b>\n\n";
                            }
                            else if (data.Source != null)
                            {
                                if (!data.Source.StartsWith("http"))
                                {
                                    Info.Title = $"<b>{StyleFixer.Default.FixToHTML(data.Source)}</b>\n\n";
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
                        if (!string.IsNullOrEmpty(data.Characters) && string.IsNullOrEmpty(Info.Characters))
                        {
                            Info.Characters = $"<b>{{0}}:</b> {StyleFixer.Default.FixToHTML(data.Characters)}\n";
                        }
                        if (!string.IsNullOrEmpty(data.Material) && string.IsNullOrEmpty(Info.Material))
                        {
                            Info.Material = $"<b>{{0}}:</b> {StyleFixer.Default.FixToHTML(data.Material)}\n";
                        }
                        if (!string.IsNullOrEmpty(data.Part) && string.IsNullOrEmpty(Info.Part))
                        {
                            Info.Part = $"<b>{{0}}:</b> {StyleFixer.Default.FixToHTML(data.Part)}\n";
                        }
                        if (data.Creator != null && string.IsNullOrEmpty(Info.By))
                        {
                            if (data.Creator is string)
                            {
                                if (!string.IsNullOrEmpty(data.Creator as string))
                                {
                                    Info.By = $"<b>{{0}}:</b> {StyleFixer.Default.FixToHTML(data.Creator as string)}\n";
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(data.Year) && string.IsNullOrEmpty(Info.Year))
                        {
                            Info.Year = $"<b>{{0}}:</b> {data.Year}\n";
                        }

                        if (!string.IsNullOrEmpty(data.EstTime) && string.IsNullOrEmpty(Info.EstTime))
                        {
                            Info.EstTime = $"<b>{{0}}:</b> {data.EstTime}\n";
                        }

                    }
                }
            }
        }
        public Sauce(ResponseException responseException)
        {
            Status = responseException.InnerException == default ? SauceStatus.BadRequest : SauceStatus.Error;
            Message = responseException.Message;
        }
        internal SauceStatus Status { get; private set; }
        internal string Message { get; private set; }
        public SauceInfo Info { get; set; }
        public SauceUrlList Urls { get; set; }
        public float Similarity { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
