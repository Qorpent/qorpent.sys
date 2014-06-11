using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

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
		public void CanSupplyInterpolableParameters()
		{
			var result = Compile(@"
class A year=1 abstract
generator
	import A
	dataset
		item u=~{year}~{year}
");
			var cls = result.Get(BSharpContextDataType.Working).First();
			Console.WriteLine(cls.Compiled);
			Assert.AreEqual("11", cls.Compiled.Attr("u"));
		}

		[Test]
		public void CanSupplyImportClause()
		{
			var result = Compile(@"
class A x=1 abstract
dataset cls
	item _cls = A
generator ${_cls}X
	dataset cls
	import ${_cls}
");
			var cls = result.Get("AX");
			Assert.AreEqual("1",cls.Compiled.Attr("x"));
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
generator 'cls${_c}' 'cls_n${_c}'
	dataset dA
	import X
	x = '${_c}${_c}'
";
			var result = Compile(code);
			Assert.AreEqual(3,result.Working.Count);
			Assert.NotNull(result.Get("cls1"));
			Assert.NotNull(result.Get("cls2"));
			Assert.NotNull(result.Get("cls3"));

			Assert.AreEqual("3",result.Get("cls3").Compiled.Attr("y"));
			Assert.AreEqual("33",result.Get("cls3").Compiled.Attr("x"));
		}
	}
}