using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class ClassReference : CompileTestBase {
		[Test]
		public void SimpleTest() {
			var code = @"
class A
class B x=^A
";
			var result = Compile(code).Get("B").Compiled;
			Assert.AreEqual("A",result.Attr("x"));
		}



		[Test]
		public void NamespaceTest()
		{
			var code = @"
namespace X
	class A
	class B x=^A
";
			var result = Compile(code).Get("B").Compiled;
			Assert.AreEqual("X.A", result.Attr("x"));
		}

		[Test]
		public void CrossNamespaceTest()
		{
			var code = @"
namespace Y.Z
	class A
namespace X
	class B x=^A
";
			var result = Compile(code).Get("B").Compiled;
			Assert.AreEqual("Y.Z.A", result.Attr("x"));
		}
		[Test]
		public void CrossNamespace2Test()
		{
			var code = @"
namespace Y.Z
	class A
namespace X
	class B x=^Z.A
";
			var result = Compile(code).Get("B").Compiled;
			Assert.AreEqual("Y.Z.A", result.Attr("x"));
		}
	}
}