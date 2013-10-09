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
        public void RoundToNearestOrder(double s, int o, int r) {
            Assert.AreEqual(r, s.RoundToNearestOrder(o));
        }
    }
}
