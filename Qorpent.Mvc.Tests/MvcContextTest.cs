#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : MvcContextTest.cs
// Project: Qorpent.Mvc.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Applications;

namespace Qorpent.Mvc.Tests {
	[TestFixture]
	public class MvcContextTest {
		public class sampleResponse : HttpResponseBase {
			public override NameValueCollection Headers {
				get { return _headers; }
			}

			private readonly NameValueCollection _headers = new NameValueCollection();
		}

		public class sampleContext : HttpContextBase {
			public sampleContext(sampleRequest req, sampleResponse res) {
				_request = req;
				_response = res;
			}

			public override HttpRequestBase Request {
				get { return _request; }
			}

			public override HttpResponseBase Response {
				get { return _response; }
			}

			private readonly sampleRequest _request;
			private readonly sampleResponse _response;
		}

		public class sampleRequest : HttpRequestBase {
			public override NameValueCollection Headers {
				get { return _headers; }
			}

			public override NameValueCollection QueryString {
				get { return _qs; }
			}

			public override NameValueCollection Form {
				get { return _fm; }
			}

			private readonly NameValueCollection _fm = new NameValueCollection();
			private readonly NameValueCollection _headers = new NameValueCollection();

			private readonly NameValueCollection _qs = new NameValueCollection();
		}

		[Test]
		public void Action_And_Render_Name_Was_SetedUp_Normally() {
			var ctx = new MvcContext {Uri = new Uri("http://localhost/mY/echo.bXl.qweb?x=1")};
			Assert.AreEqual("my.echo", ctx.ActionName);
			Assert.AreEqual("bxl", ctx.RenderName);
		}

		[Test]
		public void Parameters_Can_Be_Evaluated_From_Query_String() {
			var ctx = new MvcContext {Uri = new Uri("http://localhost/mY/echo.bXl.qweb?x=1&y=2")};
			Assert.AreEqual(1, ctx.Get<int>("x"));
			Assert.AreEqual(2, ctx.Get<int>("y"));
		}

		[Test]
		public void bug_not_well_action_render_specification() {
			var ctx = new MvcContext("http://localhost/test/act2.xml.qweb");
			Assert.AreEqual("test.act2", ctx.ActionName);
			Assert.AreEqual("xml", ctx.RenderName);
		}

		[Test]
		public void bug_not_well_action_render_specification2() {
			var ctx = new MvcContext("http://localhost/action1.xml.qweb");
			Assert.AreEqual("action1", ctx.ActionName);
			Assert.AreEqual("xml", ctx.RenderName);
		}

		[Test]
		public void command_parsed() {
			Assert.AreEqual("start.end", new MvcContext("http://server/start/end.qweb?param").ActionName);
			Assert.AreEqual("start.end", new MvcContext("http://server/start/end.json.qweb?param").ActionName);
		}

		[Test]
		public void context_logon_user_and_user() {
			var test = new GenericPrincipal(new GenericIdentity("test"), null);
			var ctx = new MvcContext {LogonUser = test};
			MvcContextBase.Current = ctx;
			Assert.AreEqual("test", ctx.LogonUser.Identity.Name);
			Assert.AreEqual("test", ctx.User.Identity.Name);
		}

		[Test]
		public void multiple_name_param() {
			var req = new HttpRequest("", "http://server/app/start/end.wiki.qweb", "y=1&y=2");
			var ctx = new HttpContext(req, new HttpResponse(null));
			var context = new MvcContext(new System.Web.HttpContextWrapper(ctx));
			Assert.AreEqual("1,2", context.Parameters["y"]);
		}

		[Test]
		public void type_parsed() {
			Assert.AreEqual("xml", new MvcContext("http://server/app/start/end.qweb?param").RenderName);
			Assert.AreEqual("json", new MvcContext("http://server/app/start/end.json.qweb?param").RenderName);
			Assert.AreEqual("wiki", new MvcContext("http://server/app/start/end.wiki.qweb?param").RenderName);
		}

		[Test]
		public void works_with_httpcontext() {
			var req = new HttpRequest("", "http://server/start/end.wiki.qweb?param", "x=1&y=2");
			var ctx = new HttpContext(req, new HttpResponse(null));
			var context = new MvcContext(new System.Web.HttpContextWrapper(ctx));
			Assert.AreEqual("start.end", context.ActionName);
			Assert.AreEqual("wiki", context.RenderName);
			Assert.AreEqual("1", context.Parameters["x"]);
			Assert.AreEqual("2", context.Parameters["y"]);
		}

		[Test]
		public void xdata_by_bxl() {
			Assert.AreEqual(@"<object x=""1"" />",
			                new MvcContext().Set("_bxldata", @"object x=1").XData.ToString(SaveOptions.DisableFormatting));
		}

		[Test]
		public void xdata_by_json() {
			Assert.AreEqual(@"<object __jsontype=""object"" x=""1"" />",
			                new MvcContext().Set("_jdata", "{ x : 1}").XData.ToString(SaveOptions.DisableFormatting));
		}

		[Test]
		public void xdata_by_xml() {
			Assert.AreEqual(@"<object x=""1"" />",
			                new MvcContext().Set("_xdata", @"<object x=""1""/>").XData.ToString(SaveOptions.DisableFormatting));
		}

		[Test]
		public void xdata_can_be_null() {
			Assert.AreEqual(null, new MvcContext().XData);
		}
	}
}