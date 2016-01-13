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
	    public class SubClass {
            public SubClass()
            {
		        Extensions=  new Dictionary<string, object>();
		    }
            public int Id { get; set; }
            public IDictionary<string, object> Extensions { get; set; } 
	    }

        public enum MyEnum {
            None = 0,
            Val1 = 1,
            Val2 = 2
        }
	    public class ClassWithEnum {
	        public MyEnum TheEnum;
	    }

		public class ComdivClass{
		    public ComdivClass() {
		        Extensions=  new Dictionary<string, object>();
		        Extensions2=  new Dictionary<string, string>();
		    }
			public int A { get; set; }
			public string B { get; set; }
            public IDictionary<string, object> Extensions { get; set; } 
            public IDictionary<string, string> Extensions2 { get; set; } 
		}
		public class ComdivAction:ActionBase{
			[Bind]
			public ComdivClass Param { get; set; }
			[Bind(ParameterPrefix = "val.")]
			public ComdivClass Param2 { get; set; }

          

        }

	    public class ActionWithEnum : ActionBase {
	        [Bind] public ClassWithEnum MyArgs;
	    }


        [Test]
        public void CanBindEnums()
        {
            var mvccontext = new MvcContext("http://comdiv/my?theenum=Val1");
            var action = new ActionWithEnum();
            var binder = new DefaultActionBinder();
            binder.Bind(new ActionDescriptor(action), mvccontext);
            Assert.AreEqual(MyEnum.Val1, action.MyArgs.TheEnum);

        }

        [TestCase(@"{""a"":1,""b"":""2""}")]
        [TestCase(@"/a:1//b:2/")]
	    public void CanBindDictionaryFromJson(string def) {
            var mvccontext = new MvcContext(@"http://comdiv/my?Extensions2="+def);
            var action = new ComdivAction();
            var binder = new DefaultActionBinder();
            binder.Bind(new ActionDescriptor(action), mvccontext);
            Assert.AreEqual("1", action.Param.Extensions2["a"]);
            Assert.AreEqual("2", action.Param.Extensions2["b"]);

            mvccontext = new MvcContext(@"http://comdiv/my?Extensions=" + def);
            action = new ComdivAction();
            binder = new DefaultActionBinder();
            binder.Bind(new ActionDescriptor(action), mvccontext);
            Assert.AreEqual("1", action.Param.Extensions["a"].ToString());
            Assert.AreEqual("2", action.Param.Extensions["b"].ToString());
            
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
