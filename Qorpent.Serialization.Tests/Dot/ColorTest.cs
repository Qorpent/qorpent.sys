using NUnit.Framework;
using Qorpent.Dot.Colors;

namespace Qorpent.Serialization.Tests.Dot {
    [TestFixture]
    public class ColorTest {
        [Test]
        public void CanSumColorAndDouble() {
            var item = Color.Red + 0.3;
            Assert.IsInstanceOf<ColorListItem>(item);
            ColorList list = item;
            ColorAttribute attr = list;
            var item2 = Color.Red + 0.3 + Color.RGB(0x00,0xFF,0xAA) + 0.2 + Color.Create("33abcd") + 0.5;
            Assert.IsInstanceOf<ColorList>(item2);
            Assert.AreEqual("red;0.3:#00ffaa;0.2:#33abcd;0.5",item2.ToString());
        }    
    }
}