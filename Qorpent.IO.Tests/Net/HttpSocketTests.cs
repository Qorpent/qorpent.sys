using System;
using System.Diagnostics;
using System.Net;
using NUnit.Framework;
using Qorpent.Host;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.Net{
	[TestFixture]
	public class HttpSocketTests{
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

		[Test]
		public void CanSimplyReadContent(){
			var hs = new HttpSocket(httpend);
			var response = hs.Call(new HttpRequest{Uri = new Uri("http://127.0.0.1:50651/x.html")});
			var result = response.StringData;
			Assert.AreEqual(_html,result);
		}

		[Test]
		public void CanSimplyReadContentSsl()
		{
			var hs = new HttpSocket(httpsend,true);
			var response = hs.Call(new HttpRequest { Uri = new Uri("https://127.0.0.1:50652/x.html") });
			var result = response.StringData;
			Assert.AreEqual(_html, result);
		}


		[Test]
		public void CanDoAutoLoad(){
			HttpResponse response = null;
			using (var hs = new HttpSocket(httpend){AutoLoad = true}){
				response = hs.Call(new HttpRequest { Uri = new Uri("http://127.0.0.1:50651/x.html") });	
			}
			Assert.AreEqual(_html, response.StringData);
		}


		[Test]
		[Explicit]
		public void HttpSocketTimeOn10000(){
			var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50651);
			var hs = new HttpSocket(ep){AutoLoad = true};
			var req = new HttpRequest{Uri = new Uri("http://127.0.0.1:50651/x2.html")};
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < 10000; i++){
				hs.Call(req);
			}
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
		}

		[Test]
		[Explicit]
		public void WebClientTimeOn10000(){

			var wc = new WebClient();
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < 10000; i++)
			{

				wc.DownloadString("http://127.0.0.1:50651/x2.html");
			}
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
		} 

		
	}
}