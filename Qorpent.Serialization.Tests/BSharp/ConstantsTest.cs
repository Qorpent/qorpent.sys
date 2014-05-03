using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp{
	[TestFixture]
	public class ConstantsTest:CompileTestBase{
		[Test]
		public void CanUseConstants(){
			var code = @"
const x=1 y=2
class A a=${x} b=${y}
";
			var a = Compile(code).Get("A");
			Assert.AreEqual("1",a.Compiled.Attr("a"));
			Assert.AreEqual("2",a.Compiled.Attr("b"));
		}
		[Test]
		public void ConstantInterpolationInElements(){
			var code = @"
const 
	valf=""${_v} ${_m}""
	
class A 
	length value=${valf} _v=100 _m=km
	speed value=${valf} _v=80 _m=km/h
";
			var a = Compile(code).Get("A");
			Assert.AreEqual("100 km",a.Compiled.Element("length").Attr("value"));
			Assert.AreEqual("80 km/h", a.Compiled.Element("speed").Attr("value"));
		}

		[Test]
		public void InterpolationInElementsNoConstants_ControlTest()
		{
			var code = @"
class A 
	length value=""${_v} ${_m}"" _v=100 _m=km
	speed value=""${_v} ${_m}"" _v=80 _m=km/h
";
			var a = Compile(code).Get("A");
			Assert.AreEqual("100 km", a.Compiled.Element("length").Attr("value"));
			Assert.AreEqual("80 km/h", a.Compiled.Element("speed").Attr("value"));
		}
		[Test]
		public void CanApplyToInternals(){
			var code = @"
const x=1 y=2
class A 
	internal a=${x} b=${y}
";
			var a = Compile(code).Get("A");
			Assert.AreEqual("1", a.Compiled.Element("internal").Attr("a"));
			Assert.AreEqual("2", a.Compiled.Element("internal").Attr("b"));
		}


		[Test]
		public void CannotProvideЯнв()
		{
			var code = @"
const _ЯНВ=11
class A a=""${_ЯНВ:_ЯНВ}""
";
			var a = Compile(code).Get("A");
			Assert.AreEqual("11", a.Compiled.Attr("a"));
		}


		[Test]
		public void GlobalsAreCatched()
		{
			var code = @"
const x=1 y=2
";
			var a = Compile(code);
			Assert.AreEqual("1",a.Compiler.Global["x"]);
			Assert.AreEqual("2",a.Compiler.Global["y"]);
		}

		[Test]
		public void GlobalsWithInterpolationsAreCatched()
		{
			var code = @"
const x=${a} y=2
";
			var a = Compile(code);
			Assert.AreEqual("${a}", a.Compiler.Global["x"]);
			Assert.AreEqual("2", a.Compiler.Global["y"]);
		}


		[Test]
		public void GlobalsWithConstInterpolationsAreInterpolated()
		{
			var code = @"
const x=${a}~{y} y=2
";
			var a = Compile(code);
			Assert.AreEqual("${a}2", a.Compiler.Global["x"]);
			Assert.AreEqual("2", a.Compiler.Global["y"]);
		}

		[Test]
		public void CanUseConstantsInInterpolations()
		{
			var code = @"
const x=1 y=2 z=${x}${y}${u} u=4
class A a=${x} b=${y} c=${z} u=5
";
			var a = Compile(code).Get("A");
			Assert.AreEqual("1", a.Compiled.Attr("a"));
			Assert.AreEqual("2", a.Compiled.Attr("b"));
			Assert.AreEqual("125", a.Compiled.Attr("c"));
		}

		[Test]
		public void CanUseConstantsEarlyInterpolations()
		{
			var code = @"
const x=1 y=2 z=${x}${y}~{u} u=4
class A a=${x} b=${y} c=${z} u=5
";
			var a = Compile(code).Get("A");
			Assert.AreEqual("1", a.Compiled.Attr("a"));
			Assert.AreEqual("2", a.Compiled.Attr("b"));
			Assert.AreEqual("124", a.Compiled.Attr("c"));
		}
	}
}