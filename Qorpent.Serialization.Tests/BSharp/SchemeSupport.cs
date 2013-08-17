using System.Linq;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class SchemeSupport : CompileTestBase {
		[Test]
		public void SchemeSupportForElement() {
			var code = @"
class myscheme scheme
	for row
		ensure default outercode=code
";
			var result = Compile(code).Get("myscheme");
			Assert.AreEqual(3,result.AllElements);
		}

		[Test]
		public void SchemeElementsAreRemovedFromNonSchemaClasses()
		{
			var code = @"
class myscheme scheme
	for row
		ensure default outercode=code
class A
	import myscheme
";
			var result = Compile(code).Get("A");
			Assert.AreEqual(0, result.Compiled.Descendants("for").Count());
		}


		[Test]
		public void CanTranslateAttributes() {
			var code = @"
class myscheme scheme abstract
	for row
		ensure default outercode=code

class A abstract
	import myscheme
		
class test 
	import A
	row X
		row Y
";
			var result = Compile(code);
			Assert.AreEqual(1,result.Working.Count);

		}
	}
}