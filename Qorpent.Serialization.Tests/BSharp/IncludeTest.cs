using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class IncludeTest : CompileTestBase {
		[Test]
		public void CanInclude()
		{
			var code = @"
class A x=1
	test '${x}'
class B x=2
	include A
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(1, result.Compiled.Descendants("A").Count());
			Assert.AreEqual("1", result.Compiled.Descendants("test").First().Attr("code"));
		}

		[Test]
		public void CanIncludeBodyOnly()
		{
			var code = @"
class A x=1
	test '${x}'
class B x=2
	include A body
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(0, result.Compiled.Descendants("class").Count());
			Assert.AreEqual("1", result.Compiled.Descendants("test").First().Attr("code"));
		}

		[Test]
		public void CanEmbed()
		{
			var code = @"
class A x=1
	test '${x}%{x}'
class B x=2
	include A body
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(1, result.Compiled.Descendants("test").Count());
			Assert.AreEqual("12", result.Compiled.Descendants("test").First().Attr("code"));
		}

		[Test]
		public void CanEmbedWithParameter()
		{
			var code = @"
class A 
	test '%{x}'
class B x=2
	include A body x=1 
	include A body x=2 
	include A body x=3 
	include A body x=4 
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(4, result.Compiled.Descendants("test").Count());
			CollectionAssert.AreEquivalent(new[]{"1","2","3","4"}, result.Compiled.Descendants("test").Select(_=>_.Attr("code")).ToArray());
		}


		[Test]
		public void CanIncludeRecursivelly()
		{
			var code = @"
class A x=1 abstract
	test '${x}'
class B x=2
	include A
	test '${x}'
class C x=3
	include B
	test '${x}'
";
			var result = Compile(code).Get("C");
			Assert.AreEqual(1, result.Compiled.Descendants("A").Count());
			Assert.AreEqual(1, result.Compiled.Descendants("B").Count());
			CollectionAssert.AreEquivalent(new[]{"1","2","3"},
				result.Compiled.Descendants("test").Select(_=>_.Attr("code"))
				);			
		}
	}
}