using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp{
	[TestFixture]
	public class DefinitionsSupport:CompileTestBase{
		[Test]
		public void SimpleEvalNumberDefinition(){
			var result = Compile(@"
class Sum  abstract
	define sum : '${x} + ${y}' 
Sum x=3 x=4
");
			var cls = result.Working[0];
			Assert.AreEqual("7",cls.Compiled.Attr("sum"));
		}

		[Test]
		public void SimpleEvalNumberDefinitionBindToExisted()
		{
			var result = Compile(@"
class BaseSum sum = '${x} + ${y}'  abstract
BaseSum DynSum
	define sum
BaseSum A x=3 x=4
DynSum B x=3 x=4
");
			var cls = result.Get("A");
			Assert.AreEqual("3 + 4", cls.Compiled.Attr("sum"));
			var cls2 = result.Get("B");
			Assert.AreEqual("7", cls2.Compiled.Attr("sum"));
		}

		[Test]
		public void ElementDefinitionSupport()
		{
			var result = Compile(@"
class Sum  abstract
	define sum for=elements : (concat(${x} + ${y},'px')) 
Sum x=3 x=4
	item  x=2 y=3
	item  x=4 y=5
");
			var cls = result.Working[0];
			Assert.Null(cls.Compiled.Attribute("sum")); //donot generate on this
			Assert.AreEqual("5px",cls.Compiled.Elements("item").First().Attr("sum"));
			Assert.AreEqual("9px",cls.Compiled.Elements("item").Last().Attr("sum"));
		}
	}
}