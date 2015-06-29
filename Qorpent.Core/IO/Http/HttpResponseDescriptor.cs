using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.IO.Net;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Http {
    public class HttpResponseDescriptor : IHttpResponseDescriptor {
        public const string ETagHeader = "Etag";
        public const string LastModifiedHeader = "Last-Modified";
        public static implicit operator HttpResponseDescriptor(HttpListenerContext context) {
            return new HttpListenerResponseDescriptor(context.Response) {
                SupportGZip = ((HttpRequestDescriptor) context).GetHeader("Accept-Encoding").Contains("gzip"),
            
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
        
        public virtual void AddHeader(string name, string value) {
            if (Headers.ContainsKey(name)) {
                Headers[name] = Headers[name] + "," + value;
            }
            else {
                Headers[name] = value;
            }
            //invalid behavior
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

        public void Finish(object data, string mime = "application/json", int status = 200, RangeDescriptor range = null) {
            StatusCode = status;
            if (mime.Contains("text") || mime.Contains("json")) {
                mime += "; charset=utf-8";
            }
            ContentType = mime;
            if (null != range) {
                StatusCode = 206;
                AddHeader("Content-Range",string.Format("bytes {0}-{1}/{2}",range.Start,range.Finish,range.Total));
                var length = range.Finish - range.Start + 1;
                SetHeader("Content-Length",length.ToString());
            }
            
            WriteCookies();
            
            Write(data, true, range);
            Close();
        }

        public void WriteCookies() {
            if (null == Cookies) return;   
            if (Cookies.Count > 0)
            {
   
                
                var cookies = Cookies;
                Cookies = null;
                IList<string> _visited = new List<string>();
                var cooks = cookies.OfType<Cookie>().Reverse().ToArray();
                foreach (var cookie in cooks)
                {
                    if(string.IsNullOrWhiteSpace(cookie.Domain))continue;
                    
                    if (_visited.Contains(cookie.Name)) continue;
                    _visited.Add(cookie.Name);
                    var c = GetCookieString(cookie);
       
                    AddHeader("Set-Cookie",c);
                }
            }
        }

        private string GetCookieString(Cookie cookie) {
            var maxage = -1;
            if (cookie.Expires.ToUniversalTime() > DateTime.Now.ToUniversalTime()) {
                maxage = (cookie.Expires.ToUniversalTime() - DateTime.Now.ToUniversalTime()).TotalSeconds.ToInt();
            }
            var domain = HttpUtils.AdaptCookieDomain( cookie.Domain);
            
            var result= string.Format("{0}={1}; Path={2}; Max-Age={3}; Domain={4};", cookie.Name, cookie.Value, cookie.Path,maxage,domain);
            if (cookie.HttpOnly) {
                result += "HttpOnly;";
            }
            if (cookie.Secure) {
                result += "Secure;";
            }
            return result;
        }


        public void Write(object data, bool allowZip, RangeDescriptor range) {
            var sendobject = GetSendObject(data);
            if (allowZip && SupportGZip && IsRequiredGZip(sendobject)) {
                SetHeader("Content-Encoding", "gzip");
                var ms = EnsureStream(sendobject);
                var str = Stream;
                if (ms.Length < 1024000 && null==range) {
                    str = new MemoryStream();
                }
                using (var g = new GZipStream(str, CompressionLevel.Optimal,leaveOpen:str!=Stream)) {
                    if (null == range) {
                        ms.CopyTo(g, 4096);
                    }
                    else {
                        CopyRanged(range, ms, g);
                    }
                }

                if (str != Stream) {
                    SetHeader("Content-Length", str.Length.ToStr());
                    str.Position = 0;
                    str.CopyTo(Stream);
                }

            }
            else {
                var bytes = sendobject as byte[];
                if (null != bytes) {
                    var start = null == range ? 0 : range.Start;
                    var length = null == range ? bytes.Length : range.Finish - range.Start + 1;
                    Stream.Write(bytes, (int)start, (int)length);
                }
                else {
                    if (null != range) {
                        CopyRanged(range, (Stream) sendobject, Stream);
                    }
                    else {
                        ((Stream)sendobject).CopyTo(Stream);    
                    }
                    
                }
            }
        }

        private static void CopyRanged(RangeDescriptor range, Stream source, Stream target) {
            source.Seek(range.Start, SeekOrigin.Current);
            var totalLength = range.Finish - range.Start + 1;
            var currentLength = 0;
            var buffer = new byte[524288];
            
            while (true) {
                if (currentLength >= totalLength) {
                    break;
                }
                ;
                var readsize = Math.Min(524288, totalLength - currentLength);
                var currentChunk = source.Read(buffer, 0, (int)readsize);
                if (currentChunk <= 0) {
                    break;
                }
                currentLength += currentChunk;
                target.Write(buffer, 0, currentChunk);

            }

    
        }

        private bool IsRequiredGZip(object sendobject) {
            if(ContentType.Contains("image"))return false;
            if(ContentType.Contains("video"))return false;
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
            
            object realdata = GetDataInternal(data);
            if (string.IsNullOrWhiteSpace(Range)) {
                
            }

            return realdata;

        }

        public string Range { get; set; }

        private static object GetDataInternal(object data) {
            if (null == data) {
                return new byte[] {};
            }
            if (data is Stream) {
                return data;
            }
            if (data is byte[]) {
                return (byte[]) data;
            }
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
        public HttpRequestDescriptor CorrespondRequest { get; set; }

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

    public class RangeDescriptor {
        public long Start { get; set; }
        public long Finish { get; set; }
        public long Total { get; set; }
    }
}