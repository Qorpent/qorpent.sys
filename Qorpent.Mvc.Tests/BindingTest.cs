using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Tests
{
	[TestFixture]
	public class BindingTest
	{
		public class ComdivClass{
			public int A { get; set; }
			public string B { get; set; }
		}
		public class ComdivAction:ActionBase{
			[Bind]
			public ComdivClass Param { get; set; }
			[Bind(ParameterPrefix = "val.")]
			public ComdivClass Param2 { get; set; }

		}

		[Test]
		public void CanBindStructureComplex()
		{
			var mvccontext = new MvcContext("http://comdiv/my?a=1&b=hello&val.a=3&val.b=test");
			var action = new ComdivAction();
			var binder = new DefaultActionBinder();
			binder.Bind(new ActionDescriptor(action), mvccontext);
			Assert.AreEqual(1, action.Param.A);
			Assert.AreEqual("hello", action.Param.B);
			Assert.AreEqual(3, action.Param2.A);
			Assert.AreEqual("test", action.Param2.B);


		}

		[Test]
		public void CanBindStructureEmpty(){
			var mvccontext = new MvcContext("http://comdiv/my?a=1&b=hello");
			var action = new ComdivAction();
			var binder = new DefaultActionBinder();
			binder.Bind(new ActionDescriptor(action),mvccontext );
			Assert.AreEqual(1,action.Param.A);
			Assert.AreEqual("hello",action.Param.B);

		}

		[Test]
		public void CanBindStructureSpecial()
		{
			var mvccontext = new MvcContext("http://comdiv/my?val.a=1&val.b=hello");
			var action = new ComdivAction();
			var binder = new DefaultActionBinder();
			binder.Bind(new ActionDescriptor(action), mvccontext);
			Assert.AreEqual(1, action.Param2.A);
			Assert.AreEqual("hello", action.Param2.B);

		}
		
	}
}
