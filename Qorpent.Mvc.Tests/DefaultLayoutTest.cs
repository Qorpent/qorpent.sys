using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Qorpent.IoC;
using Qorpent.Mvc.QView;
using Qorpent.Mvc.UI;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Tests
{
	[TestFixture]
	public class DefaultLayoutTest
	{
		private IContainer container;
		private IMvcFactory factory;
		private IQViewContext ctx;
		private IQView view;
		private StringWriter output;
		private VD data;

		class VD {
			public int X;
			public int Y;
			public string PageTitle;
			public string PageBodyClass;
			public bool UseReq;
		}

		[QView("test/myview")]
		class MyView :QViewBase {
			[QViewBind]public bool usereq;
			[QViewBind] private int x;
			protected override void Render() {
				if(usereq) {
					Require("my.js");
				}
				write(x);
			}
			protected override string GetExternalResourceLocalUrl(string resource) {
				return resource;
			}
		}


		[QView("layouts/defaultheader")]
		class DefaultHeader : QViewBase
		{
			protected override void Render() {
				write("<!-- my header -->");
			}
		}

		[QView("layouts/defaultheaderend")]
		class DefaultHeaderEnd : QViewBase
		{
			protected override void Render()
			{
				write("<!-- my header end -->");
			}
		}

		[QView("layouts/defaultbeforecontent")]
		class DefaultBeforeContent: QViewBase
		{
			protected override void Render()
			{
				write("<!-- before -->");
			}
		}

		[QView("layouts/defaultaftercontent")]
		class DefaultAfterContent : QViewBase
		{
			protected override void Render()
			{
				write("<!-- after -->");
			}
		}


		[SetUp]
		public void Setup() {
			output = new StringWriter();
			container = ContainerFactory.CreateEmpty();
			container.Register(container.NewComponent<IMvcFactory,MvcFactory>());
			container.Register(container.NewComponent<IQViewBinder,QViewBinder>());
			factory = container.Get<IMvcFactory>();
			factory.Register(typeof (MyView));
			factory.Register(typeof (DefaultLayout));
			data = new VD { X = 2, Y = 3 };
			ctx = new QViewContext
				{
					Factory = factory,
					Master = "default", 
					Name = "test/myview", 
					ViewData = data,
					Output = output
				};
			
			ctx = ctx.GetNormalizedContext();
			view = factory.GetView(ctx.Name);
			factory.ClearCaches();

		}

		[Test]
		public void FreshProcess() {
			view.Process(ctx);
			Console.WriteLine(output);
			Assert.AreEqual("<!DOCTYPE html><html><head></head><body class='qorpent'>2</body></html>",
				 output.ToString());
		}

		[Test]
		public void RequireProcess() {
			data.UseReq = true;
			view.Process(ctx);
			Console.WriteLine(output);
			Assert.AreEqual("<!DOCTYPE html><html><head>\r\n<script type='text/javascript' src='my.js'></script>\r\n</head><body class='qorpent'>2</body></html>",
			 output.ToString());
		}

		[Test]
		public void TitleRender() {
			data.PageTitle = "mytitle";
			view.Process(ctx);
			Console.WriteLine(output);
			Assert.AreEqual("<!DOCTYPE html><html><head><title>mytitle</title></head><body class='qorpent'>2</body></html>",
				 output.ToString());
		}

		[Test]
		public void BodyClassRender()
		{
			data.PageBodyClass = "mycls";
			view.Process(ctx);
			Console.WriteLine(output);
			Assert.AreEqual("<!DOCTYPE html><html><head></head><body class='qorpent mycls'>2</body></html>",
				 output.ToString());
		}


		[Test]
		public void DefaultHeaderTest() {
			factory.Register(typeof (DefaultHeader));
			view.Process(ctx);
			Console.WriteLine(output);
			Assert.AreEqual("<!DOCTYPE html><html><head><!-- my header --></head><body class='qorpent'>2</body></html>",
				 output.ToString());
		}

		[Test]
		public void DefaultHeaderEndTest()
		{
			factory.Register(typeof(DefaultHeaderEnd));
			data.UseReq = true;
			view.Process(ctx);
			Console.WriteLine(output);
			Assert.AreEqual("<!DOCTYPE html><html><head>\r\n<script type='text/javascript' src='my.js'></script>\r\n<!-- my header end --></head><body class='qorpent'>2</body></html>",
				 output.ToString());
		}


		[Test]
		public void DefaultBeforeContentTest()
		{
			factory.Register(typeof(DefaultBeforeContent));
			view.Process(ctx);
			Console.WriteLine(output);
			Assert.AreEqual("<!DOCTYPE html><html><head></head><body class='qorpent'><!-- before -->2</body></html>",
				 output.ToString());
		}

		[Test]
		public void DefaultAfterContentTest()
		{
			factory.Register(typeof(DefaultAfterContent));
			view.Process(ctx);
			Console.WriteLine(output);
			Assert.AreEqual("<!DOCTYPE html><html><head></head><body class='qorpent'>2<!-- after --></body></html>",
				 output.ToString());
		}
	}
}
