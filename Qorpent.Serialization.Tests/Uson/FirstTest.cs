using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Uson;

namespace Qorpent.Serialization.Tests.Uson
{
	[TestFixture]
	public class FirstTest
	{
		class test
		{
			public int A;
			public int B;
		}
		[Test]
		public void CanCreateAndUse()
		{
			dynamic uobj = new UObj();
			//one field 
			uobj.a.b.c = 1;
			// second field (not alphabetic order)
			uobj.a.b.a = "tren\nd";
			//only GET will be treated as garbage
			var a = uobj.a.b.u.c.d.f;
			Assert.AreEqual(1,uobj.a.b.c);
			Console.WriteLine(uobj.ToJson());
			//test valid JSON in alphabet mode
			Assert.AreEqual(@"{""a"":{""b"":{""a"":""tren\nd"",""c"":1}}}", uobj.ToJson());
			uobj.a = new test {A = 3, B = 4};
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""a"":{""A"":3,""B"":4}}", uobj.ToJson());
			Console.WriteLine(uobj.ToJson(UObjSerializeMode.KeepType));
			Assert.AreEqual(@"{""a"":{""_srctype"":""Qorpent.Serialization.Tests.Uson.FirstTest+test, Qorpent.Serialization.Tests"",""A"":3,""B"":4}}", uobj.ToJson(UObjSerializeMode.KeepType));
		}
	}
}
