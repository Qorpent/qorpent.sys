using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
	}
}
