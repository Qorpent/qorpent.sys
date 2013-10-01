using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace fcapimerge {
    public class FusionChartAttributeMerger {

        public XElement Merge(XElement source) {
            var result = new XElement("fcindex");
            IEnumerable<FusionChartAttribute> attributes = source.Descendants("attribute").Select(GenerateAttribute).Where(_=>_!=null).ToArray();
            IDictionary<string, FusionChartAttribute> index = BuildIndex(attributes);
            SerializeIndex(index, result);
            Console.WriteLine(attributes.Count());
            Console.WriteLine(index.Count());
            Console.WriteLine(attributes.Count(_ => _.IsDefault));
            Console.WriteLine(attributes.Count(_ => _.IsCommon));

            var uniqueChartSets = attributes.Where(_ => !(_.IsCommon || _.IsDefault)).Select(_ => _.Chart).Distinct();
            Console.WriteLine(uniqueChartSets.Count());
            /*foreach (var fusionChartType in uniqueChartSets) {
                Console.WriteLine(fusionChartType.ToStr());
            }*/

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
            var result= new FusionChartAttribute(xElement);
            if (result.Name.ToLower().Contains("deprecated") || result.Name.ToLower().Contains("since v")) {
                result.Name = result.Name.Split('\r', '\n')[0];
            }
            return result;
        }

    }
}