using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp {

  
	[TestFixture]
	public class MergeContentTests : CompileTestBase {

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
base A
	item X y=1
base B
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
			var result = Compile(nooverride).Get("B").Compiled;
			Console.WriteLine(result);
			Assert.AreEqual(1, result.Descendants("body").Count());
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
			result = Compile(overridecode).Get("B").Compiled;
			var e = Compile(overridecode).Get("B").Error;
			if (null != e) {
				Console.WriteLine(e);
			}
			Console.WriteLine(result);
			Assert.AreEqual(0, result.Descendants("body").Count());
			Assert.AreEqual(1, result.Descendants("newbody").Count());
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