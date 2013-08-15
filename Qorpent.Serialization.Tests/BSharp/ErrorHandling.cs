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
	}
}