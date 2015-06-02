using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.IO.Http {
    public class WebContext  {
        private IHttpRequestDescriptor _request;
        private IHttpResponseDescriptor _response;
        private IPrincipal _user;

        public static implicit operator WebContext(HttpListenerContext context) {
            var request = (HttpRequestDescriptor) context;
            var response = (HttpResponseDescriptor) context;
            var result = new WebContext { Request = request, Response = response };
            result.Cookies = result.Cookies ?? context.Request.Cookies;
            request.Cookies = result.Cookies;
            result.User = context.User;
            return result;
        }

        public static implicit operator WebContext(string url) {
            var request = (HttpRequestDescriptor) url;
            return new WebContext {Request = request};
        }

        public CookieCollection Cookies { get; set; }

        public IHttpRequestDescriptor Request {
            get { return _request ?? (_request = new HttpRequestDescriptor()); }
            set { _request = value; }
        }

        public IHttpResponseDescriptor Response {
            get { return _response ?? (_response = new HttpResponseDescriptor()); }
            set { _response = value; }
        }

        public Uri Uri {
            get { return Request.Uri; }
            set { Request.Uri = value; }
        }

        public IPrincipal User {
            get { return _user ?? Request.User; }
            set {
                _user = value;
                Request.User = value;
            }
        }

        public string Method {
            get { return Request.Method; }
            set { Request.Method = value; }
        }

        public void Finish(object data, string mime = "application/json", int status = 200) {
            Response.Finish(data,mime,status);
        }

        public void Write(object data, bool allowzip = false) {
            Response.Write(data,allowzip);
        }

        public bool IsMultipartForm {
            get {
                return null != Request.ContentType && Request.ContentType.Contains("multipart/form-data");
            }
        }

        public bool IsPost {
            get { return Request.Method.ToUpperInvariant() == "POST"; }
        }

        public long ContentLength {
            get { return Request.ContentLength; }
            set {  Request.ContentLength = value; }
        }

        public string InContentType {
            get { return Request.ContentType; }
            set { Request.ContentType = value; }
        }

        public string ContentType
        {
            get { return Response.ContentType; }
            set { Response.ContentType = value; }
        }

        public Encoding ContentEncoding {
            get { return Response.ContentEncoding; }
            set { Response.ContentEncoding = value; }
        }

        public string UserHostAddress {
            get { return Request.UserHostAddress; }
            set {
                Request.UserHostAddress = value;
            }
        }
        public string UserHostName
        {
            get { return Request.UserHostName; }
            set
            {
                Request.UserHostName = value;
            }
        }
        public string UserAgent
        {
            get { return Request.UserAgent; }
            set
            {
                Request.UserAgent = value;
            }
        }

        public string[] UserLanguages {
            get { return Request.UserLanguages; }
            set { Request.UserLanguages = value; }
        }

        public int StatusCode {
            get { return Response.StatusCode; }
            set { Response.StatusCode = value; }
        }

        public Stream Stream {
            get { return Response.Stream; }
            set { Response.Stream = value; }
        }


        public void ReadRequest(Stream targetstream) {
            Request.Stream.CopyTo(targetstream);
        }

        public byte[] ReadRequest() {
            var buffer = new byte[Request.ContentLength];
            Request.Stream.Read(buffer, 0, (int)Request.ContentLength);
            return buffer;
        }

        public async Task<byte[]> ReadRequestAsync() {
            var buffer = new byte[Request.ContentLength];
            await Request.Stream.ReadAsync(buffer, 0, (int)Request.ContentLength);
            return buffer;
        }

        public string ReadRequestString() {
            return Request.Encoding.GetString(ReadRequest());
        }

        public async Task<string> ReadRequestStirngAsync() {
            var data = await ReadRequestAsync();
            return Request.Encoding.GetString(data);
        }

        public DateTime SetLastModified(DateTime version) {
            return Response.SetLastModified(version);
        }

        public DateTime GetIfModifiedSince() {
            return Request.GetIfModifiedSince();
        }

        public void SetHeader(string name, string value) {
            Response.SetHeader(name,value);
        }

        public string GetHeader(string name) {
            return Request.GetHeader(name);
        }

        public bool HasHeader(string name) {
            return Request.Headers.ContainsKey(name);
        }

        public string GetIfNoneMatch() {
            return Request.GetIfNoneMatch();
        }

        public void SetETag(string tag) {
            Response.SetETag(tag);
        }

        public void Redirect(string localurl) {
            Response.Redirect(localurl);
        }
    }
}