using System;
using System.Linq;
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
			foreach (var cls in result.Working)
			{
				Console.WriteLine(cls.Name);
			}
			Assert.NotNull(result.Get("cls_280606649690895021695363653119788766"));
			Assert.NotNull(result.Get("cls_606214790536978727312106354395331582241"));
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

			foreach (var cls in result.Working)
			{
				Console.WriteLine(cls.Name);
			}
			Assert.AreEqual("cls_606214790536978727312106354395331582241", result.Working[0].Name);
			if (2 == result.Working.Count)
			{
				Console.WriteLine(result.Working[1].Name);
			}
			Assert.AreEqual(1, result.Working.Count);
			
			
			Assert.AreEqual(0,result.Errors.Count);
		}
	}
}