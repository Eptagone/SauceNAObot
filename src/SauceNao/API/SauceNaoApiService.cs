// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the MIT License, See LICENCE in the project root for license information.

using SauceNao.API.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SauceNao.API
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
        public SauceNaoApiService(OutputType outputType, string apikey, bool testMode, string dbmask, string dbmaski, uint db, uint numres, Dedupe dedupe)
        {
            var path = "search.php?output_type=" + outputType.ToString("d");
            if (!string.IsNullOrEmpty(apikey))
            {
                path += "&api_key=" + apikey;
            }
            if (testMode)
            {
                path += "&testmode=1";
            }
            if (!string.IsNullOrEmpty(dbmask))
            {
                path += "&dbmask=" + dbmask;
            }
            if (!string.IsNullOrEmpty(dbmaski))
            {
                path += "dbmaski=" + dbmaski;
            }
            if (db == default)
            {
                path += "&db=" + db;
            }
            if (numres == default)
            {
                path += "&numres=" + numres;
            }
            if (dedupe != Dedupe.AllImplementedDedupeMethodsSuchAsBySeriesName)
            {
                path += "&dedupe=" + dedupe.ToString("d");
            }
            path += "&url={0}";
            searchpath = path;
        }
        /// <summary>Start a new search using the url provided.</summary>
        /// <param name="url">Image url.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns><see cref="Response"/></returns>
        public async Task<Response> SearchAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullPath = string.Format(searchpath, url);
                var httpResponse = await httpClient.GetAsync(fullPath, cancellationToken);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return await httpResponse.Content.ReadFromJsonAsync<Response>(cancellationToken: cancellationToken);
                }
                else
                {
                    var message = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                    var error = new ResponseException(message)
                    {
                        StatusCode = httpResponse.StatusCode
                    };
                    throw error;
                }
            }
            catch (HttpRequestException exp)
            {
                throw new ResponseException(exp.Message, exp);
            }
            catch (Exception exp)
            {
                throw new ResponseException(exp.Message, exp);
            }
        }
        /// <summary>Start a new search using the url provided.</summary>
        /// <param name="url">Image url.</param>
        /// <returns><see cref="Response"/></returns>
        public Response Search(string url)
        {
            var search = SearchAsync(url);
            search.Wait();
            if (search.IsFaulted)
            {
                throw search.Exception.InnerException;
            }
            else
            {
                return search.Result;
            }
        }
    }
}
