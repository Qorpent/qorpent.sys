using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Qorpent.IO.Http {
    public class HttpResponseDescriptor {
        public const string ETagHeader = "Etag";
        public const string LastModifiedHeader = "Last-Modified";

        public static implicit operator HttpResponseDescriptor(HttpListenerContext context) {
            return new HttpListenerResponseDescriptor(context.Response) {
                SupportGZip = ((HttpRequestDescriptor) context).GetHeader("Accept-Encoding").Contains("gzip")
            };
        }

        public static implicit operator HttpResponseDescriptor(HttpListenerResponse response) {
            return new HttpListenerResponseDescriptor(response);
        }

        public virtual void SetHeader(string name, string value) {
            if (null != Headers) {
                Headers[name] = value;
            }
        }

        public virtual string GetETag() {
            return string.Empty;
        }

        public void SetETag(string etag) {
            SetHeader(ETagHeader, etag);
        }

        public DateTime SetLastModified(DateTime time) {
            var v = time;
            if (v.Year < 1900) {
                v = new DateTime(1900, 1, 1);
            }


            v = new DateTime(v.Year, v.Month, v.Day, v.Hour, v.Minute, v.Second);
            SetHeader(LastModifiedHeader, v.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture));

            if (string.IsNullOrWhiteSpace(GetETag())) {
                SetETag(v.ToOADate().ToString(CultureInfo.InvariantCulture));
            }
            return v;
        }

        public bool SupportGZip { get; set; }

        public class HttpListenerResponseDescriptor : HttpResponseDescriptor {
            private readonly HttpListenerResponse _response;

            public HttpListenerResponseDescriptor(HttpListenerResponse response) {
                _response = response;
                Cookies = _response.Cookies;
            }

            public override void SetHeader(string name, string value) {
                _response.Headers[name] = value ?? string.Empty;
            }

            public override string GetETag() {
                return _response.Headers[ETagHeader] ?? "";
            }

            public override void Close() {
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

        public void Finish(object data, string mime = "application/json", int status = 200) {
            StatusCode = status;
            ContentType = mime;
            var sendobject = GetSendObject(data);
            if (SupportGZip) {
                SetHeader("Content-Encoding", "gzip");
                var ms = EnsureStream(sendobject);
                using (var g = new GZipStream(Stream, CompressionLevel.Optimal)) {
                    ms.CopyTo(g, 2 ^ 14);

                }
            }
            else {
                var bytes = sendobject as byte[];
                if (null!=bytes) {
                    Stream.Write(bytes, 0, bytes.Length);
                }
                else {
                    ((Stream)sendobject).CopyTo(Stream);
                }
            }

            Close();
        }

        private Stream EnsureStream(object sendobject) {
            if (sendobject is Stream) return sendobject as Stream;
            return new MemoryStream(sendobject as byte[]);
        }

        private object GetSendObject(object data) {
            
            
            if(null==data)return new byte[]{};
            if (data is Stream) return data;
            if (data is byte[]) return (byte[]) data;
            return Encoding.UTF8.GetBytes(data.ToString());
        }

        public virtual void Close() {

            if (null != this.Stream && !NoCloseStream) {
                this.Stream.Close();
            }
        }

        public virtual Stream Stream { get; set; }

        public virtual string ContentType { get; set; }

        public virtual int StatusCode { get; set; }
        public virtual string StatusDescription { get; set; }
        public virtual Encoding ContentEncoding { get; set; }

        protected IDictionary<string, string> Headers { get; set; }
        public CookieCollection Cookies { get; set; }
        public bool NoCloseStream { get; set; }

        public virtual string GetHeader(string name) {
            Headers = Headers ?? new ConcurrentDictionary<string, string>();
            if (null != Headers) {
                if (Headers.ContainsKey(name)) return Headers[name];
            }
            return null;
        }

        public virtual void Redirect(string localurl) {
            throw new NotImplementedException();
        }
    }
}