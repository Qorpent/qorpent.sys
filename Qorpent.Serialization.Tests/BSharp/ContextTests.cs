using System.Linq;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class ContextTests : CompileTestBase {
		[Test]
		public void IsCorrectResolvingbyPrototypeWithOrOperation() {
			const string c = @"
class a prototype='x'
class b prototype='y'
class c prototype='z'
";
			var ctx = Compile(c);
			var cls = ctx.ResolveAll("x|y").ToArray();
			Assert.AreEqual(2, cls.Length);
			Assert.IsTrue(cls.Any(_ => _.Name == "a"));
			Assert.IsTrue(cls.Any(_ => _.Name == "b"));
		}
	}
}
