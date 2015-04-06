using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
    /// <summary>
    /// 
    /// </summary>
    public class MathTests {
        [TestCase(1024.0, 1, 1020)]
        [TestCase(1126.0, 2, 1130)]
        [TestCase(1171.0, 2, 1170)]
        public void Round(double s, int o, int r) {
            Assert.AreEqual(r, s.Round(o));
        }
        [TestCase(1024.0, 1, 1030)]
        [TestCase(1126.0, 1, 1130)]
        [TestCase(1171.0, 1, 1180)]
        [TestCase(1171.0, 2, 1200)]
        public void RoundUp(double s, int o, int r) {
            Assert.AreEqual(r, s.RoundUp(o));
        }
        [TestCase(1024.0, 1, 1020)]
        [TestCase(1126.0, 1, 1120)]
        [TestCase(1120.0, 2, 1100)]
        [TestCase(1171.0, 1, 1170)]
        [TestCase(1171.0, 2, 1100)]
        [TestCase(-90.0, 2, -100)]
        [TestCase(-10.0, 2, -100)]
        public void RoundDown(double s, int o, int r) {
            Assert.AreEqual(r, s.RoundDown(o));
        }
        [TestCase(100, 3)]
        [TestCase(51.344, 2)]
        public void GetNumberOfDigits(double s, int e) {
            Assert.AreEqual(e, s.GetNumberOfDigits());
        }
    }
}
