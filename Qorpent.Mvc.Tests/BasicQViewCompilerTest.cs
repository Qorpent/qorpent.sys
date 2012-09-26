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
// Original file : BasicQViewCompilerTest.cs
// Project: Qorpent.Mvc.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Mvc.QView;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.TestSupport;

namespace Qorpent.Mvc.Tests {
	[TestFixture]
	[QorpentFixture(UseTemporalFileSystem = true)]
	public class BasicQViewCompilerTest : QorpentFixture {
		[SetUp]
		public void setup() {
			container = ContainerFactory.CreateDefault();
			container.SetParameter<IFileNameResolver>("Root", Tmpdir);
			container.Register(container.NewExtension(ConsoleLogWriter.CreateLogger(customFormat: "${Level} ${Name} ${Message}")));
			compiler = container.Get<IQViewCompiler>();
			app = new Application {Container = container};
		}


		private IContainer container;
		private IQViewCompiler compiler;
		private Application app;

		[Action("my.test",Role="DEFAULT")]
		private class MyTest : IAction {
			public object Process(IMvcContext context) {
				return 45;
			}
		}


		[Test]
		public void Can_Compile_With_Resources() {
			var vbxlfile = Path.Combine(Tmpdir, "test.vbxl");
			var resfile = Path.Combine(Tmpdir, "test.1_ru___resb");
			File.WriteAllText(vbxlfile, "h1:hello@");
			File.WriteAllText(resfile, "hello Привет");
			var path = compiler.Compile(QViewCompileType.Full);
			var ass = Assembly.Load(File.ReadAllBytes(path));
			var factory = container.Get<IMvcFactory>();
			factory.Register(ass);
			IQView view = null;
			Assert.NotNull(view = factory.GetView("test"));
			var ctx = new QViewContext();
			var sw = new StringWriter();
			ctx.Output = sw;
			using (var s = ass.GetManifestResourceStream("test_Root_View.1_ru___resb")) {
				Assert.NotNull(s);
				Assert.AreEqual("hello Привет", new StreamReader(s).ReadToEnd());
			}
			view.Process(ctx);
			Console.WriteLine(sw.ToString());
			StringAssert.Contains("<h1>Привет</h1>", sw.ToString());
		}

		[Test]
		public void Can_Compile_With_Resources_FullName() {
			var vbxlfile = Path.Combine(Tmpdir, "sys\\my\\test.vbxl");
			var resfile = Path.Combine(Tmpdir, "sys\\my\\test.1_ru___resb");
			Directory.CreateDirectory(Tmpdir + "\\sys\\my");
			File.WriteAllText(vbxlfile, "h1:hello@");
			File.WriteAllText(resfile, "hello Привет");
			var path = compiler.Compile(QViewCompileType.Full);
			var ass = Assembly.Load(File.ReadAllBytes(path));
			var factory = container.Get<IMvcFactory>();
			factory.Register(ass);
			IQView view = null;
			Assert.NotNull(view = factory.GetView("my/test"));
			var ctx = new QViewContext();
			var sw = new StringWriter();
			ctx.Output = sw;
			using (var s = ass.GetManifestResourceStream("my_0_test_Sys_View.1_ru___resb")) {
				Assert.NotNull(s);
				Assert.AreEqual("hello Привет", new StreamReader(s).ReadToEnd());
			}
			view.Process(ctx);
			Console.WriteLine(sw.ToString());
			StringAssert.Contains("<h1>Привет</h1>", sw.ToString());
		}


		[Test]
		public void Can_Compile_With_Resources_FullName_Subview() {
			var vbxlfile = Path.Combine(Tmpdir, "sys\\my\\test.vbxl");
			var vbxlfile2 = Path.Combine(Tmpdir, "sys\\my\\test2.vbxl");
			var resfile = Path.Combine(Tmpdir, "sys\\my\\test.1_ru___resb");
			Directory.CreateDirectory(Tmpdir + "\\sys\\my");
			File.WriteAllText(vbxlfile, @"
h1:hello@
sub test2
");
			File.WriteAllText(vbxlfile2, "h2:@subview");
			File.WriteAllText(resfile, "hello Привет");
			var path = compiler.Compile(QViewCompileType.Full);
			var ass = Assembly.Load(File.ReadAllBytes(path));
			var factory = container.Get<IMvcFactory>();
			factory.Register(ass);
			IQView view = null;
			Assert.NotNull(view = factory.GetView("my/test"));
			var ctx = new QViewContext();
			ctx.Name = "/my/test";
			ctx.Factory = factory;
			var sw = new StringWriter();
			ctx.Output = sw;
			view.Process(ctx);
			Console.WriteLine(sw.ToString());
			StringAssert.Contains("<h1>Привет</h1><h2>subview</h2>", sw.ToString());
		}


		[Test]
		public void Can_Compile_With_Resources_FullName_Subview_By_Action_AndLayout() {
			var vbxlfile = Path.Combine(Tmpdir, "sys\\my\\test.vbxl");
			var vbxlfile2 = Path.Combine(Tmpdir, "sys\\my\\test2.vbxl");
			var vbxlfile3 = Path.Combine(Tmpdir, "sys\\test3.vbxl");
			var vbxlfile4 = Path.Combine(Tmpdir, "sys\\test4.vbxl");
			var layoutfile = Path.Combine(Tmpdir, "sys\\layouts\\test.vbxl");
			var resfile = Path.Combine(Tmpdir, "sys\\my\\test.1_ru___resb");
			Directory.CreateDirectory(Tmpdir + "\\sys\\my");
			Directory.CreateDirectory(Tmpdir + "\\sys\\layouts");
			File.WriteAllText(vbxlfile, @"
h1:hello@
sub test2
sub '/test3'
sub '../test4'
");
			File.WriteAllText(vbxlfile2, "h2:ViewData");
			File.WriteAllText(vbxlfile3, "p:3");
			File.WriteAllText(vbxlfile4, "p:4");
			File.WriteAllText(resfile, "hello Привет");
			File.WriteAllText(layoutfile, @"
p : @before
mainrender
p : @after
			");
			var path = compiler.Compile(QViewCompileType.Full);
			var ass = Assembly.Load(File.ReadAllBytes(path));

			var factory = container.Get<IMvcFactory>();
			factory.Register(typeof (MyTest));
			factory.Register(ass);

			var h = factory.CreateHandler();
			var ctx = factory.CreateContext("http://localhost/app/my/test.qview.qweb?layout=test");
			var sw = new StringWriter();
			ctx.Output = sw;
			h.ProcessRequest(ctx);

			Console.WriteLine(sw.ToString());
			StringAssert.Contains("<p>before</p><h1>Привет</h1><h2>45</h2><p>3</p><p>4</p><p>after</p>", sw.ToString());
		}


		[Test]
		public void Can_compile_simplest_view() {
			var path = compiler.Compile(QViewCompileType.Full, directSource: @"
using Qorpent.Mvc
h1 : @hello
");
			var ass = Assembly.Load(File.ReadAllBytes(path));
			var factory = Application.Current.MvcFactory;
			factory.Register(ass);
			Assert.NotNull(factory.GetView("directsource"));
		}

		[Test]
		public void Monitor_used() {
			var factory = app.MvcFactory;
			compiler.Compile(QViewCompileType.Full, directSource: @"
using Qorpent.Mvc
h1 : @hello
");
			Thread.Sleep(500);
			Assert.NotNull(factory.GetView("directsource"));
		}
	}
}