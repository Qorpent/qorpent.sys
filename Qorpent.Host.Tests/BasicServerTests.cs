using System;
using System.Linq;
using System.Net;
using NUnit.Framework;

namespace Qorpent.Host.Lib.Tests
{
	[TestFixture]
	public class BasicServerTests
	{
		private HostServer srv;
		/// <summary>
		/// 
		/// </summary>
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			this.srv = new HostServer(8094);
			srv.Start();
		}

		[Test]
		public void CanStartServer()
		{
			
			srv.On("/test", "CanStartServer");
			var wcl = new WebClient();
			var str = wcl.DownloadString("http://127.0.0.1:8094/test");
			Assert.AreEqual("CanStartServer", str);
		}
		[Test]
		public void Delegate()
		{
			srv.OnContext("/echo", 
				c => 
					c.Response.Finish(c.Request.Url.Query.Substring(1))
				);
			var wcl = new WebClient();
			var str = wcl.DownloadString("http://127.0.0.1:8094/echo?x");
			Assert.AreEqual("x", str);
		}

		[Test]
		public void SupportsCrossSite(){
			srv.On("/test", "test");
			var req = WebRequest.Create("http://127.0.0.1:8094/test");
			req.Headers["Access-Control-Request-Headers"] = "x-other";
			var resp = req.GetResponse();
			Assert.False(resp.Headers.AllKeys.Any(_ => _.StartsWith("Access-Control")));
			srv.Config.AccessAllowOrigin = "*";
			req = WebRequest.Create("http://127.0.0.1:8094/test");
			req.Headers["Access-Control-Request-Headers"] = "x-other";
			resp = req.GetResponse();
			Assert.AreEqual(4, resp.Headers.AllKeys.Count(_ => _.StartsWith("Access-Control")));
		}

		[Test]
		public void SupportsOptionsMethod()
		{
			srv.On("/test", "test");
			srv.Config.AccessAllowOrigin = "*";
			var req = WebRequest.Create("http://127.0.0.1:8094/test");
			req.Headers["Access-Control-Request-Headers"] = "x-other";
			req.Method = "OPTIONS";
			var resp = (HttpWebResponse)req.GetResponse();
			Assert.AreEqual(0,resp.ContentLength);
			Assert.AreEqual(HttpStatusCode.OK,resp.StatusCode);
			Assert.AreEqual(4, resp.Headers.AllKeys.Count(_ => _.StartsWith("Access-Control")));
			Assert.True(!string.IsNullOrWhiteSpace(resp.Headers["Allow"]));
		}

		[Test]
		public void Uson()
		{
			srv.On("/calc",q =>
			new{
				sum = q.a + q.b,
				diff = q.b - q.a,
				perc = q.b/(decimal)q.a*100,
				div = (q.b - q.a)/(decimal)q.a*100,
				mid = (q.a + q.b)/2
			});


			var wcl = new WebClient();
			var str = wcl.DownloadString("http://127.0.0.1:8094/calc?a=10&b=12");
			Assert.AreEqual(@"{""diff"":2,""div"":20,""mid"":11,""perc"":120,""sum"":22}", str);

			str = wcl.DownloadString(@"http://127.0.0.1:8094/calc?{""a"":10,""b"":12}");
			Assert.AreEqual(@"{""diff"":2,""div"":20,""mid"":11,""perc"":120,""sum"":22}", str);

			str = wcl.DownloadString(@"http://127.0.0.1:8094/calc/xml?{""a"":10,""b"":12}");
			Console.WriteLine(str);
			Assert.AreEqual(@"<result sum=""22"" diff=""2"" perc=""120"" div=""20"" mid=""11"" />", str);

			str = wcl.DownloadString(@"http://127.0.0.1:8094/calc/xml?{""a"":""10"",""b"":""12""}");
			Console.WriteLine(str);
			Assert.AreEqual(@"<result sum=""22"" diff=""2"" perc=""120"" div=""20"" mid=""11"" />", str);


			str = wcl.UploadString("http://127.0.0.1:8094/calc/xml", @"{""a"":""10"",""b"":""12""}");
			Console.WriteLine(str);
			Assert.AreEqual(@"<result sum=""22"" diff=""2"" perc=""120"" div=""20"" mid=""11"" />", str);

			str = wcl.UploadString("http://127.0.0.1:8094/calc/xml", @"a=10&b=12");
			Console.WriteLine(str);
			Assert.AreEqual(@"<result sum=""22"" diff=""2"" perc=""120"" div=""20"" mid=""11"" />", str);
		}
	}
}
