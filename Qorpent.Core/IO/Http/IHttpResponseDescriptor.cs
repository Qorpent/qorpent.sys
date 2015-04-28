using System;
using System.IO;
using System.Net;
using System.Text;

namespace Qorpent.IO.Http {
    public interface IHttpResponseDescriptor {
       
        bool SupportGZip { get; set; }
        Stream Stream { get; set; }
        string ContentType { get; set; }
        int StatusCode { get; set; }
        string StatusDescription { get; set; }
        Encoding ContentEncoding { get; set; }
        CookieCollection Cookies { get; set; }
        bool NoCloseStream { get; set; }
        bool WasClosed { get; set; }
        void SetHeader(string name, string value);
        string GetETag();
        void SetETag(string etag);
        DateTime SetLastModified(DateTime time);
        void Finish(object data, string mime = "application/json", int status = 200);
        void Close();
        string GetHeader(string name);
        void Redirect(string localurl);
        void Write(object data, bool allowZip);
    }
}