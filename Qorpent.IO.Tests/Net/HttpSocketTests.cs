using System;
using System.Net;
using NUnit.Framework;
using Qorpent.Host;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.Net{
	[TestFixture]
	public class HttpSocketTests{
		private const string _html = "<html><body>привет</body></html>";
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

		
	}
}