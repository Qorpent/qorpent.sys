using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using Qorpent.Host;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.Net{
	[TestFixture]
	[Explicit]
	public class HttpReaderTimingTests{
		private const string _html = "<html><body>привет</body></html>";
		private const string _html2 = @"<html><body>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div><div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div><div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div><div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
<div>привет</div>
</body></html>";
		private HostServer host;
		private IPEndPoint httpend;
		private IPEndPoint httpsend;

		/// <summary>
		/// 
		/// </summary>
		[TestFixtureSetUp]
		public void FixtureSetup(){
			var hostConfig = new HostConfig();
			hostConfig.Bindings.Add(new HostBinding { Port = 50651, Schema = HostSchema.Http, Interface = "127.0.0.1" });
			hostConfig.Bindings.Add(new HostBinding { Port = 50652, Schema = HostSchema.Https, Interface = "127.0.0.1" });
			host = new HostServer(hostConfig);
			host.On("/x.html", _html, "text/hmtl");
			host.On("/x2.html", _html2, "text/hmtl");
			host.Start();
			httpend = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50651);
			httpsend = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50652);
		}
		[TestFixtureTearDown]
		public void FixtureTearDown(){
			host.Stop();
		}


		

		[TestCase(10)]
		[TestCase(100)]
		[TestCase(1000)]
		[TestCase(2000)]
		[Explicit]
		public void HttpSocket3TimeNativeSocket(int count)
		{
			var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50651);
			
			var req = new HttpRequest { Uri = new Uri("http://127.0.0.1:50651/x2.html") };
			var reader = new HttpResponseReader();
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < count; i++){
				HttpResponse r;
				using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)){
					socket.Connect(ep);
					using (var s = new NetworkStream(socket)){
						var w = new HttpRequestWriter(s);
						w.Write(req);
						r = reader.Read(s);
					}
					Assert.Greater(r.StringData.Length, 10);
				}

			}
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
		}

		[TestCase(10)]
		[TestCase(100)]
		[TestCase(1000)]
		[TestCase(2000)]
		[Explicit]
		public void WebClientTime(int count)
		{

			var wc = new WebClient();
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < count; i++)
			{
				var s = wc.DownloadString("http://127.0.0.1:50651/x2.html");
				Assert.Greater(s.Length, 10);
			}
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
		}

		[TestCase(10)]
		[Explicit]
		public void WebClientTimeYandexRu(int count)
		{

			var wc = new WebClient();
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < count; i++)
			{
				var s = wc.DownloadString("http://www.yandex.ru");
				Assert.Greater(s.Length, 10);
			}
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
		}

	


		
	}
}