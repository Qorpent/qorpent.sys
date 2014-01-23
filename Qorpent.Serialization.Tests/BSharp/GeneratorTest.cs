using NUnit.Framework;

namespace Qorpent.Serialization.Tests.BSharp
{
	[TestFixture]
	public class GeneratorTest : CompileTestBase
	{
		[Test]
		public void NotFailOnParse()
		{
			var result = Compile(@"
dataset X
generator Y
").Errors;
			Assert.AreEqual(0,result.Count);
		}

		[Test]
		public void CanGenerateWithOneDataSetSimple()
		{
			var code = @"
class X abstract
	y = '${_c}'
dataset dA
	item _c=1
	item _c=2
	item _c=3
generator gA
	dataset dA
	classCode = 'cls${_c}'
	className = 'cls_n${_c}'
	x = '${_c}${_c}'
";
			var result = Compile(code);
			Assert.AreEqual(3,result.Working.Count);
			Assert.NotNull(result.Get("cls1"));
			Assert.NotNull(result.Get("cls2"));
			Assert.NotNull(result.Get("cls3"));
		}
	}
}