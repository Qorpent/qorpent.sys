using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Uson;

namespace Qorpent.Utils.Tests
{
	/// <summary>
	/// Тесты, проверяющие подстановку
	/// </summary>
	[TestFixture]
	public class StringInterpolationMainTests
	{
		private StringInterpolation _si;

		[SetUp]
		public void SetUp() {
			_si = new StringInterpolation();
		}
		[TestCase("${a}","")]
		[TestCase("${a}${b}","")]
		[TestCase("x${a}y${b}z","xyz")]
		public void CanReplaceWithEmptiesIfNoSourceGiven(string src, string result) {
			Assert.AreEqual(result,_si.Interpolate(src));
		}

		[Test]
		public void MI_334_Invalid_Train_AncorTrim(){
			var str = "xxx$";
			var result = new StringInterpolation().Interpolate(str, new{x = 1});
			Assert.AreEqual(str,result);
		}

		[TestCase("${a}${{a}", "1${a}")]
		public void CanEscapeInerpolationsOnStrFormatManer(string src, string result){
			Assert.AreEqual(result,_si.Interpolate(src,new{a=1}));
		}

		[Test]
		public void CanInterpolateUObj(){
			Assert.AreEqual("12", _si.Interpolate("${x}${y}", new { x = 1, y = 2 }.ToUson()));
		}


		[Test]
		public void CanInterpolateWithDots()
		{
			Assert.AreEqual("12", _si.Interpolate("${t.x}${t.y}", null, new Dictionary<string, object>{{"t.x",1},{"t.y",2}}));
		}

		[Test]
		public void CanInterpolateWithDotsAndDefaults()
		{
			Assert.AreEqual("12013", _si.Interpolate("${t.x:2}${t.y:2013}", null, new Dictionary<string, object> { { "t.x", 1 } }));
		}
		

		[TestCase("$a}", "$a}")]
		[TestCase("$a} ${b", "$a} ${b")]
		[TestCase("$a} ${b}", "$a} ")]
		[TestCase("$a ${b}", "$a ")]
		[TestCase("${a", "${a")]
		[TestCase("x${a z", "x${a z")]
		public void NotFullyFinishedPlacesAreReturnedAsInSource(string src, string result)
		{
			Assert.AreEqual(result, _si.Interpolate(src));
		}
		[TestCase("$${a}", "a:1", "$1")]
		[TestCase("${a}","a:1","1")]
		[TestCase("${a} ${b}","a:1|b:25","1 25")]
		[TestCase("${ab} ${bc}","ab:1|bc","1 ")]
		[TestCase("${ab} ${bc}", "ab:1|bc:¶", "1 ")]
		public void SimpleSubstitutions(string src,string datasrc, string result) {
			var data = ComplexStringExtension.Parse(datasrc);
			var realresult = _si.Interpolate(src,data);
			Console.WriteLine(realresult);
			Assert.AreEqual(result,realresult );
		}

		[TestCase("${a%0000}", "a:int~1", "0001")]
		[TestCase("${a%dd.MM.yyyy}", "a:datetime~2003-02-01", "01.02.2003")]
		[TestCase("${a%000.00}", "a:decimal~23.4484", "023.45")]
		public void FormattedSubstitutions(string src, string datasrc, string result)
		{
			var data = ComplexStringExtension.Parse(datasrc);
			var realresult = _si.Interpolate(src, data);
			Console.WriteLine(realresult);
			Assert.AreEqual(result, realresult);
		}

		
		[TestCase("${name} ${code} ${idx}","n c 2")]
		[TestCase("${name} ${code} ${no_idx}","n c ")]
		[TestCase("${name} ${code} ${no_idx:23}","n c 23")]
		[TestCase("${x,name} ${y,code} ${no_idx:23}","n c 23")]
		[TestCase("${code,name} ${name,code} ${no_idx:23}","c n 23")]
		public void SimpleSubstitutionsFromObject(string src, string result) {
			var obj = new {name = "n", code = "c", idx = 2};
			var realresult = _si.Interpolate(src, obj);
			Console.WriteLine(realresult);
			Assert.AreEqual(result,realresult);
		}

		[TestCase("${a,b}", "a|b:2", "2" , Description="first is empty")]
		[TestCase("${a,b}", "a:1|b:2", "1" , Description="first is not empty")]
		[TestCase("${a,b}", "a:¶|b:2", "", Description = "first is explicitly empty")]
		[TestCase("${a,b}", "b:2", "2")]
		[TestCase("${c,b}", "b:2|c:3", "3")]
		[TestCase("${a,b} ${c,d,e} ${e,d,c}", "b:2|c:3|e:4", "2 3 4")]
		public void ConcurentSubstitutuionsNoDefault(string src, string datasrc, string result)
		{
			var data = ComplexStringExtension.Parse(datasrc);
			var realresult = _si.Interpolate(src, data);
			Console.WriteLine(realresult);
			Assert.AreEqual(result, realresult);
		}

