// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SauceNAO.API.Models
{
    public sealed class Response
    {
        public Response() { }
        internal Response(Exception exception)
        {
            Exception = exception;
        }

        [JsonIgnore]
        public bool Okey { get; set; }
        [JsonIgnore]
        public Exception Exception { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        [JsonIgnore]
        public string HttpContent { get; set; }

        [JsonPropertyName("header")]
        public Header Header { get; set; }
        [JsonPropertyName("results")]
        public Result[] Results { get; set; }

        internal static Response ResponseError(HttpStatusCode statusCode) => new Response { StatusCode = statusCode };
        internal static async Task<Response> FromHttpResponse(HttpResponseMessage httpResponse)
        {
            Response response;
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                response = await httpResponse.Content
                        .ReadFromJsonAsync<Response>()
                        .ConfigureAwait(false);
                response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                response = ResponseError(httpResponse.StatusCode);
                response.HttpContent = await httpResponse.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false);
            }
            response.Okey = true;
            return response;
        }
    }
}
