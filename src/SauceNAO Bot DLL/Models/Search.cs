// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using SauceNAO.API.Models;
using SauceNAO.Enumerators;
using SauceNAO.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

using SDIR = SauceNAO.Resources.SauceDirectory;

namespace SauceNAO.Models
{
    public class Sauce
    {
        public Sauce() { }
        internal Sauce(Response response)
        {
            Response = response;
            if (response.Okey == false)
            {
                Status = StatusResult.Error;
            }
            else if (response.StatusCode != HttpStatusCode.OK)
            {
                Status = StatusResult.BadRequest;
                Message = response.HttpContent;
            }
            else if (response.Results == null)
            {
                Status = StatusResult.SauceError;
                Message = response.Header.Message;
            }
            else
            {
                IEnumerable<Result> results = response
                    .Results
                    .Where(r => double.Parse(r.Header.Similarity) > 55);
                if (!results.Any())
                {
                    Status = StatusResult.NotFound;
                    Message = string.Empty;
                }
                else
                {
                    Status = StatusResult.Found;
                    IEnumerable<DataResult> sourcesdata = from r in results select r.Data;
                    var Urls = new List<string>();
                    foreach (DataResult data in sourcesdata)
                    {
                        if (data.ExtUrls != null)
                        {
                            Urls.AddRange(data.ExtUrls);
                        }
                        if (string.IsNullOrEmpty(Title))
                        {
                            if (data.Title != null)
                            {
                                Title = $"<b>{data.Title.ParseHTMLTags()}</b>\n\n";
                            }
                            else if (data.Source != null)
                            {
                                if (!data.Source.StartsWith("http"))
                                {
                                    Title = $"<b>{data.Source.ParseHTMLTags()}</b>\n\n";
                                }
                            }
                        }
                        if (data.Source != null)
                        {
                            if (data.Source.StartsWith("http"))
                            {
                                Urls.Add(data.Source);
                            }
                        }
                        if (!string.IsNullOrEmpty(data.Characters) && string.IsNullOrEmpty(Characters))
                        {
                            Characters = $"<b>{{0}}:</b> {data.Characters.ParseHTMLTags()}\n";
                        }
                        if (!string.IsNullOrEmpty(data.Material) && string.IsNullOrEmpty(Material))
                        {
                            Material = $"<b>{{0}}:</b> {data.Material.ParseHTMLTags()}\n";
                        }
                        if (!string.IsNullOrEmpty(data.Part) && string.IsNullOrEmpty(Part))
                        {
                            Part = $"<b>{{0}}:</b> {data.Part.ParseHTMLTags()}\n";
                        }
                        if (data.Creator != null && string.IsNullOrEmpty(By))
                        {
                            if (data.Creator.GetType() == typeof(string))
                            {
                                if (!string.IsNullOrEmpty((string)data.Creator))
                                {
                                    By = $"<b>{{0}}:</b> {((string)data.Creator).ParseHTMLTags()}\n";
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(data.Year) && string.IsNullOrEmpty(Year))
                        {
                            Year = $"<b>{{0}}:</b> {data.Year}\n";
                        }

                        if (!string.IsNullOrEmpty(data.EstTime) && string.IsNullOrEmpty(EstTime))
                        {
                            EstTime = $"<b>{{0}}:</b> {data.EstTime}\n";
                        }
                    }
                    // Fix urls
                    for (int i = 0; i < Urls.Count; i++)
                    {
                        if (Urls[i].Contains("i.pximg.net"))
                        {
                            string pid = Path.GetFileNameWithoutExtension(Urls[i]);
                            Urls[i] = string.Format(SDIR.PixivId, pid.Split('_')[0]);
                        }
                    }
                    Data = Urls.Distinct().ToArray();
                }
            }
        }

        internal void Finish(TargetFile file, string tmpurl, uint date)
        {
            Okey = false;
            File = file.UniqueId;
            OriginalFile = file.OriginalFileId;
            Type = file.Type;
            TmpUrl = tmpurl;
            SearchDate = date;
        }
        internal void Finish(TargetFile file, uint date)
        {
            Okey = true;
            File = file.UniqueId;
            OriginalFile = file.OriginalFileId;
            Type = file.Type;
            SearchDate = date;
        }

        public int Id { get; set; }
        public bool Okey { get; set; }
        public string File { get; set; }
        public string Type { get; set; }
        public string OriginalFile { get; set; }
        public string[] Data { get; set; }
        public string TmpUrl { get; set; }
        public uint SearchDate { get; set; }

        public string Title { get; set; }
        public string Characters { get; set; }
        public string Material { get; set; }
        public string Part { get; set; }
        public string Year { get; set; }
        public string EstTime { get; set; }
        public string By { get; set; }

        [NotMapped]
        internal Response Response { get; set; }
        [NotMapped]
        internal StatusResult Status { get; set; }
        [NotMapped]
        internal string Message { get; set; }

        public string ReadInfo([Optional] CultureInfo lang)
        {
            if (lang == default)
            {
                lang = new CultureInfo("en");
            }

            string info = string.Empty;
            if (!string.IsNullOrEmpty(Title))
            {
                info += Title;
            }
            if (!string.IsNullOrEmpty(Characters))
            {
                info += string.Format(Characters, Characters.Contains(',') ? MSG.ResultCharacters(lang) : MSG.ResultCharacter(lang));
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
            return info;
        }
    }
}
