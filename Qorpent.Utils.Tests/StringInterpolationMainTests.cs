using System;
using NUnit.Framework;

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

		[TestCase("${a}","a:1","1")]
		[TestCase("${a} ${b}","a:1|b:25","1 25")]
		[TestCase("${ab} ${bc}","ab:1|bc","1 ")]
		[TestCase("${ab} ${bc}","ab:1|bc:_EMPTY_","1 ")]
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
		[TestCase("${a,b}", "a:_EMPTY_|b:2", "" , Description="first is explicitly empty")]
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
	}
}
