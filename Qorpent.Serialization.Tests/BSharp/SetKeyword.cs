using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class SetKeyword:CompileTestBase {
		[Test]
		public void SimpleTest() {
			var code = @"
class A
	set x=1
		item a
		item b
		item c x=2
";
			CommonCheckValues(code);
		}

		private void CommonCheckValues(string code) {
			var result = Compile(code).Get("A").Compiled;
			Assert.AreEqual(0, result.Elements("set").Count());
			Assert.AreEqual(3, result.Elements("item").Count());
			var a = result.Elements("item").ElementAt(0);
			var b = result.Elements("item").ElementAt(1);
			var c = result.Elements("item").ElementAt(2);
			Assert.AreEqual("1", a.Attr("x"));
			Assert.AreEqual("1", b.Attr("x"));
			Assert.AreEqual("2", c.Attr("x"));
		}

		[Test]
		public void ImportAndInterpolationTest()
		{
			var code = @"
class B
	set x='${y}'
		item a
		item b
		item c x=2
class A y=1
	import B
";
			CommonCheckValues(code);
		}

		[Test]
		public void IncludeAndInterpolationTest()
		{
			var code = @"
class B
	set x='~{y}'
		item a
		item b
		item c x=2
class A y=1
	include B body
";
			CommonCheckValues(code);
		}

		[Test]
		public void NestedSet()
		{
			var code = @"
class A
	set x='1'
		item a
		item b
		set x=2
			item c
";
			CommonCheckValues(code);
		}
	}
}