using System;
using System.Net;

namespace Lab2.Shared.Exceptions {
    [Serializable]
    public class RequestException : Exception {
        public RequestException(string message, HttpStatusCode statusCode) : base(message) {
            StatusCode = statusCode;
        }

        public RequestException(string message, HttpStatusCode statusCode, Exception inner) : base(message, inner) {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}
