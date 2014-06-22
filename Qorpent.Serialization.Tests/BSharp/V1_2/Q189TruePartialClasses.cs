using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp.V1_2{
	[TestFixture]
	public class Q189TruePartialClasses{
		[Test]
		public void SimplestTest(){
			var ctx = BSharpCompiler.Compile(@"
class A partial x=1
class A partial y=2
");
			Assert.AreEqual(0,ctx.GetErrors().Count());
			var cls = ctx["A"];
			Assert.AreEqual("1",cls.Compiled.Attr("x"));
			Assert.AreEqual("2",cls.Compiled.Attr("y"));
		}

		[Test]
		public void SimplestTestWithFirstNotPartial()
		{
			var ctx = BSharpCompiler.Compile(@"
class A x=1
class A partial y=2
");
			Assert.AreEqual(0, ctx.GetErrors().Count());
			var cls = ctx["A"];
			Assert.AreEqual("1", cls.Compiled.Attr("x"));
			Assert.AreEqual("2", cls.Compiled.Attr("y"));
		}

		[Test]
		public void FlagsAreMerged()
		{
			var ctx = BSharpCompiler.Compile(@"
class A partial x=1 explicit
class A partial y=2 abstract
");
			var clses = ctx.Get(BSharpContextDataType.SrcPkg);
			Assert.AreEqual(1,clses.Count());
			var cls = clses.First();
			Assert.True(cls.Is(BSharpClassAttributes.Abstract));
			Assert.True(cls.Is(BSharpClassAttributes.ExplicitElements));

		}

		[Test]
		public void ImportsMerged()
		{
			var ctx = BSharpCompiler.Compile(@"
class X a=1
class Y b=2
class A partial x=1
	import X
class A partial y=2
	import Y
");
			var cls = ctx["A"];
			Console.WriteLine(cls.Compiled.ToString().Replace("\"","\"\""));
			Assert.AreEqual(@"<class code=""A"" name=""partial"" y=""2"" x=""1"" fullcode=""A"" a=""1"" b=""2"" />".Trim().LfOnly(), cls.Compiled.ToString().Trim().LfOnly());

		}

		[Test]
		public void PreventAttributeConflict()
		{
			var ctx = BSharpCompiler.Compile(@"
class A partial x=1
class A partial x=2
");
			Assert.AreEqual(1, ctx.GetErrors(ErrorLevel.Error).Count());
		}

		[Test]
		public void PreventElementAttributeConflict()
		{
			var ctx = BSharpCompiler.Compile(@"
class A partial
	x 1 a=1
class A partial
	x 1 a=2
");
			Assert.AreEqual(1, ctx.GetErrors(ErrorLevel.Error).Count());
		}

		[Test]
		public void PreventElementBodyConflict()
		{
			var ctx = BSharpCompiler.Compile(@"
class A partial
	x 1 a=1
		x
class A partial
	x 1
		y
");
			Assert.AreEqual(1, ctx.GetErrors(ErrorLevel.Error).Count());
		}

		[Test]
		public void NestedElementPartials()
		{
			var ctx = BSharpCompiler.Compile(@"
class A partial x=1
	x u a=1
		b
class A partial y=2
	x u b=2

");
			Assert.AreEqual(0, ctx.GetErrors().Count());
			var cls = ctx["A"];
			Assert.AreEqual("1", cls.Compiled.Attr("x"));
			Assert.AreEqual("2", cls.Compiled.Attr("y"));
			var xs = cls.Compiled.Elements("x");
			Assert.AreEqual(1,xs.Count());
			var x = xs.First();
			Assert.NotNull(x.Element("b"));
			Assert.AreEqual("u",x.GetCode());
			Assert.AreEqual("1",x.Attr("a"));
			Assert.AreEqual("2",x.Attr("b"));
		}
	}
}