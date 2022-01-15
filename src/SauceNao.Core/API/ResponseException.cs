// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Net;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SauceNAO.Core.API
{
    public sealed class SearchResponseException : Exception
    {
        public SearchResponseException()
        {
        }

        public SearchResponseException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public SearchResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SearchResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        [JsonIgnore]
        public HttpStatusCode? StatusCode { get; set; }
    }
}
