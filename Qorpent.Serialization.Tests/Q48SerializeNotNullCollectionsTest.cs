using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests
{
	[TestFixture]
	public class Q48SerializeNotNullCollectionsTest
	{
		[Serialize]
		public class A {

			[SerializeNotNullOnly]
			public string[] Ar { get; set; }

			[SerializeNotNullOnly]
			public List<string> Lst { get; set; }

			[SerializeNotNullOnly]
			public IDictionary<string,string> Dct { get; set; }

		}
		[TestCase(2, 2, 2, "{'Ar': ['test'], 'Lst': ['test'], 'Dct': {'test': 'test'}}")]
		[TestCase(1,1,1,"{}")]
		[TestCase(0, 0, 0, "{}")]
		public void TestSerialization(int usear, int uselst, int usedct,string expected) {
			var obj = new A();
			if (1 == usear) {
				obj.Ar = new string[] {};
			}
			if (2 == usear)
			{
				obj.Ar = new string[] {"test" };
			}
			if (1 == uselst) {
				obj.Lst = new List<string>();
			}
			if (2 == uselst) {
				obj.Lst = new List<string> {"test"};
			}

			if (1 == usedct) {
				obj.Dct = new Dictionary<string, string>();
			}
			if (2 == usedct)
			{
				obj.Dct = new Dictionary<string,string> {{ "test" ,"test"}};
			}

			var s = new JsonSerializer();
			var res = s.Serialize("test", obj).Trim().Replace("\"", "'");
			Console.WriteLine(res);

			Assert.AreEqual(expected,res);
		}
	}
}
