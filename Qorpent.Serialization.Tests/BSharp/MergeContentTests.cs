﻿using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp {

  
	[TestFixture]
	public class MergeContentTests : CompileTestBase {

	    [Test]
	    public void LevelCodeElementMerging()
	    {
            var code = @"
class X
    element item leveledcodes=true
    item 1
        item 3
    item 2
        item 3
    item 3 y=2
class A
    import X
    +item 3 x=1
    +item '2/3' y=5
    ";
            var result = Compile(code).Get("A");
            Console.WriteLine(result.Compiled.ToString());
            Assert.NotNull(result.Compiled.Elements("item").ElementAt(1));
            Assert.NotNull(result.Compiled.Elements("item").ElementAt(1).Elements("item").First());
            Assert.AreEqual("5",result.Compiled.Elements("item").ElementAt(1).Elements("item").First().Attr("y"));

            Assert.AreEqual("2",result.Compiled.Elements("item").ElementAt(2).Attr("y"));
        }


	    [Test]
	    public void TemplatedElementSupport() {
	        var code = @"
class A  _u=33
    element x template=true template-nested=true
		y c_%{.code}  basic = %{.code} _i=2 _i2=true l=%{_u}
			z %{_i} _if=_i
            z2 %{_i} _if=_i2
    x 23 a=%{basic} _i=1 _i2=false 
        w %{basic}%{basic}
        x 24
";
            var result = Compile(code).Get("A");
            Console.WriteLine(result.Compiled.ToString());
	        var e = result.Compiled.Element("y");
            Assert.NotNull(e);
            
            var z = e.Element("z");
            Assert.NotNull(z);

            Assert.AreEqual("c_23",e.GetCode());
            Assert.AreEqual("23",e.Attr("basic"));
            var w = e.Element("w");
            Assert.NotNull(w);
            Assert.AreEqual("2323", w.GetCode());
            Assert.AreEqual("23", e.Attr("a"));
           Assert.AreEqual("1",z.Attr("code"));

            Assert.Null(e.Element("z2"));
            Assert.Null(e.Element("x"));
            Assert.NotNull(e.Element("y"));

            Assert.AreEqual("33",e.Attr("l"));
        }

        [Test]
		public void CanMergeWithLocalAttributes(){
			var code = @"
class A
    element item
    item 1 y=${x}${x} x=1
		body ${y}
class B
	import A
    ~item 1 x=2
";
			var result = Compile(code).Get("B");
			Assert.AreEqual("22",result.Compiled.Element("item").Element("body").Attr("code"));
		}

        [Test]
        public void CanExtendNotCodedElements()
        {
            var code = @"
class A
    element item
    item
~class A
    +item x=1
";
            var result = Compile(code).Get("A").Compiled;
            Assert.AreEqual(1, result.Elements("item").Count());
            Assert.AreEqual("1", result.Element("item").Attr("x"));

        }

        [Test]
        
        public void NestedElementsSupport()
        {
            var code = @"
class A
    element item
    element item2
    item X
        item2 X
~class A
    +item2 X x=1
";
            var result = Compile(code).Get("A").Compiled;
            Console.WriteLine(result);
            Assert.AreEqual("1", result.Descendants("item2").First().Attr("x"));

        }

         [Test]
        public void CanExtendNotCodedElementsInExtension()
        {
            var code = @"
class A
    element item
    item
+class A
    +item x=1
";
            var result = Compile(code).Get("A").Compiled;
            Assert.AreEqual(1, result.Elements("item").Count());
            Assert.AreEqual("1", result.Element("item").Attr("x"));

        }

		[Test]
		public void CanRedefineElement() {
			const string noelementcode = @"
class base 
base A explicit
	item X y=1
base B explicit
	import A
	item X y=2
";
			var result = Compile(noelementcode).Get("B").Compiled;
			Console.WriteLine(result);
			Assert.AreEqual(2, result.Elements("item").Count());
			const string elementcode = @"
class base
	element item
base A
	item X y=1
base B
	import A
	item X y=2
";
			result = Compile(elementcode).Get("B").Compiled;
			Console.WriteLine(result);
			Assert.AreEqual(1, result.Elements("item").Count());
			Assert.AreEqual("2", result.Elements("item").First().Attr("y"));
		}

		[Test]
		public void CanOverrideElement()
		{
			const string noelementcode = @"
class base
base A
	item X x=2 y=1
		
base B
	import A
	itemset X y=2
";


			var th = Compile(noelementcode).Get("B");
			var result = th.Compiled;
			if (null != th.Error) {
				Console.WriteLine(th.Error);
				Assert.Fail(th.Error.Message);
			}
			Console.WriteLine(result);
			Assert.AreEqual("1", result.Element("item").Attr("y"));
			Assert.AreEqual(1, result.Elements("itemset").Count());
			const string elementcode = @"
class base
	element item
	element itemset override=item

base A
	item X x=2 y=1
		
base B
	import A
	itemset X y=2
";

			th = Compile(elementcode).Get("B");
			result = th.Compiled;
			if (null != th.Error)
			{
				Console.WriteLine(th.Error);
				Assert.Fail(th.Error.Message);
			}
			Console.WriteLine(result);
			Assert.AreEqual("2", result.Element("item").Attr("y"));
			Assert.AreEqual(0, result.Elements("itemset").Count());
		}
		[Test]
		[Repeat(1000)]
		[Category("NOTC")]
		public void Q229_Build_536_CanOverrideElementBodyStability(){
			try{
				CanOverrideElementBody();
			}
			catch (Exception e){
				Assert.Fail(e.ToString());
			}
		}

	    [Test]
	    public void AliasedElementsSupprt() {
            var result = Compile(@"
class base abstract
    element item
    element itema alias=item targetattr=x targetvalue=a
    element itemb alias=item targetattr=y targetvalue=b
class A
    import base
    item x
    itema y
    itemb z
").Get("A");
	        var str = result.Compiled.ToString().Simplify(SimplifyOptions.SingleQuotes);
            Console.WriteLine(str);
            Assert.AreEqual(@"<class code='A' fullcode='A'>
  <item code='x' />
  <item code='y' x='a' />
  <item code='z' y='b' />
</class>", str);
	    }

		[Test]
		public void CanOverrideElementBody()
		{
			const string nooverride = @"
class base
	element item
	element itemset override=item
base A
	item X x=2 y=1
		body
base B
	import A
	itemset X y=2
";
			var result = Compile(nooverride).Get("B");
			//Console.WriteLine(result);
			if (result.Compiled.Descendants("body").Count() != 1){
				foreach (var ae in result.AllElements	){
					Console.WriteLine(ae.Name+":"+ae.TargetName+":"+ae.Type+":"+ae.Implicit);
				}
			}
			Assert.AreEqual(1, result.Compiled.Descendants("body").Count());
			const string overridecode = @"
class base
	element item
	element itemset override=item

base A
	item X x=2 y=1
		body
base B
	import A
	itemset X y=2
		newbody
";
			result = Compile(overridecode).Get("B");
			var e = Compile(overridecode).Get("B").Error;
			if (null != e) {
				Console.WriteLine(e);
			}
			//Console.WriteLine(result);
			Assert.AreEqual(0, result.Compiled.Descendants("body").Count());
			Assert.AreEqual(1, result.Compiled.Descendants("newbody").Count());
		}

		[Test]
		public void CanUseDefaultMergeNames()
		{
			

			const string overridecode = @"
class base
	element item

base A
	item X x=2 y=1
		body
base B
	import A
	~item X y=2
		newbody
base C
	import B
	+item X y=3 z=23
		body2
";
			var th = Compile(overridecode).Get("C");
			var result =th.Compiled;
			var e = th.Error;
			if (null != e)
			{
				Console.WriteLine(e);
			}
			Console.WriteLine(result);
			Assert.AreEqual(0, result.Element("item").Descendants("body").Count());
			Assert.AreEqual(1, result.Element("item").Descendants("newbody").Count());
			Assert.AreEqual(1, result.Element("item").Descendants("body2").Count());
			Assert.AreEqual("23", result.Element("item").Attr("z"));
			Assert.AreEqual("2", result.Element("item").Attr("y"));
			Assert.AreEqual("2", result.Element("item").Attr("x"));
			
		}

		[Test]
		[Explicit("timingtest")]
		[Repeat(3000)]
		public void TimingForMergeOn3000Rebuilds()
		{


			const string overridecode = @"
class base
	element item

base A  x='${.x:1}'
	item X x=2 y=1
		body
base B  x='${.x:35}'
	import A
	~item X y=2
		newbody
base C x='${.x:34}'
	import B
	+item X y=3 z=23
		body2
	test '${x}'
";
			var th = Compile(overridecode).Get("C");
			

		}


		[Test]
		public void CanExtendElementBody()
		{
			const string noextend = @"
class base
	element item

base A
	item X x=2 y=1
		body
base B
	import A
	itemex X y=2 z=3
		body2
";
			var result = Compile(noextend).Get("B").Compiled;
			Console.WriteLine(result);
			Assert.AreEqual(1, result.Element("item").Elements("body").Count());
			Assert.AreEqual(0, result.Element("item").Elements("body2").Count());
			Assert.AreEqual("",result.Element("item").Attr("z"));
			Assert.AreEqual("1", result.Element("item").Attr("y"));
			const string extend = @"
class base
	element item
	element itemex extend=item

base A
	item X x=2 y=1
		body
base B
	import A
	itemex X y=2 z=3
		body2
";
			result = Compile(extend).Get("B").Compiled;
			var e = Compile(extend).Get("B").Error;
			if (null != e)
			{
				Console.WriteLine(e);
			}
			Console.WriteLine(result);
			Assert.AreEqual(1, result.Element("item").Elements("body").Count());
			Assert.AreEqual(1, result.Element("item").Elements("body2").Count());
			Assert.AreEqual("3", result.Element("item").Attr("z"));
			Assert.AreEqual("1", result.Element("item").Attr("y"));
		}
	}
}