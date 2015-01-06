using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.Net{
	[TestFixture(Description = "Проверка HTTP ридера в 3-е редакции")]
	public class HttpResponseReaderTests{
		private Stream GetStream(string http, Encoding enc=null){
			enc = enc ?? Encoding.UTF8;
			return new MemoryStream(enc.GetBytes(http));
			
		}

	    [Test]
	    public void CookieParsedWell() {
            var cookiestr = "p=99aee31d64ef04c3b049c19ffff99c5943800e05475d4c8fd97a3; expires=Tue, 05 Jan 2016 21:51:10 GMT; path=/; domain=login.vk.com; secure; HttpOnly";
	        var cookies = HttpUtils.ParseCookies(cookiestr);
            Assert.AreEqual(1,cookies.Count());
	        var cookie = cookies.First();
            Assert.AreEqual("p",cookie.Name);
            Assert.AreEqual("99aee31d64ef04c3b049c19ffff99c5943800e05475d4c8fd97a3", cookie.Value);
	    }

		[Test]
		[Explicit]
		public void KnowlegeBase_ASCII_Encoding_Against_CharCast(){
			var buffer = new byte[4096];
			for (var i = 0; i < buffer.Length/64; i += 64){
				for (byte c = 32; c < 32 + 64; c++){
					buffer[i + c] = c;
				}
			}
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < 100000; i++){
				var str = Encoding.ASCII.GetString(buffer);
			}
			sw.Stop();
			Console.Write("ASCII: "+sw.Elapsed);
			sw = Stopwatch.StartNew();
			for (var i = 0; i < 100000; i++){
				var sb = new StringBuilder(4096);
				for (var j = 0; j < 4096; j++){
					sb.Append((char) buffer[j]);
				}
				var str = sb.ToString();
			}
			sw.Stop();
			Console.Write("CAST: " + sw.Elapsed);
		}
		

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(5)]
		[TestCase(7)]
		[TestCase(11)]
		[TestCase(13)]
		[TestCase(17)]
		[TestCase(19)]
		[TestCase(23)]
		[TestCase(29)]
		[TestCase(31)]
		[TestCase(1024)]
		[TestCase(8192)]
		public void CanReadPreamble(int pageSize)
		{
			var b = new StringBuilder();
			var s = GetStream("HTTP/1.1 200 OK\r\n\r\n");
			var reader = new HttpResponseReader{BufferSize = pageSize};
			var resp = reader.Read(s);
			Assert.AreEqual(200,resp.State);
			Assert.AreEqual("OK",resp.StateName);
			Assert.True(resp.Success);
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(5)]
		[TestCase(7)]
		[TestCase(11)]
		[TestCase(13)]
		[TestCase(17)]
		[TestCase(19)]
		[TestCase(23)]
		[TestCase(29)]
		[TestCase(31)]
		[TestCase(1024)]
		[TestCase(8192)]
		public void CanReadHeaders(int pageSize)
		{
			var b = new StringBuilder();
			var s = GetStream("HTTP/1.1 200 OK\r\nAaa: Bbb\r\nCcc: Ddd\r\n\r\n");
			var reader = new HttpResponseReader { BufferSize = pageSize };
			var resp = reader.Read(s);
			Assert.AreEqual(200, resp.State);
			Assert.AreEqual("OK", resp.StateName);
			Assert.AreEqual("Bbb",resp.Headers["Aaa"]);
			Assert.AreEqual("Ddd",resp.Headers["Ccc"]);
			Assert.True(resp.Success);
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(5)]
		[TestCase(7)]
		[TestCase(11)]
		[TestCase(13)]
		[TestCase(17)]
		[TestCase(19)]
		[TestCase(23)]
		[TestCase(29)]
		[TestCase(31)]
		[TestCase(1024)]
		[TestCase(8192)]
		public void CanReadData(int pageSize)
		{
			var b = new StringBuilder();
			var s = GetStream("HTTP/1.1 200 OK\r\nAaa: Bbb\r\nCcc: Ddd\r\nContent-Length: 10\r\n\r\nABCDEFGHIJ");
			var reader = new HttpResponseReader{BufferSize = pageSize};
			var resp = reader.Read(s);
			Assert.AreEqual(200, resp.State);
			Assert.AreEqual("OK", resp.StateName);
			Assert.AreEqual("Bbb", resp.Headers["Aaa"]);
			Assert.AreEqual("Ddd", resp.Headers["Ccc"]);
			Assert.AreEqual(10,resp.ContentLength);
			Assert.AreEqual(10,resp.Data.Length);
			Assert.AreEqual("ABCDEFGHIJ", resp.StringData);
			Assert.True(resp.Success);
		}
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(5)]
		[TestCase(7)]
		[TestCase(11)]
		[TestCase(13)]
		[TestCase(17)]
		[TestCase(19)]
		[TestCase(23)]
		[TestCase(29)]
		[TestCase(31)]
		[TestCase(1024)]
		[TestCase(8192)]
		public void CanReadChunkedData(int pageSize)
		{
			var b = new StringBuilder();
			var s = GetStream("HTTP/1.1 200 OK\r\nAaa: Bbb\r\nCcc: Ddd\r\nTransfer-Encoding: chunked\r\n\r\n2\r\nAB\r\n3\r\nCDE\r\n5\r\nFGHIJ\r\n0\r\n");
			var reader = new HttpResponseReader{BufferSize = pageSize};
			var resp = reader.Read(s);
			Assert.AreEqual(200, resp.State);
			Assert.AreEqual("OK", resp.StateName);
			Assert.AreEqual("Bbb", resp.Headers["Aaa"]);
			Assert.AreEqual("Ddd", resp.Headers["Ccc"]);
			Assert.True( resp.Chunked);
			Assert.AreEqual("ABCDEFGHIJ", resp.StringData);
			Assert.AreEqual(10, resp.Data.Length);
			
			Assert.True(resp.Success);
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(5)]
		[TestCase(7)]
		[TestCase(11)]
		[TestCase(13)]
		[TestCase(17)]
		[TestCase(19)]
		[TestCase(23)]
		[TestCase(29)]
		[TestCase(31)]
		[TestCase(1024)]
		[TestCase(8192)]
		public void CanReadEncodedData(int pageSize)
		{
			var s = GetStream("HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=Windows-1251\r\nAaa: Bbb\r\nCcc: Ddd\r\nContent-Length: 10\r\n\r\nабвгдежзик",Encoding.GetEncoding("Windows-1251"));
			var reader = new HttpResponseReader{BufferSize = pageSize};
			var resp = reader.Read(s);
			Assert.AreEqual(200, resp.State);
			Assert.AreEqual("OK", resp.StateName);
			Assert.AreEqual("Bbb", resp.Headers["Aaa"]);
			Assert.AreEqual("Ddd", resp.Headers["Ccc"]);
			Assert.AreEqual(10, resp.ContentLength);
			Assert.AreEqual(10, resp.Data.Length);
			Assert.AreEqual("Windows-1251", resp.Charset);
			Assert.AreEqual("абвгдежзик", resp.StringData);
			Assert.True(resp.Success);
		}

	
	}
}