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
        [TestCase(1126.0, 2, 1130)]
        [TestCase(1171.0, 2, 1180)]
        public void RoundUp(double s, int o, int r) {
            Assert.AreEqual(r, s.RoundUp(o));
        }
        [TestCase(1024.0, 1, 1020)]
        [TestCase(1126.0, 2, 1120)]
        [TestCase(1171.0, 2, 1170)]
        public void RoundDown(double s, int o, int r) {
            Assert.AreEqual(r, s.RoundDown(o));
        }
    }
}
