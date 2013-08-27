using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.BSharp.Matcher;
using Qorpent.Bxl;

namespace Qorpent.Serialization.Tests.BSharp.Matcher
{
	[TestFixture]
	public class SingleConditionTests
	{

		[TestCase("x a!=1", "x a=1", false)]
		[TestCase("x a!=2", "x a=1", true)]
		[TestCase("x a=1","x a=1",true)]
		[TestCase("x a!=1", "x", true)]
		[TestCase("x a!=2", "x", true)]
		[TestCase("x a=1", "x", false)]
		public void EqAnNeq(string src, string trg, bool result) {
			MainTest(src, trg, result);
		}

		[TestCase("x a=TRUE", "x", false)]
		[TestCase("x a!=TRUE", "x", true)]
		[TestCase("x a=TRUE", "x a=1", true)]
		[TestCase("x a=TRUE", "x a=true", true)]
		[TestCase("x a=TRUE", "x a=0", false)]
		[TestCase("x a!=TRUE", "x a=0", true)]	
		public void IsTrue(string src, string trg, bool result)
		{
			MainTest(src, trg, result);
		}

		[TestCase("x a=NULL","x",true)]
		[TestCase("x a=NULL","x a=1",false)]
		[TestCase("x a!=NULL","x a=1",true)]
		[TestCase("x a!=NULL","x",false)]
		public void IsNull(string src, string trg, bool result)
		{
			MainTest(src, trg, result);
		}

		[TestCase("x a>=2", "x", false)]
		[TestCase("x a>=2", "x a=3", true)]
		[TestCase("x a>=2", "x a=2", true)]
		[TestCase("x a>>=2", "x a=3", true)]
		[TestCase("x a>>=2", "x a=2", false)]
		[TestCase("x a<=2", "x", true)]
		[TestCase("x a<=2", "x a=3", false)]
		[TestCase("x a<=2", "x a=2", true)]
		[TestCase("x a<=2", "x a=1", true)]
		[TestCase("x a<<=2", "x a=1", true)]
		[TestCase("x a<<=2", "x a=2", false)]
		public void IsLeGe(string src, string trg, bool result)
		{
			MainTest(src, trg, result);
		}
		[TestCase("x a%=z", "x a=xzx", true)]
		[TestCase("x a%=a", "x a=xzx", false)]
		[TestCase("x a%=a", "x", false)]
		public void Contains(string src, string trg, bool result)
		{
			MainTest(src, trg, result);
		}

		[TestCase("x a~='z\\\\d'", "x a=xz2x", true)]
		[TestCase("x a~='z\\\\d'", "x a=xzAx", false)]
		[TestCase("x a~='z\\\\d'", "x", false)]
		public void Regex(string src, string trg, bool result)
		{
			MainTest(src, trg, result);
		}

		[TestCase("x a&=val", "x a='x val c'", true)]
		[TestCase("x a&=val", "x a='x, val, c'", true)]
		[TestCase("x a&=val", "x a='x; val; c'", true)]
		[TestCase("x a&=val", "x a='x val2 c'", false)]
		[TestCase("x a&=val", "x", false)]
		public void InList(string src, string trg, bool result)
		{
			MainTest(src, trg, result);
		}

		private static void MainTest(string src, string trg, bool result) {
			var p = new BxlParser();
			var srca = p.Parse(src,"c",BxlParserOptions.NoLexData).Element("x").Attributes().First();
			var c = new SingleCondition(srca);
			var trge = p.Parse(trg,"c",BxlParserOptions.NoLexData).Element("x");
			var res = c.IsMatch(trge);
			Assert.AreEqual(result, res);
		}
	}
}
