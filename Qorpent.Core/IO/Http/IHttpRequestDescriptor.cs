using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;

namespace Qorpent.IO.Http {
    public interface IHttpRequestDescriptor {
        IDictionary<string, string> Headers { get; set; }
        IPrincipal User { get; set; }
        string ContentType { get; set; }
        Encoding Encoding { get; set; }
        long ContentLength { get; set; }
        Uri Uri { get; set; }
        Stream Stream { get; set; }
        string Method { get; set; }
        string[] UserLanguages { get; set; }
        string UserHostAddress { get; set; }
        string UserHostName { get; set; }
        string UserAgent { get; set; }
        CookieCollection Cookies { get; set; }
        IPEndPoint LocalEndPoint { get; set; }
        IPEndPoint RemoteEndPoint { get; set; }
        string GetHeader(string name);
        DateTime GetIfModifiedSince();
        string GetIfNoneMatch();
    }
}