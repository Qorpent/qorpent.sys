using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Http {
    public class HttpRequestDescriptor {
        private Encoding _encoding;
        private string _method;
        public const string IfModifiedSinceHeader = "If-Modified-Since";
        public const string IfNoneMatchHeader = "If-None-Match";
        public virtual IDictionary<string, string> Headers { get; set; }

        public static implicit operator HttpRequestDescriptor(HttpListenerContext context) {
            return new HttpListenerRequestDescriptor(context.Request){User = context.User};
        }

        public static implicit operator HttpRequestDescriptor(HttpListenerRequest request) {
            return new HttpListenerRequestDescriptor(request);
        }

        public static implicit operator HttpRequestDescriptor(string url) {
            return new HttpRequestDescriptor {Uri = new Uri(url)};
        }

        public virtual IPrincipal User { get; set; }

        public virtual string GetHeader(string name) {
            if (null == Headers) {
                return string.Empty;
            }
            if (Headers.ContainsKey(name)) {
                return string.Empty;
            }
            foreach (var key in Headers.Keys) {
                if (key.ToLowerInvariant() == name.ToLowerInvariant()) {
                    return Headers[key] ?? string.Empty;
                }
            }
            return string.Empty;
        }

        public DateTime GetIfModifiedSince() {
            var header = GetHeader(IfModifiedSinceHeader);
            if (header.IsNotEmpty()) {
                var result =
                    DateTime.ParseExact(header, "R", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal)
                        .ToLocalTime();
                return new DateTime(result.Year, result.Month, result.Day,
                    result.Hour, result.Minute, result.Second);
            }
            return new DateTime(1900, 1, 1);
        }

        public string GetIfNoneMatch() {
            return GetHeader(IfNoneMatchHeader);
        }

        public string ContentType { get; set; }

        public Encoding Encoding {
            get { return _encoding ?? Encoding.UTF8; }
            set { _encoding = value; }
        }

        public long ContentLength { get; set; }
        public Uri Uri { get; set; }
        public Stream Stream { get; set; }

        public string Method {
            get { return _method ?? "GET"; }
            set { _method = value; }
        }

        public virtual string[] UserLanguages { get; set; }
        public virtual string UserHostAddress { get; set; }
        public virtual string UserHostName { get; set; }
        public virtual string UserAgent { get; set; }
        public CookieCollection Cookies { get; set; }

        public class HttpListenerRequestDescriptor : HttpRequestDescriptor {
            private IDictionary<string, string> _headers;
            private readonly HttpListenerRequest _request;

            public HttpListenerRequestDescriptor(HttpListenerRequest request) {
                _request = request;
                ContentType = request.ContentType;
                Encoding = request.ContentEncoding;
                ContentLength = request.ContentLength64;
                Uri = request.Url;
                Stream = request.InputStream;
                Method = request.HttpMethod;
                Cookies = request.Cookies;

            }

            public override IDictionary<string, string> Headers {
                get { return _headers ?? (_headers = ConvertToDict(_request.Headers)); }
            }

            private IDictionary<string, string> ConvertToDict(NameValueCollection headers) {
                var result = new Dictionary<string, string>();
                foreach (var key in headers.AllKeys) {
                    result[key] = headers[key];
                }
                return result;
            }

            public override string[] UserLanguages {
                get { return _request.UserLanguages; }
                set {  }
            }

            public override string UserHostAddress {
                get { return _request.UserHostAddress; }
                set { }
            }

            public override string UserHostName {
                get { return _request.UserHostName; }
                set {  }
            }

            public override string UserAgent
            {
                get { return _request.UserAgent; }
                set { }
            }
        }
    }
}