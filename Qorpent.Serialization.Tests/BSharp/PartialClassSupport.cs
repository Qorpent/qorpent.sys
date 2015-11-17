using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class PartialClassSupport : CompileTestBase {
		[Test]
		public void CanResolvePartialAbstractClassesWithDifferentDirs() {
			const string code = @"
class r ""test"" abstract partial _line=1 _file=file _dir=boo a=1
class r ""test"" abstract partial _line=2 _file=elif _dir=oob b=2
class X
	import r
";
			var r = Compile(code).Get("X");
			Assert.NotNull(r);
			Console.WriteLine(r.Compiled.ToString());
		}

	   

		[Test]
		public void CanDefineClassFromOverride() {
			var code = @"
~class A
";
			var result = Compile(code).Get("A");
			Assert.NotNull(result);
		}

		[Test]
		public void CanDefineClassFromExtension()
		{
			var code = @"
+class A
";
			var result = Compile(code).Get("A");
			Assert.NotNull(result);
		}


        [Test]
        public void CanMakeClassAbstract()
        {
            var code = @"
class A
class B
~class A abstract=true
";
            Assert.NotNull(Compile(code).Get("B"));
            var a = Compile(code).Get("A");
            Assert.Null( a);
        }

        [Test]
        public void CanInterpolateWithExtensions()
        {
            var code = @"
class A x=2
+class A
    test y=${x}
";
            var a = Compile(code).Get("A");
            Assert.NotNull(a);
            Assert.AreEqual("2",a.Compiled.Descendants("test").First().Attr("y"));
        }



		[Test]
		public void ExtendWithElementValues()
		{
			var code = @"
class A abstract
	element el
	el 1
		row 1
A B
	+el 1
		row 2 : 'test'	
	
";
			var a = Compile(code).Get("B").Compiled.ToString().Replace("\"","'");
			Console.WriteLine(a);
			Assert.AreEqual(@"<A code='B' fullcode='B'>
  <el code='1'>
    <row code='1' />
    <row code='2'>test</row>
  </el>
</A>", a);
		}

		[Test]
		public void OverrideWithElementValues()
		{
			var code = @"
class A abstract
	element el
	el 1
		row 1
A B
	~el 1
		row 2 : 'test'	
	
";
			var a = Compile(code).Get("B").Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(a);
			Assert.AreEqual(@"<A code='B' fullcode='B'>
  <el code='1'>
    <row code='2'>test</row>
  </el>
</A>", a);
		}


		[Test]
		public void CanOverrideAttribute()
		{
			var code = @"
class A x=1 y=1
~class A x=2
";
			
			var result = Compile(code);
			Assert.AreEqual(1, result.Overrides.Count);
			var th = result.Get("A");
			Assert.AreEqual("2",th.Compiled.Attr("x"));
		}

		[Test]
		public void CanExtendAttribute()
		{
			var code = @"
class A x=1 y=1
+class A x=2 z=2
";
			var result = Compile(code).Get("A");
			Assert.AreEqual("2", result.Compiled.Attr("z"));
			Assert.AreEqual("1", result.Compiled.Attr("x"));
		}

		[Test]
		public void CanOverrideValidOrder()
		{
			var code = @"
class A x=1 y=1
~class A x=3 priority=20
~class A x=2 priority=10
";
			var result = Compile(code).Get("A");
			Assert.AreEqual("3", result.Compiled.Attr("x"));
		}

		[Test]
		public void CanExtendValidOrder()
		{
			var code = @"
class A x=1 y=1
+class A z=3 u=4 priority=20
+class A z=4 w=2 priority=10
";
			var result = Compile(code).Get("A");
			Assert.AreEqual("4", result.Compiled.Attr("u"));
			Assert.AreEqual("4", result.Compiled.Attr("z"));
		}


		[Test]
		public void RemoveBeforeSupport()
		{
			var code = @"
class colset abstract
colset cs_a
	row A
	col A
	col B
colset cs_b
	row B
	col D
	col C
class base
	element out
	out X
		include cs_a body
base other
	+out X
		remove-before col
		include cs_b body
";
			var result = Compile(code).Get("other").Compiled.ToString().Replace("\"","'");
			Console.WriteLine(result);
			Assert.AreEqual(@"<base code='other' fullcode='other'>
  <out code='X'>
    <row code='A' id='cs_a' />
    <row code='B' id='cs_b' />
    <col code='D' id='cs_b' />
    <col code='C' id='cs_b' />
  </out>
</base>",result);
		}

        [Test]
        public void CanOverrideElement()
        {
            var code = @"
class A
    element item
    item X
~class A
    ~item X y=1
";
            var result = Compile(code).Get("A");
            Assert.AreEqual("1", result.Compiled.Element("item").Attr("y"));
        }


	    [Test]
	    public void MacroReplaceTest() {
	        var code = @"
class macroses abstract
    macro-replace flag 
        set-xml-name=param 
        set-type=bool 
        set-default='~{default,__xmlvalue:true}' 
        set-hidden='~{hidden:true}'
        set-xml-value=EMPTY
class reportbase
    flag y
class report
    import macroses
    import reportbase
    flag x
    flag y : false
    flag z ZZ hidden=false : false
";
	        var result = Compile(code).Get("report");
	        string res;
            Console.WriteLine(res= result.Compiled.ToString().Simplify(SimplifyOptions.SingleQuotes));
           Assert.AreEqual(@"<class code='report' fullcode='report'>
  <param code='x' type='bool' default='true' hidden='true'></param>
  <param code='y' type='bool' default='false' hidden='true'></param>
  <param code='z' name='ZZ' hidden='false' type='bool' default='false'></param>
</class>", res);
	    }

        [Test]
        public void CanExtendElement()
        {
            var code = @"
class A
    element item
    item X x=2
~class A
    +item X x=1 y=1
";
            var result = Compile(code).Get("A");
            Assert.AreEqual("1", result.Compiled.Element("item").Attr("y"));
            Assert.AreEqual("2", result.Compiled.Element("item").Attr("x"));
        }

		


		[Test]
		public void CanExtendAndOverrideValidOrder()
		{
			var code = @"
class A x=1 y=1
+class A z=3 u=4 w=3 r=2 priority=20
+class A z=4 w=2 priority=10
~class A z=2 priority=20
~class A z=1 u=3 priority=10
";
			var result = Compile(code);
			var th = result.Get("A");
			Assert.AreEqual(2,result.Overrides.Count);
			Assert.AreEqual(2,result.Extensions.Count);
			Assert.AreEqual("3", th.Compiled.Attr("u"));
			Assert.AreEqual("2", th.Compiled.Attr("r"));
			Assert.AreEqual("2", th.Compiled.Attr("w"));
			Assert.AreEqual("2", th.Compiled.Attr("z"));
		}


		[Test]
		public void Q186InvalidClassNameAndNamespaceResolution() {
			var code = @"class X.Y.A";
			var result = Compile(code);
			var cls = result.Get("X.Y.A");
			Assert.AreEqual("A",cls.Name);
			Assert.AreEqual("X.Y.A", cls.FullName);
			Assert.AreEqual("X.Y", cls.Namespace);
		}
	}
}