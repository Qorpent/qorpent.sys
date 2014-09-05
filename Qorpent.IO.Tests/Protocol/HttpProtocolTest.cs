using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.Net;
using Qorpent.IO.Protocols;

namespace Qorpent.IO.Tests.Protocol{
	/// <summary>
	/// Тесты для работы с HTTP
	/// </summary>
	[TestFixture]
	public class HttpProtocolTest
	{
		
		Stream GetStream(string request){
			var bytes = Encoding.ASCII.GetBytes(request);
			return new MemoryStream(bytes);
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
		[TestCase(1024)]
		public void CanReadState(int pageSize){
			var stream = GetStream("HTTP/1.1 200 OK\r\n\r\n");
			var buf = new BufferManager(new ProtocolBufferOptions(pageSize));
			var protocol = new HttpProtocol();
			var res = buf.Read(stream, protocol);
			
			if (null != res.Error){
				Console.WriteLine(res.Error.ToString());
			}

			Assert.AreEqual(200, protocol.Response.State);
			Assert.AreEqual("OK", protocol.Response.StateName);
			Assert.True(protocol.Success);
			Assert.True(res.Ok);
			Assert.True(res.Success);
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
		[TestCase(1024)]
		public void CanReadHeaders(int pageSize)
		{
			var stream = GetStream("HTTP/1.1 200 OK\r\nA: B\r\nC: D\r\n\r\n");
			var buf = new BufferManager(new ProtocolBufferOptions(pageSize));
			var protocol = new HttpProtocol();
			var res = buf.Read(stream, protocol);

			if (null != res.Error)
			{
				Console.WriteLine(res.Error.ToString());
			}

			Assert.AreEqual(200, protocol.Response.State);
			Assert.AreEqual("OK", protocol.Response.StateName);
			Assert.AreEqual("B", protocol.Response.Headers["A"]);
			Assert.AreEqual("D", protocol.Response.Headers["C"]);
			Assert.True(protocol.Success);
			Assert.True(res.Ok);
			Assert.True(res.Success);
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
		[TestCase(1024)]
		public void CanReadFixedContent(int pageSize)
		{
			var stream = GetStream("HTTP/1.1 200 OK\r\nContent-Length: 4\r\n\r\nXXXX");
			var buf = new BufferManager(new ProtocolBufferOptions(pageSize));
			var protocol = new HttpProtocol();
			var res = buf.Read(stream, protocol);

			if (null != res.Error)
			{
				Console.WriteLine(res.Error.ToString());
			}

			Assert.AreEqual(200, protocol.Response.State);
			Assert.AreEqual("OK", protocol.Response.StateName);
			Assert.AreEqual("XXXX", protocol.Response.StringData);
			Assert.True(protocol.Success);
			Assert.True(res.Ok);
			Assert.True(res.Success);
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
		[TestCase(1024)]
		public void CanReadChunkedContent(int pageSize)
		{
			var stream = GetStream("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\n\r\n2\r\nXX\r\n2\r\nXX\r\n3\r\nZZZ\r\n0\r\n\r\n");
			var buf = new BufferManager(new ProtocolBufferOptions(pageSize));
			var protocol = new HttpProtocol();
			var res = buf.Read(stream, protocol);

			if (null != res.Error)
			{
				Console.WriteLine(res.Error.ToString());
			}

			Assert.AreEqual(200, protocol.Response.State);
			Assert.AreEqual("OK", protocol.Response.StateName);
			Assert.AreEqual("XXXXZZZ", protocol.Response.StringData);
			Assert.True(protocol.Success);
			Assert.True(res.Ok);
			Assert.True(res.Success);
		}
	}
}