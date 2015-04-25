using System;
using System.IO;
using System.Net;
using System.Text;

namespace Qorpent.IO.Http {
    internal class HttpListenerResponseDescriptor : HttpResponseDescriptor {
        private readonly HttpListenerResponse _response;

        public HttpListenerResponseDescriptor(HttpListenerResponse response) {
            _response = response;
            Cookies = _response.Cookies;
        }

        public override void SetHeader(string name, string value) {
            _response.Headers[name] = value ?? String.Empty;
        }

        public override string GetETag() {
            return _response.Headers[HttpResponseDescriptor.ETagHeader] ?? "";
        }

        public override void Close() {
            if(NoCloseStream)return;
            
            _response.Close();
        }

        public override string ContentType {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public override Stream Stream {
            get { return _response.OutputStream; }
            set { }
        }

        public override int StatusCode {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public override string StatusDescription {
            get { return _response.StatusDescription; }
            set { _response.StatusDescription = value; }
        }

        public override Encoding ContentEncoding {
            get { return _response.ContentEncoding; }
            set { _response.ContentEncoding = value; }
        }

        public override string GetHeader(string name) {
            return _response.Headers[name];
        }

        public override void Redirect(string localurl) {
            _response.Redirect(localurl);
        }
    }
}