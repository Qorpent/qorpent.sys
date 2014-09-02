using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.Net
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture(Description = "Проверка HTTP сканера")]
	public class HttpReaderTests
	{

		HttpReader GetReader(string http){
			var mstr = new MemoryStream( Encoding.UTF8.GetBytes(http));
			return new HttpReader(mstr);
		}

		[Test]
		public void CanReadVersion(){
			var s = GetReader("HTTP/1.1 200 OK\r\n\r\n");
			var ver = s.Next();
			Assert.AreEqual(HttpEntityType.Version,ver.Type);
			Assert.AreEqual("1.1",ver.StringData);
		}

		[Test]
		public void DetectsFinishInSimpleNoHeaderRequest(){
			var b = new StringBuilder();
			var s = GetReader("HTTP/1.1 200 OK\r\n\r\n");
			HttpEntity e = null;
			while (null!=(e= s.Next())){
				b.Append(e.Type);
				b.Append("; ");
			}
			Assert.AreEqual("Version; State; StateName; Finish; ",b.ToString());
		}

		[Test]
		public void CanReadState()
		{
			var s = GetReader("HTTP/1.1 200 OK\r\n\r\n");
			s.Next();
			var ver = s.Next();
			Assert.AreEqual(HttpEntityType.State, ver.Type);
			Assert.AreEqual(200, ver.NumericData);
		}
		[Test]
		public void CanReadStateName()
		{
			var s = GetReader("HTTP/1.1 200 OK\r\n\r\n");
			s.Next();
			s.Next();
			var ver = s.Next();
			Assert.AreEqual(HttpEntityType.StateName, ver.Type);
			Assert.AreEqual("OK", ver.StringData);
		}
		[Test]
		public void CanReadLongStateName()
		{
			var s = GetReader("HTTP/1.1 400 Bad Request\r\n\r\n");
			s.Next();
			s.Next();
			var ver = s.Next();
			Assert.AreEqual(HttpEntityType.StateName, ver.Type);
			Assert.AreEqual("Bad Request", ver.StringData);
		}

		[Test]
		public void CanReadEmptyHeader()
		{
			var b = new StringBuilder();
			var s = GetReader("HTTP/1.1 200 OK\r\nC:\r\n\r\n");
			HttpEntity e = null;
			while (null != (e = s.Next()))
			{
				if(e.Error!=null)throw new Exception(e.Error.ToString());
				b.Append(e.Type);
				b.Append("; ");
			}
			Assert.AreEqual("Version; State; StateName; HeaderName; HeaderValue; Finish; ", b.ToString());
		}

		[Test]
		public void CanReadHeadersStack(){
			var b = new StringBuilder();
			var s = GetReader("HTTP/1.1 200 OK\r\nX: A\r\nY: B\r\nC:\r\n\r\n");
			HttpEntity e = null;
			while (null != (e = s.Next()))
			{
				b.Append(e.Type);
				b.Append("; ");
			}
			Assert.AreEqual("Version; State; StateName; HeaderName; HeaderValue; HeaderName; HeaderValue; HeaderName; HeaderValue; Finish; ", b.ToString());
		}


		[Test]
		public void CatchesSpecialHeaders()
		{
			var b = new StringBuilder();
			var s = GetReader("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\nContent-Type: text/html; charset=UTF-8\r\nContent-Length: 500\r\n\r\n");
			var all = s.Read().ToArray();
			Assert.AreEqual("chunked",s.TransferEncoding);
			Assert.AreEqual("text/html",s.ContentType);
			Assert.AreEqual(500,s.ContentLength);
			Assert.AreEqual("UTF-8",s.Charset);
		}

		[Test]
		public void Bug_CanReadEncodedData()
		{
			var s = GetReader("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n6\r\nабв\r\n8\r\nгдеж\r\n\r\n");
			var chunk = s.Read().FirstOrDefault(_ => _.Type == HttpEntityType.Chunk);
			Console.Write((char)63);
			Assert.AreEqual(208,chunk.BinaryData[0]);
		}

		[Test]
		public void CanReadHeadersStackWithValueChecks()
		{
			var b = new StringBuilder();
			var s = GetReader("HTTP/1.1 200 OK\r\nX: A\r\nY: B\r\nC:\r\n\r\n");
			HttpEntity e = null;
			string lastname = "";
			while (null != (e = s.Next()))
			{
				if (e.Type == HttpEntityType.HeaderName){
					lastname = e.StringData;
				}else if (e.Type == HttpEntityType.HeaderValue){
					b.AppendFormat("{0}={1}; ", lastname, e.StringData);
					lastname = "";
				}
			}
			Assert.AreEqual("X=A; Y=B; C=; ", b.ToString());
		}

		[Test]
		public void CanReadSimpleContentWithContentLength(){
			var s = GetReader("HTTP/1.1 200 OK\r\nContent-Length: 10\r\n\r\nABCDEFGHIJ");
			var es = s.Read().ToArray();
			var chunk = es.FirstOrDefault(_ => _.Type == HttpEntityType.Chunk);
			Assert.NotNull(chunk);
			Assert.AreEqual(10,chunk.Length);
			Assert.AreEqual(10,chunk.BinaryData.Length);
			Assert.AreEqual("ABCDEFGHIJ",Encoding.ASCII.GetString(chunk.BinaryData));
		}

		[Test]
		public void CanReadChunkedContentWithContentLength()
		{
			var s = GetReader("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\n\r\n0A\r\nABCDEFGHIJ\r\n2\r\nKL\r\n3\r\nMNO\r\n0");
			var es = s.Read().ToArray();
			var sb = new StringBuilder();
			foreach (var e in es.Where(_=>_.Type==HttpEntityType.Chunk)){
				sb.Append(Encoding.ASCII.GetString(e.BinaryData));
			}
			Assert.AreEqual("ABCDEFGHIJKLMNO",sb.ToString());
		}

		[Test]
		[Category("NOTC")]
		public void CanReadLiveSample(){
			var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			try{
				var addr = Dns.GetHostAddresses("www.yandex.ru");
				socket.Connect(addr, 80);
				socket.Send(Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: www.yandex.ru\r\n\r\n"));
				using (var s = new NetworkStream(socket)){

					var reader = new HttpReader(s);
					var ms = new MemoryStream();
					foreach (var chunk in reader.Read()){
						//Console.WriteLine(chunk.Type);
						if (chunk.Type == HttpEntityType.Chunk){
							ms.Write(chunk.BinaryData, 0, chunk.Length);
						}
					}
					ms.Position = 0;
					var content = Encoding.GetEncoding(reader.Charset).GetString(ms.GetBuffer(), 0, (int) ms.Length);
					//Console.WriteLine(content);
					Assert.True(content.Contains("Сделать&nbsp;Яндекс&nbsp;стартовой&nbsp;страницей"));
				}
			}
			finally{
				socket.Dispose();
			}
		}


		[Test]
		[Category("NOTC")]
		[Explicit]
		public void CanReadLiveSslSample()
		{
			var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			try{
				var addr = Dns.GetHostAddresses("www.google.ru");
				socket.Connect(addr, 443);
				using (var s = new NetworkStream(socket)){
					using (var ssl = new SslStream(s)){
						ssl.AuthenticateAsClient("www.google.ru");
						ssl.Write(Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: www.google.ru\r\n\r\n"));
						var reader = new HttpReader(ssl);
						var ms = new MemoryStream();
						foreach (var chunk in reader.Read()){
							//Console.WriteLine(chunk.Type);
							if (chunk.Type == HttpEntityType.Chunk){
								ms.Write(chunk.BinaryData, 0, chunk.Length);
							}
						}
						ms.Position = 0;
						var content = Encoding.GetEncoding(reader.Charset).GetString(ms.GetBuffer(), 0, (int) ms.Length);
						//Console.WriteLine(content);
						Assert.True(content.Contains("<title>Google</title>"));
					}
				}
			}
			finally{
				socket.Dispose();
			}
		}

		

		[Test]
		[Category("NOTC")]
		[Explicit]
		public void Performance20OnNotSsl()
		{
			for (var i = 0; i < 20; i++)
			{
				CanReadLiveSample();
				Console.Write(".");
			}
		}

		[Test]
		[Category("NOTC")]
		[Explicit]
		public void Performance20OnNotSslWebClient()
		{
			var cli = new WebClient();
			for (var i = 0; i < 20; i++){
			
				var page = cli.DownloadString("http://yandex.ru");
				Console.Write(".");
			}
		}

		// "consts" to help understand calculations
		const int bytesperlong = 4; // 32 / 8
		const int bitsperbyte = 8;

		

		[Test]
		[Category("NOTC")]
		[Explicit]
		public void TryReuseSameSocket(){
			var addr = Dns.GetHostAddresses("cdn.sstatic.net");
			var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			try
			{
				// i thought that i must to use this option
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
				socket.Connect(addr, 80);
				var bigbuff = new byte[10000];
				for (var i = 0; i < 20; i++)
				{
					
					//now i try to call some URL many times without socket closing
					var buff = Encoding.ASCII.GetBytes("GET /Js/stub.en.js?v=82eb4b63730d HTTP/1.1\r\nHost: cdn.sstatic.net\r\nConnection: Keep-Alive\r\n\r\n");
					socket.Send(buff);
					var reciveCount = 0;
					var totalCount = 0;
					var ns = new NetworkStream(socket);
					var r = new HttpReader(ns);
					r.Read().ToArray();
					while (socket.Available != 0)
					{
						socket.Receive(bigbuff);
					}
					//Console.WriteLine(totalCount);
					//only first call will proceed, all others will not have a result
					//Assert.AreNotEqual(0,totalCount);

				}

			}
			finally
			{
				socket.Dispose();
			}
		}


	


	}
}
