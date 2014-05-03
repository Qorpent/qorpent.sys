using System.Linq;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class AdvancedIncludeTest : CompileTestBase {
		[Test]
		public void CanIncludeWithPrototype() {
			var code = @"
class A prototype=a
class B prototype=a
class C
	include all a
";
			var res = Compile(code).Get("C").Compiled;
			Assert.AreEqual(0, res.Elements("include").Count());
			Assert.AreEqual(1, res.Elements("A").Count());
			Assert.AreEqual(1, res.Elements("B").Count());
		}


		[Test]
		public void CanIncludeWithBaseClass()
		{
			var code = @"
class X abstract
class A prototype=a
	import X
class B prototype=a
	import X
class C
	include all X
";
			var res = Compile(code).Get("C").Compiled;
			Assert.AreEqual(0, res.Elements("include").Count());
			Assert.AreEqual(1, res.Elements("A").Count());
			Assert.AreEqual(1, res.Elements("B").Count());
		}
		[Test]
		public void CanIncludeBodyWithPrototype()
		{
			var code = @"
class A prototype=a
	test 1
class B prototype=a
	test 2
class C
	include all a body
";
			var res = Compile(code).Get("C").Compiled;
			Assert.AreEqual(0, res.Elements("include").Count());
			Assert.AreEqual(2, res.Elements("test").Count());
		}

		[Test]
		public void CanIncludeBodyWithBaseClass()
		{
			var code = @"
class X abstract
class A prototype=a
	import X
	test 1
class B prototype=a
	import X
	test 2
class C
	include all X body
";
			var res = Compile(code).Get("C").Compiled;
			Assert.AreEqual(0, res.Elements("include").Count());
			Assert.AreEqual(2, res.Elements("test").Count());
		}
	}
}