using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp{
	[TestFixture]
	public class PatchClassSupport : CompileTestBase{
		[Test]
		public void PatchCompiledButNotIncludedIntoLibPkg(){
			var code = "patch";
			var result = Compile(code);
			Assert.AreEqual(1, result.Working.Count);
			Assert.True(result.Working[0].Is(BSharpClassAttributes.Patch));
			Assert.AreEqual(0,result.Get(BSharpContextDataType.LibPkg).Count());
		}

		[Test]
		public void PatchWithoutTargetCauseError()
		{
			var code = "patch";
			var result = Compile(code);
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual(1, result.Errors.Count);
			Assert.AreEqual(BSharpErrorType.PatchUndefinedTarget,result.Errors[0].Type);
			
		}

		[TestCase(BSharpSyntax.PatchCreateBehaviorCreate,BSharpPatchBehavior.CreateOnNew)]
		[TestCase(BSharpSyntax.PatchCreateBehaviorError, BSharpPatchBehavior.ErrorOnNew)]
		[TestCase(BSharpSyntax.PatchCreateBehaviorNone, BSharpPatchBehavior.NoneOnNew)]
		[TestCase("", BSharpPatchBehavior.ErrorOnNew)]
		[TestCase("zzzz",BSharpPatchBehavior.Invalid)]
		public void InvalidBehaviorCauseError(string value, BSharpPatchBehavior patchtype)
		{
			var code = "patch for=x new='"+value+"'";
			var result = Compile(code);
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual(patchtype,result.Working[0].PatchBehavior);
			if (patchtype == BSharpPatchBehavior.Invalid){
				Assert.AreEqual(1, result.Errors.Count);
				Assert.AreEqual(BSharpErrorType.PatchError, result.Errors[0].Type);
			}

			
			

		}

		[Test]
		public void SimplePatch(){
			var code = @"
class A
	x a name1 shortname=n
patch for=^A:
	x a name2 idx=1
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name2' shortname='n' idx='1' />
</class>", str);
		}


		[Test]
		public void PatchWithInternals()
		{
			var code = @"
class A
	x a
patch for=^A new=create:
	x a name2 
		val 1
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name2'>
    <val code='1' />
  </x>
</class>", str);
		}


		[Test]
		public void HierarchicalPatch()
		{
			var code = @"
class A
	x a name1 shortname=n
		y b 
patch for=^A:
	x a name2 idx=1
		y b name2
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name2' shortname='n' idx='1'>
    <y code='b' name='name2' />
  </x>
</class>", str);
		}

		[Test]
		public void HierarchicalMove()
		{
			var code = @"
class A
	x a name1 shortname=n
		y b 
	x b
patch for=^A:
	x b
		y b
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name1' shortname='n' />
  <x code='b'>
    <y code='b' />
  </x>
</class>", str);
		}

		[Test]
		public void MultiplePatchWithPriority()
		{
			var code = @"
class A
	x a name1 shortname=n
patch for=^A priority=1000:
	x a idx=3
patch for=^A priority=100:
	x a name2 idx=2
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name2' shortname='n' idx='3' />
</class>", str);
		}

		[Test]
		public void ErrorOnNonExistedByDefault()
		{
			var code = @"
class A
	x a name1 shortname=n
patch for=^A 
	x a idx=3
	x b idx=4
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name1' shortname='n' />
</class>", str); //not changed!!!
			Assert.True(Compile(code).Errors.Any(_=>_.Type==BSharpErrorType.PatchError));
		}


		[Test]
		public void CanIgnoreNotExistedInNoneMode()
		{
			var code = @"
class A
	x a name1 shortname=n
patch for=^A new=none
	x a idx=3
	x b idx=4
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name1' shortname='n' idx='3' />
</class>", str); //not changed!!!
			Assert.False(Compile(code).Errors.Any(_ => _.Type == BSharpErrorType.PatchError));
		}


		[Test]
		public void CanCreateNotExistedInCreateMode()
		{
			var code = @"
class A
	x a name1 shortname=n
patch for=^A new=create
	x a idx=3
	x b idx=4
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name1' shortname='n' idx='3' />
  <x code='b' idx='4' />
</class>", str); //not changed!!!
			Assert.False(Compile(code).Errors.Any(_ => _.Type == BSharpErrorType.PatchError));
		}


	}
}