using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
	[TestFixture]
	public class XmlInterpolationTests {
		private XmlInterpolation _xi;

		[SetUp]
		public void SetUp()
		{
			_xi = new XmlInterpolation();
		}

		/// <summary>
		/// Тест в котором один атрибут просто прошивается в другой в одном элементе
		/// </summary>
		[Test]
		public void SimleInterpolation() {
			var x = XElement.Parse("<a x='1' y='${x}${x}2'/>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("1",x.Attribute("x").Value);
			Assert.AreEqual("112", x.Attribute("y").Value);
		}

		/// <summary>
		/// Тест в котором один атрибут просто прошивается в другой в одном элементе,
		/// но при этом производится 2 преобразования
		/// </summary>
		[Test]
		public void IterativeSimleInterpolation()
		{
			var x = XElement.Parse("<a x='1' y='${x}${x}2' z='${y}${x}3'/>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("1", x.Attribute("x").Value);
			Assert.AreEqual("112", x.Attribute("y").Value);
			Assert.AreEqual("11213", x.Attribute("z").Value);
		}

	    [Test]
	    public void RepeatExtension() {
	        var x = new BxlParser().Parse(@"
e u=33
    x ${mycode} ${myname} z=${u} xi-repeat=data 
",options:BxlParserOptions.NoLexData|BxlParserOptions.ExtractSingle);
	        _xi = new XmlInterpolation {UseExtensions = true};
	        var ctx = new {data = new object[] {new {mycode = 2, myname = "Two"}, new {mycode = 3, myname = "Three"}}};
	        var cfg = new ConfigBase(ctx.ToDict());
	        x = _xi.Interpolate(x, cfg);
            Console.WriteLine(x.ToString().Replace("\"","'"));
            Assert.AreEqual(@"<e u='33'>
  <x code='2' id='2' name='Two' z='33' />
  <x code='3' id='3' name='Three' z='33' />
</e>".Simplify(SimplifyOptions.Full), x.ToString().Simplify(SimplifyOptions.Full));
	    }

        [Test]
        public void RepeatWithScopeExtension()
        {
            var x = new BxlParser().Parse(@"
e mycode=1 myname=2
    x ${x.mycode} ${x.myname} mc=${mycode} mn=${myname} xi-repeat=data xi-scope=x 
", options: BxlParserOptions.NoLexData | BxlParserOptions.ExtractSingle);
            _xi = new XmlInterpolation { UseExtensions = true };
            var ctx = new { data = new object[] { new { mycode = 2, myname = "Two" }, new { mycode = 3, myname = "Three" } } };
            var cfg = new ConfigBase(ctx.ToDict());
            x = _xi.Interpolate(x, cfg);
            Console.WriteLine(x.ToString().Replace("\"", "'"));
            Assert.AreEqual(@"<e mycode='1' myname='2'>
  <x code='2' id='2' name='Two' mc='1' mn='2' xi-scope='x' />
  <x code='3' id='3' name='Three' mc='1' mn='2' xi-scope='x' />
</e>".Simplify(SimplifyOptions.Full), x.ToString().Simplify(SimplifyOptions.Full));
        }

        [Test]
        public void RepeatWithScopeExtensionAndFunctionCall()
        {
            var x = new BxlParser().Parse(@"
e mycode=1 myname=aa
    x ${x.mycode} ${x.myname} mf=${upper(myname)} mc=${mycode} mn=${myname} xi-repeat=data xi-scope=x 
", options: BxlParserOptions.NoLexData | BxlParserOptions.ExtractSingle);
            _xi = new XmlInterpolation { UseExtensions = true };
            var ctx = new { data = new object[] { new { mycode = 2, myname = "Two" }, new { mycode = 3, myname = "Three" } } };
            var cfg = new ConfigBase(ctx.ToDict());
            cfg["upper"] = (Func<string, string>) (s => s.ToUpper());
            x = _xi.Interpolate(x, cfg);
            Console.WriteLine(x.ToString().Replace("\"", "'"));
            Assert.AreEqual(@"<e mycode='1' myname='aa'>
  <x code='2' id='2' name='Two' mf='AA' mc='1' mn='aa' xi-scope='x' />
  <x code='3' id='3' name='Three' mf='AA' mc='1' mn='aa' xi-scope='x' />
</e>".Simplify(SimplifyOptions.Full), x.ToString().Simplify(SimplifyOptions.Full));
        }

	    [Test]
	    public void BasicXiIfTest() {
            var x = new BxlParser().Parse(@"
root x=true
    item 1 x=false xi-if='a & x'
    item 2 xi-if='a & x'
    item 3 x=false xi-if='a | x'
    item 4 xi-if='a | x'
", options: BxlParserOptions.NoLexData | BxlParserOptions.ExtractSingle);
	        var res = new XmlInterpolation().Interpolate(new XElement(x), new {a = true});
            Console.WriteLine(res.ToString().Replace("\"", "'"));
            Assert.AreEqual(@"<root x='true'>
  <item code='2' id='2' />
  <item code='3' id='3' x='false' />
  <item code='4' id='4' />
</root>".Simplify(SimplifyOptions.Full), res.ToString().Simplify(SimplifyOptions.Full));
            var res2 = new XmlInterpolation().Interpolate(new XElement(x), new { a = false });
            Console.WriteLine(res2.ToString().Replace("\"", "'"));
            Assert.AreEqual(@"<root x='true'>
  <item code='4' id='4' />
</root>".Simplify(SimplifyOptions.Full), res2.ToString().Simplify(SimplifyOptions.Full));
            
	    }


        [Test]
        public void RepeatLocalSetExtension()
        {
            var x = new BxlParser().Parse(@"
xi-dataset x
    x x=${mycode} y=${myname} xi-repeat=data  
e u=33
    i ${x}${x} ${y}${y} z=${u} xi-repeat=$x 
", options: BxlParserOptions.NoLexData | BxlParserOptions.ExtractSingle);
            _xi = new XmlInterpolation { UseExtensions = true };
            var ctx = new { data = new object[] { new { mycode = 2, myname = "Two" }, new { mycode = 3, myname = "Three" } } };
            x = _xi.Interpolate(x, ctx);
            Console.WriteLine(x.ToString().Replace("\"", "'"));
            Assert.AreEqual(@"<root>
  <xi-dataset code='x' id='x'>
    <x x='2' y='Two' />
    <x x='3' y='Three' />
  </xi-dataset>
  <e u='33'>
    <i code='22' id='22' name='TwoTwo' z='33' />
    <i code='33' id='33' name='ThreeThree' z='33' />
  </e>
</root>".Simplify(SimplifyOptions.Full), x.ToString().Simplify(SimplifyOptions.Full));
        }

        [Test]
        public void RepeatExtensionWithBodyOption()
        {
            var x = new BxlParser().Parse(@"
e 
    x xi-repeat=data xi-body
        x-code ${mycode}
        x-name ${myname}   
", options: BxlParserOptions.NoLexData | BxlParserOptions.ExtractSingle);
            _xi = new XmlInterpolation { UseExtensions = true };
            var ctx = new { data = new object[] { new { mycode = 2, myname = "Two" }, new { mycode = 3, myname = "Three" } } };
            x = _xi.Interpolate(x, ctx);
            Console.WriteLine(x.ToString().Replace("\"", "'"));
            Assert.AreEqual(@"<e>
  <x-code code='2' id='2' />
  <x-name code='Two' id='Two' />
  <x-code code='3' id='3' />
  <x-name code='Three' id='Three' />
</e>".Simplify(SimplifyOptions.Full), x.ToString().Simplify(SimplifyOptions.Full));
        }

        [Test]
        public void RepeatExtensionWithIfOption()
        {
            var x = new BxlParser().Parse(@"
e b=true
    x  xi-repeat=data xi-body xi-where=include xi-if=!b
        x-code ${mycode} x=1 xi-if=mycode
        x-name ${myname} x=1 xi-if=myname
    x  xi-repeat=data xi-body xi-where=include xi-if=b
        x-code ${mycode} x=2 xi-if=mycode
        x-name ${myname} x=2 xi-if=myname
", options: BxlParserOptions.NoLexData | BxlParserOptions.ExtractSingle);
            _xi = new XmlInterpolation { UseExtensions = true };
            var ctx = new { data = new object[] {
                new { mycode = 2, myname = "Two", include=true }, 
                new { mycode = 3, myname = "Three", include=false },
                new { mycode = 0, myname = "Four", include=true },
                new { mycode = 5, myname = "", include=true }
            } };
            var cfg = new ConfigBase(ctx.ToDict());
            x = _xi.Interpolate(x, cfg);
            Console.WriteLine(x.ToString().Replace("\"", "'"));
            Assert.AreEqual(@"<e b='true'>
  <x-code code='2' id='2' x='2' />
  <x-name code='Two' id='Two' x='2' />
  <x-name code='Four' id='Four' x='2' />
  <x-code code='5' id='5' x='2' />
</e>".Simplify(SimplifyOptions.Full), x.ToString().Simplify(SimplifyOptions.Full));
        }

        [Test]
        public void RepeatExtensionWithComplexIfOption()
        {
            var x = new BxlParser().Parse(@"
e 
    was-excluded ${mycode} xi-repeat=data xi-where=!include
    not-complete ${mycode} ${myname} xi-repeat=data xi-where='!(mycode & myname)'
    more-than-2 ${mycode} ${myname} xi-repeat=data xi-where='mycode > 2'
", options: BxlParserOptions.NoLexData | BxlParserOptions.ExtractSingle);
            _xi = new XmlInterpolation { UseExtensions = true };
            var ctx = new
            {
                data = new object[] {
                new { mycode = 2, myname = "Two", include=true }, 
                new { mycode = 3, myname = "Three", include=false },
                new { mycode = 0, myname = "Four", include=true },
                new { mycode = 5, myname = "", include=false }
            }
            };
            var cfg = new ConfigBase(ctx.ToDict());
            x = _xi.Interpolate(x, cfg);
            Console.WriteLine(x.ToString().Replace("\"", "'"));
            Assert.AreEqual(@"<e>
  <was-excluded code='3' id='3' />
  <was-excluded code='5' id='5' />
  <not-complete code='0' id='0' name='Four' />
  <not-complete code='5' id='5' name='' />
  <more-than-2 code='3' id='3' name='Three' />
  <more-than-2 code='5' id='5' name='' />
</e>".Simplify(SimplifyOptions.Full), x.ToString().Simplify(SimplifyOptions.Full));
        }




		/// <summary>
		/// Тест в котором один атрибут просто прошивается в другой в одном элементе,
		/// но при этом производится 2 преобразования
		/// </summary>
		[Test]
		public void CanStopInterpolationDown()
		{
			var x = XElement.Parse("<a x='1'><b a='${x}' stopinterpolate='1'><c a='${x}'/></b></a>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("1", x.Descendants("b").First().Attribute("a").Value);
			Assert.AreEqual("${x}", x.Descendants("c").First().Attribute("a").Value);
		}

		/// <summary>
		/// Тест в котором один атрибут просто прошивается в другой в одном элементе,
		/// но при этом производится 2 преобразования
		/// </summary>
		[Test]
		public void CanStopInterpolationTotally()
		{
			var x = XElement.Parse("<a x='1'><b a='${x}' stopinterpolate='all'><c a='${x}'/></b></a>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("${x}", x.Descendants("b").First().Attribute("a").Value);
			Assert.AreEqual("${x}", x.Descendants("c").First().Attribute("a").Value);
		}


		/// <summary>
		/// Простой проброс интрполяции вниз
		/// </summary>
		[Test]
		public void SimleInterpolation2Level()
		{
			var x = XElement.Parse("<a x='1'><b y='${x}'/></a>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("1", x.Element("b").Attribute("y").Value);
			
		}

		/// <summary>
		/// Простой проброс интрполяции вниз с перекрытием родительского атрибута
		/// </summary>
		[Test]
		public void SimleInterpolation2LevelOverride()
		{
			var x = XElement.Parse("<a x='1' y='2'><b y='${x}${x}${x}' z='${y}z${y}'/></a>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("111z111", x.Element("b").Attribute("z").Value);

		}
		/// <summary>
		/// Простой проброс интрполяции вниз с явной ссылкой на родительский атрибут
		/// </summary>
		[Test]
		public void SimleInterpolation2LevelExplicitParent()
		{
			var x = XElement.Parse("<a x='1' y='2'><b y='${x}${x}${x}' z='${.y}z${.y}'/></a>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("2z2", x.Element("b").Attribute("z").Value);

		}

		/// <summary>
		/// Простой проброс интрполяции вниз с явной ссылкой на родительский атрибут
		/// </summary>
		[Test]
		public void BugNotResolveWithLevel()
		{
			var x = new BxlParser().Parse(
@"
test11 x=A
	test12 
		test14 x=B
			test15
				test16 x='${..x}${.x}C' y='${x}+'"
				);
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("ABC", x.Descendants("test16").First().Attribute("x").Value);

		}
	}
}