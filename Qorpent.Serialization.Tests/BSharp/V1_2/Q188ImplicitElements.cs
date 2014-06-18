using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp.V1_2
{
	/// <summary>
	/// #Q-188 неявная самонастройка элементов
	/// </summary>
	public class Q188ImplicitElements
	{
		[TestCase("")]
		[TestCase("~")]
		[TestCase("+")]
		public void CanSetupElement(string prefix){
			var ctx = BSharpCompiler.Compile(string.Format(@"
class X
	{0}a 1
",prefix));
			var cls = ctx["X"];
			var a = cls.SelfElements.FirstOrDefault(_ => _.Name == "a");
			Assert.NotNull(a);
			Assert.AreEqual(BSharpElementType.Define,a.Type);
			Assert.True(a.Implicit);
		}

		public void CanDisableImplicitElements()
		{
			var ctx = BSharpCompiler.Compile(@"
class X explicit
	a 1
");
			var cls = ctx["X"];
			var a = cls.SelfElements.FirstOrDefault(_ => _.Name == "a");
			Assert.Null(a);
		}


		public void CanPreventExplicitInAllHierarchy()
		{
			var ctx = BSharpCompiler.Compile(@"
class X explicit
	a 1
");
			var cls = ctx["X"];
			var a = cls.SelfElements.FirstOrDefault(_ => _.Name == "a");
			Assert.Null(a);
		}

		

		[Test]
		public void NotCodedElementsNotCauseElementDefinition()
		{
			var ctx = BSharpCompiler.Compile(string.Format(@"
class X
	a
"));
			var cls = ctx["X"];
			var a = cls.SelfElements.FirstOrDefault(_ => _.Name == "a");
			Assert.Null(a);
		}

		[Test]
		public void AmbigousCodedElementsNotCauseElementDefinition()
		{
			var ctx = BSharpCompiler.Compile(string.Format(@"
class X
	a 1
	a 1
"));
			var cls = ctx["X"];
			var a = cls.SelfElements.FirstOrDefault(_ => _.Name == "a");
			Assert.Null(a);
		}

		[Test]
		public void ExplicitElementCanOverrideImplicit()
		{
			var ctx = BSharpCompiler.Compile(@"
class X
	element a extend=z 
	a 1
	b 2
	c 3
class Y
	import X
	element b override=y
");
			var cls = ctx["Y"];
			var a = cls.AllElements.FirstOrDefault(_ => _.Name == "a");
			var b = cls.AllElements.FirstOrDefault(_ => _.Name == "b");
			var c = cls.AllElements.FirstOrDefault(_ => _.Name == "c");
			Assert.NotNull(a);
			Assert.NotNull(b);
			Assert.NotNull(c);
			Assert.True(c.Implicit);
			Assert.False(a.Implicit);
			Assert.False(b.Implicit);
			Assert.AreEqual(BSharpElementType.Define, c.Type);
			Assert.AreEqual(BSharpElementType.Extension, a.Type);
			Assert.AreEqual(BSharpElementType.Override, b.Type);
			
		}
	}
}
