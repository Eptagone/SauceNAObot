using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SauceNao.API.Models
{
    public sealed class ResponseException : Exception
    {
        public ResponseException()
        {
        }

        public ResponseException(string message) : base(message)
        {
        }

        public ResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        [JsonIgnore]
        public string HttpContent => Message;
    }
}
