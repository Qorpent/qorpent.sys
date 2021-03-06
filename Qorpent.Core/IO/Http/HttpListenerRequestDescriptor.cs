﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

namespace Qorpent.IO.Http {
    internal class HttpListenerRequestDescriptor : HttpRequestDescriptor {
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
            if (null != request.Headers["X-Real-IP"]) {           
                var addr = request.RemoteEndPoint.Address.ToString();
                if (addr.StartsWith("127.0.") || addr.StartsWith("192.168.")) {
                    RemoteEndPoint = new IPEndPoint(IPAddress.Parse(request.Headers["X-Real-IP"]),0);
                }
                else {
                    throw new Exception("Invalid X-Real-IP origin");
                }
            }
            else {
                RemoteEndPoint = request.RemoteEndPoint;
            }
            LocalEndPoint = request.LocalEndPoint;

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