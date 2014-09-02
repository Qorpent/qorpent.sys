﻿using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.Net{
	[TestFixture]
	public class HttpResultTests{
		HttpResult GetResult(string http)
		{
			var mstr = new MemoryStream(Encoding.UTF8.GetBytes(http));
			return new HttpResult(new HttpReader(mstr));
		}
		HttpResult GetResult(byte[] http)
		{
			var mstr = new MemoryStream(http);
			return new HttpResult(new HttpReader(mstr));
		}
		[Test]
		public void CanReadPreamble(){
			var s = GetResult("HTTP/1.1 200 OK\r\n\r\n");
			Assert.AreEqual("1.1",s.Version);
			Assert.AreEqual(200,s.State);
			Assert.AreEqual("OK",s.StateName);
		}
		[Test]
		public void CanReadMainHeaders()
		{
			var s = GetResult("HTTP/1.1 200 OK\r\nContent-Length: 500\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n");
			Assert.AreEqual(500, s.ContentLength);
			Assert.AreEqual("text/html", s.ContentType);
			Assert.AreEqual("UTF-8", s.Charset);
		}
		[Test]
		public void CanReadData()
		{
			var s = GetResult("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n3\r\nABC\r\n4\r\nDEFG\r\n\r\n");
			Assert.AreEqual("ABCDEFG", Encoding.ASCII.GetString(s.Data));
		}
		[Test]
		public void CanReadEncodedData()
		{
			Console.WriteLine(string.Join(",",Encoding.UTF8.GetBytes("абв")));
			Console.WriteLine(Encoding.UTF8.GetBytes("гдеж").Length);
			var s = GetResult("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n6\r\nабв\r\n8\r\nгдеж\r\n\r\n");
			Assert.AreEqual("абвгдеж", s.StringData);
		}
		[Test]
		public void CanReadCookie()
		{
			var s = GetResult("HTTP/1.1 200 OK\r\nSet-Cookie: yandexuid=8458776491409595482; Expires=Thu, 29-Aug-2024 18:18:01 GMT; Domain=.yandex.ru; Path=/\r\n\r\n");
			var cookie = s.Cookies["yandexuid"];
			Assert.AreEqual("8458776491409595482", cookie.Value);
			Assert.AreEqual(new DateTime(2024, 08, 29, 18, 18, 01).ToLocalTime(), cookie.Expires);
			Assert.AreEqual(".yandex.ru", cookie.Domain);
			Assert.AreEqual("/", cookie.Path);
		}

		[Test]
		public void CanUnGZip(){
			var dataStream = new MemoryStream();
			var gzip = new GZipStream(dataStream,CompressionLevel.Optimal,true);
			gzip.Write(Encoding.ASCII.GetBytes("hello world"),0,11);
			gzip.Flush();
			gzip.Close();
			var data = new byte[dataStream.Position];
			dataStream.Position = 0;
			dataStream.Read(data, 0, data.Length);
			var len = data.Length;
			var ms = new MemoryStream();
			var buff = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Length: " + len + "\r\nContent-Type: gzip\r\n\r\n");
			ms.Write(buff,0,buff.Length);
			ms.Write(data,0,data.Length);
			var http = new byte[ms.Position];
			ms.Position = 0;
			ms.Read(http, 0, http.Length);
			var s = GetResult(http);
			Console.WriteLine(s.ContentLength);
			Assert.AreEqual("hello world",s.StringData);
		}

		[Test]
		public void CanUnDeflate()
		{
			var dataStream = new MemoryStream();
			var deflate = new DeflateStream(dataStream, CompressionLevel.Optimal, true);
			deflate.Write(Encoding.ASCII.GetBytes("hello world"), 0, 11);
			deflate.Flush();
			deflate.Close();
			var data = new byte[dataStream.Position];
			dataStream.Position = 0;
			dataStream.Read(data, 0, data.Length);
			var len = data.Length;
			var ms = new MemoryStream();
			var buff = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Length: " + len + "\r\nContent-Type: deflate\r\n\r\n");
			ms.Write(buff, 0, buff.Length);
			ms.Write(data, 0, data.Length);
			var http = new byte[ms.Position];
			ms.Position = 0;
			ms.Read(http, 0, http.Length);
			var s = GetResult(http);
			Console.WriteLine(s.ContentLength);
			Assert.AreEqual("hello world", s.StringData);
		}
	}
}