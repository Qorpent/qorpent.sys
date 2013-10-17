using System.Linq;
using NUnit.Framework;
using Qorpent.AbstractCanvas;

namespace Qorpent.Serialization.Tests {
    /// <summary>
    /// 
    /// </summary>
    public class CanvasTests {
        [Test]
        public void DefaultScale() {
            var canvas = new Canvas(-10, 100, 0, 200);

            Assert.AreEqual(110, canvas.Width);
            Assert.AreEqual(200, canvas.Height);
        }
        [TestCase(0, 100, 0, 200, 20, 40, 1, false)]
        [TestCase(0, 100, 0, 200, 20, 40, 2, true)]
        [TestCase(0, 100, 0, 200, 20, 40, 4, true)]
        [TestCase(0, 100, 0, 200, 100, 200, 10, true)]
        public void CanScaleCanvas(double a, double b, double c, double d, int w, int h, int px, bool u) {
            var canvas = new Canvas(a, b, c, d);
            canvas.Scale(w, h);

            var p = canvas.SetPoint(10, 10);
            canvas.SetPoint(15, 15);

            var n = canvas.Nearby(p, px);
            var p1 = n.FirstOrDefault();

            if (u) {
                Assert.AreEqual(1, n.Count());
                Assert.AreEqual(15.0, p1.X);
                Assert.AreEqual(15.0, p1.Y);
            } else {
                Assert.AreEqual(0, n.Count());
            }

        }
    }
}
