using System.Linq;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class ConditionalInternals:CompileTestBase {
		[Test]
		public void NonConditional() {
			var code = @"
class A
	test 1
	test 2
	test 3
";
			var result = Compile(code).Get("A");
			Assert.AreEqual(3,result.Compiled.Elements().Count());
		}

		[Test]
		public void OverridenConditional()
		{
			var code = @"

class A
	test 1 if='${x}'
	test 2 if='!${x}'
	test 3 if='${x} | z'
A B x=y y=1
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(2, result.Compiled.Elements().Count());

			code = @"

class A
	test 1 if='${x}'
	test 2 if='!${x} & !z'
	test 3 if='${x} | z'
A B x=y z=1
";
			result = Compile(code).Get("B");
			Assert.AreEqual(1, result.Compiled.Elements().Count());
		}

		[Test]
		public void SimpleConditional()
		{
			var code = @"
class A x=1
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			var result = Compile(code).Get("A");
			Assert.AreEqual(2, result.Compiled.Elements().Count());
			code = @"
class A 
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			result = Compile(code).Get("A");
			Assert.AreEqual(1, result.Compiled.Elements().Count());

			code = @"
class A y=1
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			result = Compile(code).Get("A");
			Assert.AreEqual(2, result.Compiled.Elements().Count());
		}

		[Test]
		public void AnyLevelConditional() {
			var code = @"
class A y=1
	test 1 
		testl if='x'
	test 2 
		testl if='y'
	test 3
		testl
";
			var result = Compile(code).Get("A");
			Assert.AreEqual(2, result.Compiled.Descendants("testl").Count());
		}
	}
}