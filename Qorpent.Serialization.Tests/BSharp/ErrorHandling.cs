using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class ErrorHandling : CompileTestBase
	{
		[Test]
		public void DuplicateClassNames()
		{
			var code = @"
namespace X
	class B
class B
class A
class A x=1
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(1,errors.Count());
			var error = errors.FirstOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.DuplicateClassNames,error.Type);
			Assert.AreEqual("1",error.Class.Source.Attribute("x").Value);
		}

		[Test]
		public void NotResolvedImport()
		{
			var code = @"
class A
	import B
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(1, errors.Count());
			var error = errors.FirstOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.NotResolvedImport, error.Type);
			var i = error.Data as BSharpImport;
			Assert.NotNull(i);
		}

		[Test]
		public void OrphanClass()
		{
			var code = @"
no-class A
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(1, errors.Count());
			var error = errors.FirstOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.OrphanClass, error.Type);
		}

		[Test]
		public void OrphanImport()
		{
			var code = @"
no-class B
class A
	import B
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(1, errors.Count());
			var error = errors.FirstOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.OrphanImport, error.Type);
			var i = error.Data as BSharpImport;
			Assert.NotNull(i);
		}

		[Test]
		public void FakeInclude()
		{
			var code = @"
class A
	include '${x}'
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(1, errors.Count());
			var error = errors.FirstOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.FakeInclude, error.Type);
			var i = error.Xml;
			Assert.NotNull(i);
		}


		[Test]
		public void NotResolvedInclude()
		{
			var code = @"
class A
	include B
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(1, errors.Count());
			var error = errors.FirstOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.NotResolvedInclude, error.Type);
			var i = error.Xml;
			Assert.NotNull(i);
		}

		[Test]
		public void OrphanInclude()
		{
			var code = @"
test B
class A
	include B
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(2, errors.Count());
			var error = errors.LastOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.OrphanInclude, error.Type);
			var i = error.Xml;
			Assert.NotNull(i);
		}


		[Test]
		public void ClassCreatedFormOverride()
		{
			var code = @"
~class A
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(1, errors.Count());
			var error = errors.FirstOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.ClassCreatedFormOverride, error.Type);
		}

		[Test]
		public void ClassCreatedFormExtension()
		{
			var code = @"
+class A
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(1, errors.Count());
			var error = errors.FirstOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.ClassCreatedFormExtension, error.Type);
		}

		[Test]
		public void EmptyInclude()
		{
			var code = @"
class B
class A
	include B body
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(1, errors.Count());
			var error = errors.FirstOrDefault();
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.EmptyInclude, error.Type);
			var i = error.Xml;
			Assert.NotNull(i);
		}

		[Test]
		public void RecycleImport()
		{
			var code = @"
class B
	import A
class A
	import B
";
			var result = Compile(code);
			var errors = result.GetErrors();
			Assert.AreEqual(2, errors.Count());
			var error = errors.ElementAt(0);
			Assert.NotNull(error);
			Assert.AreEqual(BSharpErrorType.RecycleImport, error.Type);
			var i = error.Data as BSharpImport;
			Assert.NotNull(i);
			Assert.IsNotEmpty(error.ClassName);

			var error2 = errors.ElementAt(1);
			Assert.NotNull(error2);
			Assert.AreEqual(BSharpErrorType.RecycleImport, error2.Type);
			var i2 = error2.Data as BSharpImport;
			Assert.NotNull(i2);
			Assert.IsNotEmpty(error2.ClassName);

			Assert.AreNotEqual(error2.ClassName,error.ClassName);
			Assert.AreNotEqual(i,i2);
			
		}
	}
}