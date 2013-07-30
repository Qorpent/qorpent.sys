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
// Original file : SimpleMvcProcessing.cs
// Project: Qorpent.Mvc.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Mvc.Binding;
using Qorpent.Mvc.QView;

namespace Qorpent.Mvc.Tests {
	[TestFixture]
	public class SimpleMvcProcessing {
		[SetUp]
		public void setup() {
			container = ContainerFactory.CreateDefault();
			container.Register(
				container.NewExtension(ConsoleLogWriter.CreateLogger(level: LogLevel.All,
				                                                     customFormat: "${Level} ${Name} ${Message}")));
			app = new Application {Container = container};
			app.MvcFactory
				.Register(typeof (Action1))
				.Register(typeof (Action2))
				.Register(typeof (Act2view));
			handler = container.Get<IMvcHandler>();
		}


		private IContainer container;
		private Application app;
		private IMvcHandler handler;

		public class Action1 : IAction {
			public object Process(IMvcContext context) {
				return 1;
			}
		}

		[Action("test.act2",Role = "DEFAULT")]
		public class Action2 : IAction {
			public object Process(IMvcContext context) {
				return X;
			}

			[Bind(Default = 2)] private int X = 0;
		}

		[QView("test/act2", QViewLevel.Sys)]
		public class Act2view : QViewBase {
			protected override void Render() {
				write("hello " + ViewContext.ViewData);
			}
		}

		[Test]
		public void EnvironmentPrepared() {
			Assert.NotNull(container.Get<IAction>("action1.action"));
			Assert.NotNull(container.Get<IAction>("test.act2.action"));
			Assert.NotNull(container.Get<IQView>("test/act2.sys.view"));
			Assert.NotNull(app.MvcFactory.GetView("test/act2"));
		}

		[Test]
		public void action1_executed() {
			var ctx = app.CreateContext("http://localhost/action1.xml.qweb");
			handler.ProcessRequest(ctx);
			Assert.AreEqual(1, ctx.ActionResult);
		}

		[Test]
		public void action1_rendered() {
			var ctx = app.CreateContext("http://localhost/action1.xml.qweb");
			handler.ProcessRequest(ctx);
			Assert.AreEqual("<root><value>1</value></root>", ctx.Output.ToString());
		}

		[Test]
		public void action2_executed() {
			var ctx = app.CreateContext("http://localhost/test/act2.xml.qweb");
			handler.ProcessRequest(ctx);
			Assert.AreEqual(2, ctx.ActionResult);
		}

		[Test]
		public void action2_executed_with_binding() {
			var ctx = app.CreateContext("http://localhost/test/act2.xml.qweb?x=33");
			handler.ProcessRequest(ctx);
			Assert.AreEqual(33, ctx.ActionResult);
		}


		[Test]
		public void action2_executed_with_qview() {
			var ctx = app.CreateContext("http://localhost/test/act2.qview.qweb?x=33");
			handler.ProcessRequest(ctx);
			Assert.AreEqual(33, ctx.ActionResult);
			Console.WriteLine(ctx.Output.ToString());
			Assert.AreEqual("hello 33", ctx.Output.ToString());
		}


		[Test]
		public void action2_rendered() {
			var ctx = app.CreateContext("http://localhost/test/act2.xml.qweb");
			handler.ProcessRequest(ctx);
			Console.WriteLine(ctx.Output.ToString());
			Assert.AreEqual("<root><value>2</value></root>", ctx.Output.ToString());
		}
	}
}