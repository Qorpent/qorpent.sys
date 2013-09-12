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