using System.Linq;
using NUnit.Framework;
using Qorpent.Charts.Implementation;

namespace Qorpent.Core.Tests.Charts {
    [TestFixture]
    public class ChartTests {
        [Test]
        public void CanUseDataSets() {
            var dataset = new ChartDataset();
            dataset.AddValue(1000);
            dataset.AddValue(5003);
            dataset.AddValue(2031);

            var values = dataset.GetValues().ToArray();

            Assert.AreEqual(3, values.Count());
            Assert.Contains(1000, values);
            Assert.Contains(5003, values);
            Assert.Contains(2031, values);
        }
        [Test]
        public void CanUseAttributesFromElement() {
            var element = new ChartElement();
            element.AddAttribute("test", "value");
            element.AddAttribute("test2", "OK");

            Assert.AreEqual("value", element.GetAttributeValue("test"));
            Assert.AreEqual("OK", element.GetAttributeValue("test2"));
            Assert.AreEqual(null, element.GetAttributeValue("NOT_EXISTS"));
        }
    }
}
