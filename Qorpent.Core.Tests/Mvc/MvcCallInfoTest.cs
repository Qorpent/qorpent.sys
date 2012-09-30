using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.Mvc;

namespace Qorpent.Core.Tests.Mvc
{
	[TestFixture]
	public class MvcCallInfoTest
	{
		[TestCase("http://mydomain.com/test/_sys/echo.qweb", "test", "_sys.echo")]
		[TestCase("http://mydomain.com/test/_sys/echo.qweb", "", "test._sys.echo")]
		[TestCase("http://mydomain.com/_sys/echo.qweb","","_sys.echo")]
		public void valid_action_name(string url, string appname, string testaction) {
			var app = new Application();
			app.ApplicationName = appname;
			Application.Current = app;
			Assert.AreEqual(testaction,MvcCallInfo.GetActionName(new Uri(url)));
		}

		[Test]
		public void decode() {
			Console.WriteLine(Encoding.UTF8.GetString( Convert.FromBase64String("Y29tZGl2OnphcTEhUUFa")));
		}
	}
}
