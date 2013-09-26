using System.Diagnostics;
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
        [Test]
        public void CanDrawElementStructure() {
            var element = new ChartElement();
            element.AddAttribute("test", "value");
            element.AddAttribute("test2", "OK");

            element.AddChild("test");

            var xml = element.DrawStructure();
            Debug.Print(xml.ToString());

            Assert.AreEqual("value", xml.Attribute("test").Value);
            Assert.AreEqual("OK", xml.Attribute("test2").Value);
            Assert.NotNull(xml.Element("test"));
        }
        [Test]
        public void CanDrawChartStructure() {
            var chart = new Chart();

            var dataset1 = new ChartDataset();
            dataset1.AddValue(1000);
            dataset1.AddValue(5003);
            dataset1.AddValue(2031);

            var dataset2 = new ChartDataset();
            dataset2.AddValue(1000);
            dataset2.AddValue(5003);
            dataset2.AddValue(2031);

            chart.AddDataset(dataset1);
            chart.AddDataset(dataset2);

            Debug.Print(chart.DrawStructure().ToString());
        }
    }
}
