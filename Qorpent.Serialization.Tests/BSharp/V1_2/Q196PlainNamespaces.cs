using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp.V1_2{
	/// <summary>
	/// #Q-196 Плоское определение пространств имен с отношением "перед классом", а не через вложение
	/// </summary>
	[TestFixture]
	public class Q196PlainNamespaces{
		[Test]
		public void AssignToSingleClass(){
			var ctx = BSharpCompiler.Compile(@"
namespace A
class B
");
			var cls = ctx["B"];
			Assert.AreEqual("A",cls.Namespace);
		}

		[Test]
		public void MultipleNamespaces(){
			var ctx = BSharpCompiler.Compile(@"
namespace A
class B
namespace C
class D
");
			var cls = ctx["B"];
			
			Assert.AreEqual("A", cls.Namespace);
			cls = ctx["D"];
			Assert.AreEqual("C", cls.Namespace);
		}

		[Test]
		public void MixedModel(){
			var ctx = BSharpCompiler.Compile(@"
namespace A
class B
namespace C
	class D
class E
namespace F
class G
");
			var cls = ctx["B"];

			Assert.AreEqual("A", cls.Namespace);
			cls = ctx["D"];
			Assert.AreEqual("C", cls.Namespace);
			cls = ctx["E"];
			Assert.AreEqual("", cls.Namespace);
			cls = ctx["G"];
			Assert.AreEqual("F", cls.Namespace);
		}
	}
}