using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Qorpent.IO.Http {
    public class HttpResponseDescriptor : IHttpResponseDescriptor {
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

        public static implicit operator HttpResponseDescriptor(WebContext context) {
            return (HttpResponseDescriptor) context.Response;
        }

        public bool WasClosed { get; set; }

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

        public void Finish(object data, string mime = "application/json", int status = 200) {
            StatusCode = status;
            if (mime.Contains("text") || mime.Contains("json")) {
                mime += "; charset=utf-8";
            }
            ContentType = mime;
            
            Write(data,true);

            Close();
        }

        public void Write(object data, bool allowZip) {
            var sendobject = GetSendObject(data);
            if (allowZip && SupportGZip && IsRequiredGZip(sendobject)) {

                SetHeader("Content-Encoding", "gzip");
                var ms = EnsureStream(sendobject);
                using (var g = new GZipStream(Stream, CompressionLevel.Optimal)) {
                    ms.CopyTo(g, 2 ^ 14);
                }
            }
            else {
                var bytes = sendobject as byte[];
                if (null != bytes) {
                    Stream.Write(bytes, 0, bytes.Length);
                }
                else {
                    ((Stream) sendobject).CopyTo(Stream);
                }
            }
        }

        private bool IsRequiredGZip(object sendobject) {
            if(ContentType.Contains("image"))return false;
            if (sendobject is byte[]) {
                return ((byte[]) sendobject).Length >= 640;
            }
            var s = sendobject as Stream;
            try {
                return s.Length >= 640;
            }
            catch {
                return true;
            }
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
                WasClosed = true;
            }
        }

        public virtual Stream Stream { get; set; }

        public virtual string ContentType { get; set; }

        public virtual int StatusCode { get; set; }
        public virtual string StatusDescription { get; set; }
        public virtual Encoding ContentEncoding { get; set; }

        protected IDictionary<string, string> Headers { get; set; }
        public virtual CookieCollection Cookies { get; set; }
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