using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace fcapimerge {
    public class FusionChartAttributeMerger {

        public XElement Merge(XElement source) {
            var result = new XElement("fcindex");
            IEnumerable<FusionChartAttribute> attributes = source.Descendants("attribute").Select(GenerateAttribute).ToArray();
            IDictionary<string, FusionChartAttribute> index = BuildIndex(attributes);
            SerializeIndex(index, result);
            Console.WriteLine(attributes.Count());
            Console.WriteLine(index.Count());
            return result;
        }

        private void SerializeIndex(IDictionary<string, FusionChartAttribute> index, XElement result) {
            foreach (var attribute in index.Values) {
                result.Add(attribute.ToXmlElement());
            }
        }

        private IDictionary<string, FusionChartAttribute> BuildIndex(IEnumerable<FusionChartAttribute> attributes) {
            var result = new Dictionary<string, FusionChartAttribute>();
            foreach (var a in attributes) {
                if (result.ContainsKey(a.Key)) {
                    var existed = result[a.Key];
                    if (string.IsNullOrWhiteSpace(existed.Description)) {
                        existed.Description = a.Description;
                    }
                    existed.Chart = existed.Chart | a.Chart;
                }
                else {
                    result[a.Key] = a;
                }
            }
            return result;
        }

        private FusionChartAttribute GenerateAttribute(XElement xElement) {
            return new FusionChartAttribute(xElement);
        }

    }
}