using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Qorpent.Core.Tests
{
	[TestFixture]
	public class CombinationalTest
	{
		

		[Test]
		public void CombineToIntArrays()
		{
			var ints1 = new[] {1, 2, 3};
			var ints2 = new[] { 4, 5};
			var ints3 = new[] {5, 6, 7, 8};
			var result = Combine(ints1,ints2,ints3);
			Assert.AreEqual(24,result.Length);
		}
	}
}
