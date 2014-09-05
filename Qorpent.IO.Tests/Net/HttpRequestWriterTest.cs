using System;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.Net{
	[TestFixture]
	public class HttpRequestWriterTest
	{
		string Execute(string url){
			return Execute(new HttpRequest{Uri = new Uri(url)});
		}
		string Execute(HttpRequest request){
			var ms = new MemoryStream();
			var hrw = new HttpRequestWriter(ms);
			hrw.Write(request);
			var size = ms.Position;
			ms.Position = 0;
			var buffer = new byte[size];
			ms.Read(buffer, 0, (int)size);
			return Encoding.ASCII.GetString(buffer);
		}

		[TestCase("http://a.com/x", "GET /x HTTP/1.1\r\nHost: a.com\r\nAccept-Encoding: gzip, deflate\r\nUser-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0\r\n\r\n")]
		[TestCase("http://a.com/x?a=1", "GET /x?a=1 HTTP/1.1\r\nHost: a.com\r\nAccept-Encoding: gzip, deflate\r\nUser-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0\r\n\r\n")]
		[TestCase("http://a.com/x?a=а", "GET /x?a=%D0%B0 HTTP/1.1\r\nHost: a.com\r\nAccept-Encoding: gzip, deflate\r\nUser-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0\r\n\r\n")]
		public void WriterCanPrepareRequest(string url, string result){
			var test = Execute(url);
			Console.Write(test.Replace("\r","\\r").Replace("\n","\\n"));
			Assert.AreEqual(result,test);
		}

		[Test]
		public void SupportCookies(){
			var req = new HttpRequest{Uri = new Uri("http://host.domain.com/x")};
			req.Cookies = new CookieCollection();
			req.Cookies.Add(new Cookie("x","1"));
			req.Cookies.Add(new Cookie("auth","1","/x","host.domain.com"));
			req.Cookies.Add(new Cookie("auth","2","/y","host.domain.com"));
			req.Cookies.Add(new Cookie("auth","3","/x","host.domain2.com"));
			req.Cookies.Add(new Cookie("z","4","",".domain.com"));
			req.Cookies.Add(new Cookie("z2","45","",".domain.com"){Expired = true});
			req.Cookies.Add(new Cookie("z","5","",".domain2.com"));
			var test = Execute(req);
			Console.Write(test.Replace("\r", "\\r").Replace("\n", "\\n"));
			Assert.AreEqual("GET /x HTTP/1.1\r\nHost: host.domain.com\r\nAccept-Encoding: gzip, deflate\r\nUser-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0\r\nCookie: x=1, auth=1, z=4\r\n\r\n", test);
		}
	}
}