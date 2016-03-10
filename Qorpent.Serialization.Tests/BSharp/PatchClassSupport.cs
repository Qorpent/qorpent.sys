using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp{
	[TestFixture]
	public class PatchClassSupport : CompileTestBase{
		[Test]
		public void PatchCompiledButNotIncludedIntoLibPkg(){
			var code = "patch";
			var result = Compile(code);
			Assert.AreEqual(1, result.MetaClasses.Count);
			Assert.True(result.MetaClasses.Values.First().Is(BSharpClassAttributes.Patch));
			Assert.AreEqual(0,result.Get(BSharpContextDataType.LibPkg).Count());
		}

		[Test]
		public void PatchWithoutTargetCauseError()
		{
			var code = "patch";
			var result = Compile(code);
			Assert.AreEqual(1, result.MetaClasses.Count);
			Assert.AreEqual(1, result.Errors.Count);
			Assert.AreEqual(BSharpErrorType.PatchUndefinedTarget,result.Errors[0].Type);
			
		}

		[TestCase(BSharpSyntax.PatchCreateBehaviorCreate,BSharpPatchCreateBehavior.CreateOnNew)]
		[TestCase(BSharpSyntax.PatchCreateBehaviorError, BSharpPatchCreateBehavior.ErrorOnNew)]
		[TestCase(BSharpSyntax.PatchCreateBehaviorNone, BSharpPatchCreateBehavior.NoneOnNew)]
		[TestCase("", BSharpPatchCreateBehavior.ErrorOnNew)]
		[TestCase("zzzz",BSharpPatchCreateBehavior.Invalid)]
		public void InvalidBehaviorCauseError(string value, BSharpPatchCreateBehavior patchtype)
		{
			var code = "patch for=x new='"+value+"'";
			var result = Compile(code);
			Assert.AreEqual(1, result.MetaClasses.Count);
			Assert.AreEqual(patchtype, result.MetaClasses.Values.First().PatchCreateBehavior);
			if (patchtype == BSharpPatchCreateBehavior.Invalid){
				Assert.AreEqual(1, result.Errors.Count);
				Assert.AreEqual(BSharpErrorType.PatchError, result.Errors[0].Type);
			}

			
			

		}


	    [Test]
	    public void PatchWithNameMask() {
            var code = @"
class t1 a=4
class t2 a=5
class t3 a=6
patch for='name~^t[12]' x='${a}1' before
";
            var result = Compile(code);
            Assert.AreEqual("41",result.Get("t1").Compiled.Attr("x"));
            Assert.AreEqual("51",result.Get("t2").Compiled.Attr("x"));
            Assert.AreEqual("",result.Get("t3").Compiled.Attr("x"));
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
</class>".Trim().LfOnly(), str.Trim().LfOnly());
		}

		[Test]
		public void SimpleBeforePatch()
		{
			var code = @"
class A x=1
patch for=^A y=${x} before
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' x='1' y='1' fullcode='A' />".Trim().LfOnly(), str.Trim().LfOnly());
		}

		


		[Test]
		public void PatchWithInterpolations()
		{
			var code = @"
class A
patch for=^A x=1 y=${x}:
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A' x='1' y='1' />".Trim().LfOnly(), str.Trim().LfOnly());
		}


		[Test]
		public void MatchNamePatch()
		{
			var code = @"
class A
	x a 
	y a 
patch for=^A elementname=match: 
	x a name1
	y a name2
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name1' />
  <y code='a' name='name2' />
</class>".Trim().LfOnly(), str.Trim().LfOnly());
		}


		[Test]
		public void InternalElementsCreation()
		{
			var code = @"
class controller
controller notifications
	service repomanager level=local
	item notifications action=GetMainEvents
patch for=notifications new=create
	service refresh_notifincations target=notifications auto=1 persistentCode=zdev3-notifications-autorefresh
		subscribe UPDATE_NOTIFICATIONS
	item notificationsquery type=LogQuery
		parameters NotAcceptedOnly=true
	item notifications args=notificationsquery
";
			var ctx = Compile(code);
			var result = ctx.Get("notifications");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(0,ctx.GetErrors(ErrorLevel.Error).Count());

			Assert.AreEqual(@"<controller code='notifications' fullcode='notifications'>
  <service code='repomanager' level='local' />
  <item code='notifications' action='GetMainEvents' args='notificationsquery' />
  <service code='refresh_notifincations' target='notifications' auto='1' persistentCode='zdev3-notifications-autorefresh'>
    <subscribe code='UPDATE_NOTIFICATIONS' />
  </service>
  <item code='notificationsquery' type='LogQuery'>
    <parameters NotAcceptedOnly='true' />
  </item>
</controller>".Trim().LfOnly().Length, str.Trim().LfOnly().Length);
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
</class>".Trim().LfOnly(), str.Trim().LfOnly());
		}

		[Test]
		public void PlainToTreePatch(){
			var code = @"
class A 
	x a
		y b 
patch for=^A plain:
	x a name1
	y b name2
";
			var result = Compile(code).Get("A");
			var str = result.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(str);
			Assert.AreEqual(@"<class code='A' fullcode='A'>
  <x code='a' name='name1'>
    <y code='b' name='name2' />
  </x>
</class>".Trim().LfOnly(), str.Trim().LfOnly());
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
</class>".Trim().LfOnly(), str.Trim().LfOnly());
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
</class>".Trim().LfOnly(), str.Trim().LfOnly());
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
</class>".Trim().LfOnly(), str.Trim().LfOnly());
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
</class>".Trim().LfOnly(), str.Trim().LfOnly()); //not changed!!!
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
</class>".Trim().LfOnly(), str.Trim().LfOnly()); //not changed!!!
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
</class>".Trim().LfOnly(), str.Trim().LfOnly()); //not changed!!!
			Assert.False(Compile(code).Errors.Any(_ => _.Type == BSharpErrorType.PatchError));
		}


	}
}