using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp.Runtime;

namespace Qorpent.Core.Tests.BSharp.Runtime {
	[TestFixture]
	public class BSharpGenericProviderTest : BSharpRuntimeTestBase
	{
		[Test]
		public void CanEnumerateNames() {
			var p = new BSharpGenericClassProvider();
			p.Set(CreateCls<A>());
			p.Set(CreateCls<B>());
			Assert.AreEqual(2,p.GetClassNames().Count());
			Assert.AreEqual(1,p.GetClassNames("*.A").Count());
			Assert.AreEqual(1,p.GetClassNames("*.B").Count());
			Assert.AreEqual(2,p.GetClassNames("my.*").Count());
			Assert.AreEqual(1,p.GetClassNames("my.?.B").Count());
			Assert.AreEqual(1,p.GetClassNames("my.*.B").Count());
			Assert.AreEqual(1,p.GetClassNames("?.*.B").Count());
			Assert.AreEqual(0,p.GetClassNames("?.B").Count());
			Assert.AreEqual(1,p.GetClassNames("*.B").Count());
			Assert.AreEqual(2,p.GetClassNames("?.?.?").Count());
		}

		[TestCase("a", "A.B.C", "A.B.C.a")]
		[TestCase("b", "A.B.C", "A.B.C.b")]
		[TestCase("c", "A.B.C", "A.B.C.c")]
		[TestCase("B.b", "A.B.C", "A.B.b")]
		[TestCase("B.a", "A.B.C", "A.B.a")]
		[TestCase("A.a", "A.B.C", "A.a")]
		[TestCase("ba", "A.B.C", "A.B.ba")]
		[TestCase("aa", "A.B.C", "A.aa")]
		[TestCase("B.c", "A.B.C", null)]
		[TestCase("A.b", "A.B.C", null)]
		public void CanResolveClass(string n, string ns, string en) {
			var p = new BSharpGenericClassProvider();
			string[] nss = new[] {"A", "B", "C"};
			string[] css = new[] {"a", "b", "c"};
			var currentns = "";
			for (var i = 0; i < 3; i++) {
				p.Set(new BSharpRuntimeClass{Name=css[i],Namespace = ""});
			}
			for (var i = 0; i < 3; i++) {
				if (currentns != "") currentns += ".";
				currentns += nss[i];
				for (var j = 0; j <= i; j++) {
					var cn = css[j];
					p.Set(new BSharpRuntimeClass { Name = cn, Namespace = currentns });
					p.Set(new BSharpRuntimeClass {Name = nss[i].ToLower() + cn, Namespace = currentns});
				}
				
			}


			Assert.AreEqual(en,p.Resolve(n,ns));
		}
	}
}