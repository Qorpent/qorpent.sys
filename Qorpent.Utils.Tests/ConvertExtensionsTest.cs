using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests
{
	/// <summary>
	/// Проверяем работу конвертера типов
	/// </summary>
	[TestFixture]
	public class ConvertExtensionsTest
	{
		
		public enum  X {
			Default = 0,
			A = 1,
			B = 2 
		}

        public enum Y
        {
            A = 1,
            B = 1<<1,
            C = 1<<2,
        }
		
		[Test]
		public void EnumFromNullWorks() {
			var x = ((object) (null)).To<X>();
			Assert.AreEqual(X.Default,x);
		}


		[Test]
		public void EnumFromEmptyStringWorks()
		{
			var x = "".To<X>();
			Assert.AreEqual(X.Default, x);
		}

        [Test]
        public void EnumFromMultipleStringWorks()
        {
            var res = "A+C".To<Y>();
            Assert.AreEqual(Y.A | Y.C, res);
        }

		
			
	}
}
