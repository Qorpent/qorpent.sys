using System;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Charts.FusionCharts;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace fcapimerge {
    public class FusionChartAttribute {
        public FusionChartAttribute(XElement xElement) {
            Name = xElement.Attr("name");
            Type = FusionChartDataType.String;
            var t = Escaper.PascalCase(xElement.Attr("type"));
            if (!string.IsNullOrWhiteSpace(t)) {
                Type =t.To<FusionChartDataType>();
            }
            Range = xElement.Attr("range");
            var desc = new XElement("desc");
            desc.Add(xElement.Nodes());
            Description = desc.ToString();
            Category = Escaper.PascalCase( xElement.Parent.Attr("category") );
            var elementHandler = xElement.Ancestors().First(_ => null != _.Attribute("element"));
            var chartType = xElement.Ancestors("part").First().Attr("charttype");
            var elementFullName = Escaper.PascalCase( elementHandler.Attr("subelement") )+ Escaper.PascalCase( elementHandler.Attr("element"));
            Element = (FusionChartElementType) Enum.Parse(typeof (FusionChartElementType), elementFullName, true);
            Chart = (FusionChartType)Enum.Parse(typeof(FusionChartType), chartType, true);
        }

        public string Key { get { return Name + "_" + Element ; } }
        public string Name { get; set; }
        public FusionChartDataType Type { get; set; }
        public string Range { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get {
            foreach (var v in Enum.GetValues(typeof (FusionChartType))) {
                if (0 == (long) v) continue;
                if (0 == ((FusionChartType)v & Chart)) return false;
            }
            return true;
        } }

        public bool IsCommon
        {
            get {
                int matches = 0;
                foreach (var v in Enum.GetValues(typeof(FusionChartType)))
                {
                    if (0 == (long)v) continue;
                    if (0 != ((FusionChartType) v & Chart)) matches++;
                }
                return matches >= 30;
            }
        }
        public FusionChartElementType Element { get; set; }
        public FusionChartType Chart { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XElement ToXmlElement() {
            var result =  new XElement("attribute",
                                new XAttribute("name",Name),
                                new XAttribute("element",Element),
                                new XAttribute("chart",Chart.ToString().Replace(", "," + ")),
                                new XAttribute("type",Type),
                                new XAttribute("range",Range),
                                new XAttribute("category",Category),
                                new XText(Description)
                
                );
            if (IsDefault) {
                result.SetAttributeValue("default",true);
            }else if (IsCommon) {
                result.SetAttributeValue("common", true);
            }
            return result;
        }
    }
}