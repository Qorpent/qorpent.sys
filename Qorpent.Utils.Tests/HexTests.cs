using NUnit.Framework;

namespace Qorpent.Utils.Tests {
    class HexTests {
        [Test]
        public void Canminus() {
            Hex hex1 = "B";
            Hex hex2 = "A";
            Assert.AreEqual("1", (string)(hex1 - hex2));
        }
        [Test]
        public void CanPlus() {
            Hex hex1 = "1";
            Hex hex2 = "A";
            Assert.AreEqual("B", (string)(hex1 + hex2));
        }
    }
}
