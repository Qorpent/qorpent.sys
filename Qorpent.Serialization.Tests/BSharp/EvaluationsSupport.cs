using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp{
	[TestFixture]
	public class EvaluationsSupport:CompileTestBase{
		[Test]
		public void SimpleEvalNumberDefinition(){
			var result = Compile(@"
class Sum  abstract
	eval sum : '${x} + ${y}' 
Sum x=3 y=4
");
			var cls = result.Working[0];
			Assert.AreEqual("7",cls.Compiled.Attr("sum"));
		}

		[Test]
		public void SimpleEvalNumberDefinitionBindToExisted()
		{
			var result = Compile(@"
class BaseSum sum = '${x} + ${y}'  abstract
BaseSum DynSum abstract
	eval sum
BaseSum A x=3 y=4
DynSum B x=3 y=4
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
	eval sum for=elements : (concat(${x} + ${y},'px')) 
Sum x=3 x=4
	item  x=2 y=3
	item  x=4 y=5
");
			var cls = result.Working[0];
			Assert.Null(cls.Compiled.Attribute("sum")); //donot generate on this
			Assert.AreEqual("5px",cls.Compiled.Elements("item").First().Attr("sum"));
			Assert.AreEqual("9px",cls.Compiled.Elements("item").Last().Attr("sum"));
		}

		[Test]
		public void DeepInternalsSupport()
		{
			var result = Compile(@"
class base  abstract
	eval width for=.//x
base c
	p 
		x width=(concat(5+3,'px'))
");
			var cls = result.Working[0];
			Assert.AreEqual("8px", cls.Compiled.Element("p").Element("x").Attr("width"));
		}

		[Test]
		public void DeepInternalsSupportWithInclude()
		{
			var result = Compile(@"
class a embed
	x width=(concat(%{w}+3,'px'))
class base  abstract
	eval width for=.//x
base c
	include a w=5
");
			var cls = result.Working[0];
			Assert.AreEqual("8px", cls.Compiled.Element("a").Element("x").Attr("width"));
		}

		[Test]
		public void DeepInternalsSupportWithIncludeAtIncludeSide()
		{
			var result = Compile(@"
class a embed
	eval width for=.//x
	x width=(concat(%{w}+3,'px'))
class base  abstract
base c
	include a w=5
	include a w=6
");
			var cls = result.Working[0];
			Assert.AreEqual("8px", cls.Compiled.Elements("a").First().Element("x").Attr("width"));
			Assert.AreEqual("9px", cls.Compiled.Elements("a").Last().Element("x").Attr("width"));
		}

	    [Test]
	    public void SampleCountGapForIndex() {
            var result = Compile(@"
class x size=48
	eval size for=(./i[@code='placeholder']) : (../@size - sum(../i[@code!='placeholder']/@size))
	eval type for=(./i[not(@type)]) : ('string')
	i typecode size=2 
	i placeholder
	i unqmarker size=4 
");
	        var cls = result["x"].Compiled;
	        var f = cls.Elements("i").First(_ => _.Attr("code") == "placeholder");
            Assert.AreEqual("42",f.Attr("size"));
            Assert.AreEqual("string",f.Attr("type"));
	    }
	}
}