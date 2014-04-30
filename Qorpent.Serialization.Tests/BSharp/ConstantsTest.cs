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
		public void CannotProvideßíâ()
		{
			var code = @"
const _ßÍÂ=11
class A a=""${_ßÍÂ:_ßÍÂ}""
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