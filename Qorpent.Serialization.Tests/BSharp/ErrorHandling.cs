using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class ErrorHandling : CompileTestBase
	{
		[Test]
		public void CanSygnalAboutDoubleClasses() {
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
	}
}