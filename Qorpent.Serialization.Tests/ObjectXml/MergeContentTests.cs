using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.ObjectXml {
	[TestFixture]
	public class MergeContentTests : CompileTestBase {
		[Test]
		public void CanRedefineElement() {
			const string noelementcode = @"
class base
base A
	item X y=1
base B
	import A
	item X y=2
";
			var result = Compile(noelementcode).Get("B").Compiled;
			Console.WriteLine(result);
			Assert.AreEqual(2, result.Elements("item").Count());
			const string elementcode = @"
class base
	element item
base A
	item X y=1
base B
	import A
	item X y=2
";
			result = Compile(elementcode).Get("B").Compiled;
			Console.WriteLine(result);
			Assert.AreEqual(1, result.Elements("item").Count());
			Assert.AreEqual("2", result.Elements("item").First().Attr("y"));
		}
	}
}