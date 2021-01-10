// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using SauceNAO.API.Models;
using SauceNAO.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

using SDIR = SauceNAO.Resources.SauceDirectory;

namespace SauceNAO.API
{
    public class SauceNAOService
    {
        public SauceNAOService() { }
        public SauceNAOService(string apikey)
        {
            SearchPath = string.IsNullOrEmpty(apikey) ?
                "search.php?output_type=2&db=999&url={0}" :
                $"search.php?output_type=2&api_key={apikey}&db=999&url={{0}}";
        }

        /// <summary>Http Client for SauceNAO API requests</summary>
        private static readonly HttpClient client =
            new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = 128 }) { BaseAddress = new Uri(SDIR.SauceNAO) };

        /// <summary>Search Path</summary>
        private readonly string SearchPath;

        public async Task<Response> GetResponseAsync(string mediaurl)
        {
            try
            {
                var path = string.Format(SearchPath, mediaurl);
                HttpResponseMessage httpResponse = await client.GetAsync(path).ConfigureAwait(false);
                return await Response
                    .FromHttpResponse(httpResponse)
                    .ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                return new Response(exp);
            }
        }

        /// <summary>Get the final result search</summary>
        internal async Task<Sauce> SearchAsync(string mediaurl)
        {
            Response response = await GetResponseAsync(mediaurl).ConfigureAwait(false);
            return new Sauce(response);
        }
    }
}
