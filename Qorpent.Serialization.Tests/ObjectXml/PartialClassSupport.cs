using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.ObjectXml {
	[TestFixture]
	public class PartialClassSupport : CompileTestBase {
		[Test]
		public void CanDefineClassFromOverride() {
			var code = @"
~class A
";
			var result = Compile(code).Get("A");
			Assert.NotNull(result);
		}

		[Test]
		public void CanDefineClassFromExtension()
		{
			var code = @"
+class A
";
			var result = Compile(code).Get("A");
			Assert.NotNull(result);
		}

		[Test]
		public void CanOverrideAttribute()
		{
			var code = @"
class A x=1 y=1
~class A x=2
";
			
			var result = Compile(code);
			Assert.AreEqual(1, result.Overrides.Count);
			var th = result.Get("A");
			Assert.AreEqual("2",th.Compiled.Attr("x"));
		}

		[Test]
		public void CanExtendAttribute()
		{
			var code = @"
class A x=1 y=1
+class A x=2 z=2
";
			var result = Compile(code).Get("A");
			Assert.AreEqual("2", result.Compiled.Attr("z"));
			Assert.AreEqual("1", result.Compiled.Attr("x"));
		}

		[Test]
		public void CanOverrideValidOrder()
		{
			var code = @"
class A x=1 y=1
~class A x=3 priority=20
~class A x=2 priority=10
";
			var result = Compile(code).Get("A");
			Assert.AreEqual("3", result.Compiled.Attr("x"));
		}

		[Test]
		public void CanExtendValidOrder()
		{
			var code = @"
class A x=1 y=1
+class A z=3 u=4 priority=20
+class A z=4 w=2 priority=10
";
			var result = Compile(code).Get("A");
			Assert.AreEqual("4", result.Compiled.Attr("u"));
			Assert.AreEqual("4", result.Compiled.Attr("z"));
		}

		[Test]
		public void CanExtendAndOverrideValidOrder()
		{
			var code = @"
class A x=1 y=1
+class A z=3 u=4 w=3 r=2 priority=20
+class A z=4 w=2 priority=10
~class A z=2 priority=20
~class A z=1 u=3 priority=10
";
			var result = Compile(code);
			var th = result.Get("A");
			Assert.AreEqual(2,result.Overrides.Count);
			Assert.AreEqual(2,result.Extensions.Count);
			Assert.AreEqual("3", th.Compiled.Attr("u"));
			Assert.AreEqual("2", th.Compiled.Attr("r"));
			Assert.AreEqual("2", th.Compiled.Attr("w"));
			Assert.AreEqual("2", th.Compiled.Attr("z"));
		}
	}
}