using System;
using System.Linq;
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
		public void CannotRefAbstractClasses()
		{
			var code = @"
class A abstract
class B x=^A
";
			var result = Compile(code).Get("B").Compiled;
			Assert.AreEqual("ABSTRACT::A", result.Attr("x"));
		}

		[Test]
		public void NoAmbiguityOnCoAbstractReferences()
		{
			var code = @"
namespace A
	namespace B
		namespace C
			class X abstract
namespace D
	namespace E
		namespace F
			class X
class Target refto = ^X 
class Target2 refto = ^F.X
";
			var result = Compile(code);
			Assert.AreEqual(0,result.GetErrors(ErrorLevel.Error).ToArray().Length);
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
		public void CanResolveReferenceInInclude()
		{
			var code = @"
namespace X
	class A
	class B u='z%{cls}' embed
	class C
		include B cls=^A
";
			var result = Compile(code).Get("C").Compiled;
			Console.WriteLine(result);
			Assert.AreEqual("zX.A", result.Element("X.B").Attr("u"));
		}

		[Test]
		public void ArraySyntaxSupport()
		{
			var code = @"
namespace X
	class A
	class B x=^A*
";
			var result = Compile(code).Get("B").Compiled;
			Assert.AreEqual("X.A*", result.Attr("x"));
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