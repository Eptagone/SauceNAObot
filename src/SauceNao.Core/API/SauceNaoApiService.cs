// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Runtime.InteropServices;

namespace SauceNAO.Core.API
{
    /// <summary>This class provides access to the saucenao.com api.</summary>
    public sealed class SauceNaoApiService
    {
        /// <summary>Saucenao Root URL.</summary>
        public const string SauceNao = "https://saucenao.com/";

        /// <summary>Http Client for SauceNAO API requests</summary>
        private static readonly HttpClient httpClient = new() { BaseAddress = new Uri(SauceNao) };

        private readonly string searchpath;

        /// <summary>Initializes a new instance of SauceNaoApiService class.</summary>
        public SauceNaoApiService()
        {
            searchpath = "search.php?url={0}";
        }

        /// <summary>Initializes a new instance of SauceNaoApiService class.</summary>
        /// <param name="outputType">0=normal html 1=xml api(not implemented) 2=json api</param>
        /// <param name="apikey">Allows using the API from anywhere regardless of whether the client is logged in, or supports cookies.</param>
        /// <param name="testMode">Causes each index which has a match to output at most 1 for testing. Works best with a numres greater than the number of indexes searched.</param>
        /// <param name="dbmask">Mask for selecting specific indexes to ENABLE. dbmask=8191 will search all of the first 14 indexes. If intending to search all databases, the db=999 option is more appropriate.</param>
        /// <param name="dbmaski">Mask for selecting specific indexes to DISABLE. dbmaski=8191 would search only indexes higher than the first 14. This is ideal when attempting to disable only certain indexes, while allowing future indexes to be included by default.<para>Bitmask Note: Index numbers start with 0. Even though pixiv is labeled as index 5, it would be controlled with the 6th bit position, which has a decimal value of 32 when set.</para></param>
        /// <param name="db">search a specific index number or all without needing to generate a bitmask.</param>
        /// <param name="numres">Change the number of results requested.</param>
        /// <param name="dedupe">0=no result deduping 1=consolidate booru results and dedupe by item identifier 2=all implemented dedupe methods such as by series name. Default is 2, more levels may be added in future.</param>
        /// <param name="hide">This controls the hidden field of results based on result content info. All results still show up in the api, check the hidden field for whether the site would have shown the image file.</param>
        public SauceNaoApiService([Optional] OutputType outputType, [Optional] string? apikey, [Optional] bool? testMode, [Optional] string? dbmask, [Optional] string? dbmaski, [Optional] uint? db, [Optional] uint? numres, [Optional] Dedupe dedupe, [Optional] Hide hide)
        {
            var path = "search.php?output_type=" + outputType.ToString("d");
            if (!string.IsNullOrEmpty(apikey))
            {
                path += "&api_key=" + apikey;
            }
            if (testMode != null)
            {
                path += "&testmode=" + testMode;
            }
            if (!string.IsNullOrEmpty(dbmask))
            {
                path += "&dbmask=" + dbmask;
            }
            if (!string.IsNullOrEmpty(dbmaski))
            {
                path += "dbmaski=" + dbmaski;
            }
            if (db != null)
            {
                path += "&db=" + db;
            }
            if (numres != null)
            {
                path += "&numres=" + numres;
            }
            if (dedupe != default)
            {
                path += "&dedupe=" + dedupe.ToString("d");
            }
            if (hide != default)
            {
                path += "&hide=" + hide.ToString("d");
            }
            path += "&url={0}";
            searchpath = path;
        }

        /// <summary>Start a new search using the url provided.</summary>
        /// <param name="url">Image url.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="SearchResponse"/></returns>
        /// <exception cref="SearchResponseException"/>
        public async Task<SearchResponse> SearchAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullPath = string.Format(searchpath, url);
                var httpResponse = await httpClient.GetAsync(fullPath, cancellationToken);

                var searchResponse = await httpResponse.Content.ReadFromJsonAsync<SearchResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (searchResponse != null)
                {
                    return searchResponse;
                }
                else
                {
                    var message = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                    throw new SearchResponseException(message, httpResponse.StatusCode);
                }
            }
            catch (HttpRequestException exp)
            {
                throw new SearchResponseException(exp.Message, exp);
            }
            catch (Exception exp)
            {
                throw new SearchResponseException(exp.Message, exp);
            }
        }


        /// <summary>Start a new search using the url provided.</summary>
        /// <param name="url">Image url.</param>
        /// <returns><see cref="SearchResponse"/></returns>
        [SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "<Pending>")]
        public SearchResponse Search(string url)
        {
            var search = SearchAsync(url);
            search.Wait();
            if (search.IsFaulted)
            {
                throw search.Exception?.InnerException
                    ?? search.Exception
                    ?? throw new Exception("SauceNao API: Unknown exception");
            }
            else
            {
                return search.Result;
            }
        }
    }
}
