using System;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests.BSharp{
	[TestFixture]
	public class AnonymousClassSupport:CompileTestBase{
		[Test]
		public void CanDefineAnonimousClassesWithDistinctContent(){
			var code = @"
class
	a
class
	b
";
			var result = Compile(code);
			Assert.AreEqual(2,result.Working.Count);
			Assert.AreEqual("cls11345936565850091083", result.Working[0].Name);
			Assert.AreEqual("cls17333115036514700609", result.Working[1].Name);
		}


		[Test]
		public void RemovesDoubledAnonymousClasses()
		{
			var code = @"
class
	a
class
	a
";
			var result = Compile(code);
			Assert.AreEqual("cls11345936565850091083", result.Working[0].Name);
			if (2 == result.Working.Count)
			{
				Console.WriteLine(result.Working[1].Name);
			}
			Assert.AreEqual(1, result.Working.Count);
			
			
			Assert.AreEqual(0,result.Errors.Count);
		}
	}
}