		[TestCase("${a,b:test}", "b:2", "2")]
		[TestCase("${a,b:test}", "c:x", "test")]
		[TestCase("${a,b:test! many things!!!}", "c:x", "test! many things!!!")]
		[TestCase("${a,b:test! many things!!!}", "a:1", "1")]
		[TestCase("${a,b:test! many things!!!}", "b:12", "12")]
		public void ConcurentSubstitutuionsWitDefault(string src, string datasrc, string result)
		{
			var data = ComplexStringExtension.Parse(datasrc);
			var realresult = _si.Interpolate(src, data);
			Console.WriteLine(realresult);
			Assert.AreEqual(result, realresult);
		}

	    [Test]
	    public void LimitedSupportForFunctionCall() {
	        var code = "${getx()}${gety(2)}${getz(3)}${getz(3,2)}${getz(gety(4),5)}";
	        var src = new {
	            getx = (Func<int>) (() => 1),
	            gety = (Func<int, int>) (s => s*2),
	            getz = (Func<int, int, int>) ((s, w) => s*2 + w*3)
	        };var result = _si.Interpolate(code,src);
            Console.WriteLine(result);
            Assert.AreEqual("1461231", result);
	    }

       
        [Test]
        public void CoreLibFunctionCall()
        {
            var code = "'${upper(x)}', '${lower(x)}', '${trim(x)}', '${match(x,'b.')}', '${match(x,'b(.)','1')}', '${match(x,'(?<a>b).','a')}', '${replace(x,'b','z')}'";
            
            var result = _si.Interpolate(code, new{x=" Abc "});
            Console.WriteLine(result);
            Assert.AreEqual("' ABC ', ' abc ', 'Abc', 'bc', 'c', 'b', ' Azc '", result);
        }


        [Test]
        public void TypeResolutionInFunctionCall()
        {
            var code = "'${stringer(i)}', '${inter(s)}', '${inter(d)}', '${decer(s)}', '${decer(i)}'";

            var result = _si.Interpolate(code, 
                new {
                    s = "Abc",
                    i = 2,
                    d = 4.5m,
                    stringer = (Func<string,string>)(s=>s+"!"),
                    inter = (Func<int,int>)(s=>s+3),
                    decer = (Func<decimal,decimal>)(s=>s+2.1m),
                });
            Console.WriteLine(result);
            Assert.AreEqual("'2!', '3', '7', '2.1', '4.1'", result);
        }

	    [Test]
	    public void StructureResolutionSupport() {
	        var code = "${x.a} ${x.b.c} ${x.b.c.gotta()}";
	        var ctx = new {x = new {a= "2", b = new {c = new {gotta = (Func<string>) (() => "h")}}}};
	        var result = _si.Interpolate(code, ctx);
            Console.WriteLine(result);
            Assert.AreEqual("2 { gotta = System.Func`1[System.String] } h", result);
	    }

        [Test]
        public void StructureResolutionSupportScope()
        {
            var code = "${x.a.b}";
            var ctx = new { x = new { a = new Scope(new{b=2})}};
            var result = _si.Interpolate(code, ctx);
            Console.WriteLine(result);
            Assert.AreEqual("2", result);
        }
        [Test]
        public void StructureResolutionSupportDictionary()
        {
            var code = "${x.a.b}";
            var ctx = new { x = new { a = new Scope(new Dictionary<string,object> { {"b" ,2} }) } };
            var result = _si.Interpolate(code, ctx);
            Console.WriteLine(result);
            Assert.AreEqual("2", result);
        }
        [Test]
        public void StructureResolutionSupportXElement()
        {
            var code = "${x.a.b.c}${x.a.b.value()}";
            var ctx = new { x = new { a = XElement.Parse("<a><b c='2'>z</b></a>") } };
            var result = _si.Interpolate(code, ctx);
            Console.WriteLine(result);
            Assert.AreEqual("2z", result);
        }


        [Test]
        public void CanWorkWithPrefix()
        {
            var code = "${x.y}${x.z}";
            var src = new Dictionary<string, object>();
            src["x.y"] = 1;
            src["x.z"] = 4;
            var result = _si.Interpolate(code, src);
            Console.WriteLine(result);
            Assert.AreEqual("14", result);
        }
	}
}
