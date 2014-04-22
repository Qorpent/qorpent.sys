using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class IncludeTest : CompileTestBase {
		[Test]
		public void CanInclude()
		{
			var code = @"
class A x=1
	test '${x}'
class B x=2
	include A
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(1, result.Compiled.Descendants("A").Count());
			Assert.AreEqual("1", result.Compiled.Descendants("test").First().Attr("code"));
		}

		[Test]
		public void NormalInclideAwaredInterpolation(){
			var code = @"
class point X=%{_x}%{_mes} Y=%{_y}%{_mes} coords='[%{_x}:%{_y}]' embed
class polyline _mes = px
	include point _x=2 _y=3 
	include point _x=10 _y=30
	include point _x=20 _y=-40
	include point _x=2 _y=3
";


			var result = Compile(code);
			var xml = result.Get("polyline").Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(xml);
			Assert.AreEqual(@"<class code='polyline' fullcode='polyline'>
  <point X='2px' Y='3px' coords='[2:3]' />
  <point X='10px' Y='30px' coords='[10:30]' />
  <point X='20px' Y='-40px' coords='[20:-40]' />
  <point X='2px' Y='3px' coords='[2:3]' />
</class>", xml);

		}
	[Test]
		public void EmbedIntoElementTest(){
			var code = @"
class c embed
	p : %{a}
class x
	include c 
		a = ""xxx""
";


		var result = Compile(code);
			var xml = result.Get("x").Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(xml);
			Assert.AreEqual(@"<class code='x' fullcode='x'>
  <c a='xxx'>
    <p>xxx</p>
  </c>
</class>", xml);

		}

	[Test]
	public void NoBugOnEmptyStringAttribute()
	{
		var code = @"
class c embed
	p : %{a}
class x
	include c 
		a = """"
";


		var result = Compile(code);
		var xml = result.Get("x").Compiled.ToString().Replace("\"", "'");
		Console.WriteLine(xml);
		Assert.AreEqual(@"<class code='x' fullcode='x'>
  <c>
    <p></p>
  </c>
</class>", xml);

	}
        [TestCase("+", "x", "10", "1")]
        [TestCase("~", "x", "10", "10")]
        [TestCase("+", "y", "1", "1")]
        [TestCase("~", "y", "1", "1")]
        public void CanIncludeWithNonLiteralAttrName(string op, string operand, string value, string expected) {
            var code = @"
class A x=1
class B x=2
	include A """ + op + operand + @"""=""" + value + @"""
";
            var result = Compile(code).Get("B");
            Console.WriteLine(result.Compiled);
            Assert.AreEqual(expected,result.Compiled.Element("A").Attr(operand));
        }




        [Test]
        public void CanIncludeWithElementRename() {
            var code = @"
class A x=1
class B x=2
	include A element=chelios
";
            var result = Compile(code).Get("B");
            Console.WriteLine(result.Compiled.ToString());
            Assert.AreEqual(1, result.Compiled.Descendants("chelios").Count());

        }


		[Test]
		[Ignore]
		public void CanIncludeSelectedAttributesAll() {
			var code = @"
class A a=1 b=2 c=3
	test '${x}' a=3 b=4 c=5
class B x=2
	include A
		for all -a -c
";
			var result = Compile(code).Get("B");
			var a = result.Compiled.Element("A");
			Assert.NotNull(a.Attribute("b"));
			Assert.Null(a.Attribute("a"));
			Assert.Null(a.Attribute("c"));
			var t = a.Element("test");
			Assert.NotNull(t.Attribute("b"));
			Assert.Null(t.Attribute("a"));
			Assert.Null(t.Attribute("c"));
			
		}

		[Test]
		[Ignore]
		public void CanIncludeSelectedAttributesRootOnly()
		{
			var code = @"
class A a=1 b=2 c=3
	test '${x}' a=3 b=4 c=5
class B x=2
	include A
		for root a=0 c=0
";
			var result = Compile(code).Get("B");
			var a = result.Compiled.Element("A");
			Assert.NotNull(a.Attribute("b"));
			Assert.Null(a.Attribute("a"));
			Assert.Null(a.Attribute("c"));
			var t = a.Element("test");
			Assert.NotNull(t.Attribute("b"));
			Assert.NotNull(t.Attribute("a"));
			Assert.NotNull(t.Attribute("c"));
		}

		[Test]
		[Ignore]
		public void CanIncludeSelectedAttributesBodyAll()
		{
			var code = @"
class A a=1 b=2 c=3
	test '${x}' a=3 b=4 c=5
		test2 2 a=3 b=4 c=5
class B x=2
	include A body
		for all a=0 c=0
";
			var result = Compile(code).Get("B");
			var a = result.Compiled.Element("test");
			Assert.NotNull(a.Attribute("b"));
			Assert.Null(a.Attribute("a"));
			Assert.Null(a.Attribute("c"));
			var t = a.Element("test2");
			Assert.NotNull(t.Attribute("b"));
			Assert.Null(t.Attribute("a"));
			Assert.Null(t.Attribute("c"));

		}

		[Test]
		[Ignore]
		public void CanIncludeSelectedAttributesBodyRootOnly()
		{
			var code = @"
class A a=1 b=2 c=3
	test '${x}' a=3 b=4 c=5
		test2 2 a=3 b=4 c=5
class B x=2
	include A body
		for root a=0 c=0
";
			var result = Compile(code).Get("B");
			var a = result.Compiled.Element("test");
			Assert.NotNull(a.Attribute("b"));
			Assert.Null(a.Attribute("a"));
			Assert.Null(a.Attribute("c"));
			var t = a.Element("test2");
			Assert.NotNull(t.Attribute("b"));
			Assert.NotNull(t.Attribute("a"));
			Assert.NotNull(t.Attribute("c"));
		}

		[Test]
		public void CanIncludeNoChild()
		{
			var code = @"
class A x=1
	test
class B x=2
	include A nochild
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(1, result.Compiled.Descendants("A").Count());
			Assert.AreEqual(0, result.Compiled.Descendants("test").Count());
		}

		[Test]
		public void CanIncludeNoChildAndBody()
		{
			var code = @"
class A x=1
	test 1
		test 2
class B x=2
	include A body nochild
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(1, result.Compiled.Descendants("test").Count());
			Assert.AreEqual(0, result.Compiled.Descendants("test2").Count());
		}

		[Test]
		public void CanIncludeBodyOnly()
		{
			var code = @"
class A x=1
	test '${x}'
class B x=2
	include A body
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(0, result.Compiled.Descendants("class").Count());
			Assert.AreEqual("1", result.Compiled.Descendants("test").First().Attr("code"));
		}


        [Test]
        public void CanResolveNoNameSpace() {
            var code = @"
namespace A
    class A 
namespace B
    class B
	    include A
";
            var result = Compile(code).Get("B");
            Console.WriteLine(result.Compiled);
            Assert.AreEqual(1, result.Compiled.Descendants("A.A").Count());
        }

        [Test]
        public void SelectClauseForBody()
        {
            var code = @"
class A x=1
	test a x=1 y=2
    test b x=3 y=4 z=3
class B x=2
	include A body
        select code=true x=true y=b
";
            var result = Compile(code).Get("B");
            Assert.AreEqual("a", result.Compiled.Descendants("test").ElementAt(0).Attr("code"));
            Assert.AreEqual("b", result.Compiled.Descendants("test").ElementAt(1).Attr("code"));
            var sec = result.Compiled.Descendants("test").ElementAt(1);
            Assert.AreEqual("4",sec.Attr("b"));
            Assert.Null(sec.Attribute("z"));

        }


        [Test]
        public void GroupByClauseForBody()
        {
            var code = @"
class A x=1
	test a x=1 y=5
	test a1 x=3 y=4
	test a4 x=3 y=3
	test a2 x=1 y=2
    test b x=1 y=1 z=3
class B x=2
	include A body
        groupby x
";
            var result = Compile(code).Get("B");
            Console.WriteLine(result.Compiled.ToString().Replace("\"","\"\""));
            Assert.AreEqual(@"<class code=""B"" x=""2"" fullcode=""B"">
  <group code=""1"">
    <test code=""a"" x=""1"" y=""5"" id=""A"" />
    <test code=""a2"" x=""1"" y=""2"" id=""A"" />
    <test code=""b"" x=""1"" y=""1"" z=""3"" id=""A"" />
  </group>
  <group code=""3"">
    <test code=""a1"" x=""3"" y=""4"" id=""A"" />
    <test code=""a4"" x=""3"" y=""3"" id=""A"" />
  </group>
</class>", result.Compiled.ToString());

        }





        [Test]
        public void CanFindByElementLocalName()
        {
            var code = @"
class A x=1
	x a x=1 y=5
	x a1 x=3 y=4
	y a4 x=3 y=3
	y a2 x=1 y=2
    x b x=1 y=1 z=3
class B x=2
	include A body
        where localname=x
";
            var result = Compile(code).Get("B");
            Console.WriteLine(result.Compiled.ToString().Replace("\"", "\"\""));
            Assert.AreEqual(@"<class code=""B"" x=""2"" fullcode=""B"">
  <x code=""a"" x=""1"" y=""5"" id=""A"" />
  <x code=""a1"" x=""3"" y=""4"" id=""A"" />
  <x code=""b"" x=""1"" y=""1"" z=""3"" id=""A"" />
</class>", result.Compiled.ToString());

        }


        [Test]
        public void CrossClassGroupBy()
        {
            var code = @"
class A x=1 prototype=p
	test a x=1 y=5
	test a1 x=3 y=4
class C x=1 prototype=p
	test a4 x=3 y=3
	test a2 x=1 y=2
    test b x=1 y=1 z=3
class B x=2
	include all p body
        select code=true y=true
        groupby x
        orderby y
";
            var result = Compile(code).Get("B");
            Console.WriteLine(result.Compiled.ToString().Replace("\"", "\"\""));
            Assert.AreEqual(@"<class code=""B"" x=""2"" fullcode=""B"">
  <group code=""1"">
    <test code=""b"" y=""1"" />
    <test code=""a2"" y=""2"" />
    <test code=""a"" y=""5"" />
  </group>
  <group code=""3"">
    <test code=""a4"" y=""3"" />
    <test code=""a1"" y=""4"" />
  </group>
</class>", result.Compiled.ToString());

        }

		[Test]
		public void IncludeAllWithWhere()
		{
			var code = @"
class A x=1 prototype=p
class B x=2 prototype=p
class C x=3 prototype=p
class D
	include all p
        where x>>=1
";
			var result = Compile(code).Get("D");
			Console.WriteLine(result.Compiled.ToString().Replace("\"", "\"\""));
			Assert.NotNull(result.Compiled.Element("B"));
			Assert.NotNull(result.Compiled.Element("C"));
			Assert.Null(result.Compiled.Element("A"));

		}

		[Test]
		public void MultiConditionInIncludeAll()
		{
			var code = @"
class A x=1 prototype=p
class B x=2 prototype=p
class C x=3 prototype=p
class D x=4 prototype=p
class E
	include all p
		where x=1
        where x>>=2
";
			var result = Compile(code).Get("E");
			Console.WriteLine(result.Compiled.ToString().Replace("\"", "\"\""));
			Assert.NotNull(result.Compiled.Element("A"));
			Assert.NotNull(result.Compiled.Element("C"));
			Assert.NotNull(result.Compiled.Element("D"));
			Assert.Null(result.Compiled.Element("B"));

		}

		[Test]
		public void LateInterpolationInIncludeAll()
		{
			var code = @"
class A prototype=p
	my = 1
	x = '%{other,my}'
class B other=2
	include all p
";
			var result = Compile(code).Get("B").Compiled.Element("A");
			Assert.AreEqual("2",result.Attr("x"));

		}

        [Test]
        public void AllIncludeClauses()
        {
            var code = @"
class A x=1
	test a x=1 y=5
	test a1 x=3 y=4
	test a4 x=3 y=3
	test a2 x=1 y=2
    test b x=1 y=1 z=3
class B x=2
	include A body
        select code=true y=b
        groupby x as=mygroup with=name
        orderby y
";
            var result = Compile(code).Get("B");
            Console.WriteLine(result.Compiled.ToString().Replace("\"", "\"\""));
            Assert.AreEqual(@"<class code=""B"" x=""2"" fullcode=""B"">
  <mygroup name=""1"">
    <test code=""b"" b=""1"" />
    <test code=""a2"" b=""2"" />
    <test code=""a"" b=""5"" />
  </mygroup>
  <mygroup name=""3"">
    <test code=""a4"" b=""3"" />
    <test code=""a1"" b=""4"" />
  </mygroup>
</class>",result.Compiled.ToString());
        }

		[Test]
		public void CanEmbed()
		{
			var code = @"
class A x=1 z=3
	test '${x}%{x}'
class B x=2
	include A body
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(1, result.Compiled.Descendants("test").Count());
			Assert.AreEqual("12", result.Compiled.Descendants("test").First().Attr("code"));
			Assert.AreEqual("3", result.Compiled.Descendants("test").First().Attr("z"));
		}

		[Test]
		public void CanEmbedWithParameter()
		{
			var code = @"
class A 
	test '%{x}'
class B x=2
	include A body x=1 y=2
	include A body x=2 
	include A body x=3 
	include A body x=4 
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(4, result.Compiled.Descendants("test").Count());
			CollectionAssert.AreEquivalent(new[]{"1","2","3","4"}, result.Compiled.Descendants("test").Select(_=>_.Attr("code")).ToArray());
			Assert.AreEqual("2", result.Compiled.Descendants("test").First().Attr("y"));
		}


		[Test]
		public void CanUseWhere()
		{
			var code = @"
class A
	item x=1
	item x=1 y=2
	item x=2
	item x=3
	item x=4
	
class B
	include A body
		where x>>=2
		where y!=NULL
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(3, result.Compiled.Descendants("item").Count());
			CollectionAssert.AreEquivalent(new[] { "1", "3", "4"}, result.Compiled.Descendants("item").Select(_ => _.Attr("x")).ToArray());
			Assert.AreEqual("2", result.Compiled.Descendants("item").First().Attr("y"));
		}




		[Test]
		public void CanIncludeRecursivelly()
		{
			var code = @"
class A x=1 abstract
	test '${x}'
class B x=2
	include A
	test '${x}'
class C x=3
	include B
	test '${x}'
";
			var result = Compile(code).Get("C");
			Assert.AreEqual(1, result.Compiled.Descendants("A").Count());
			Assert.AreEqual(1, result.Compiled.Descendants("B").Count());
			CollectionAssert.AreEquivalent(new[]{"1","2","3"},
				result.Compiled.Descendants("test").Select(_=>_.Attr("code"))
				);			
		}
	}
}