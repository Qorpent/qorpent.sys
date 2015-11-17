using System.IO;
using System.Net;
using System.Text;
using Qorpent.Utils.Extensions;
using static System.String;

namespace Qorpent.IO.Http {
    internal class HttpListenerResponseDescriptor : HttpResponseDescriptor {
        private readonly HttpListenerResponse _response;

        public HttpListenerResponseDescriptor(HttpListenerResponse response) {
            _response = response;
        }

        public override CookieCollection Cookies {
            get { return _response.Cookies; }
            set { _response.Cookies = value; }
        }

        public override long ConentLength
        {
            get { return _response.ContentLength64; }
            set { _response.ContentLength64 = value; }
        }
        public override void SetHeader(string name, string value) {
            if (name == "Content-Length") {
                _response.ContentLength64 = value.ToLong();
            }
            else {
                try {
                    _response.Headers[name] = value ?? Empty;
                }
                catch (ProtocolViolationException) {
                    
                }
            }
        }

        public override string GetETag() {
            return _response.Headers[ETagHeader] ?? "";
        }

        public override void AddHeader(string name, string value) {
            this._response.Headers.Add(name,value);
        }

        public override void Close() {
            if(NoCloseStream)return;
            WasClosed = true;
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