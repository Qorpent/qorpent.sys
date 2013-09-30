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
            Type = xElement.Attr("name");
            Range = xElement.Attr("name");
            var desc = new XElement("desc");
            desc.Add(xElement.Nodes());
            Description = desc.ToString();
            Category = xElement.Parent.Attr("category");
            var elementHandler = xElement.Ancestors().First(_ => null != _.Attribute("element"));
            var chartType = xElement.Ancestors("part").First().Attr("charttype");
            var elementFullName = Escaper.PascalCase( elementHandler.Attr("subelement") )+ Escaper.PascalCase( elementHandler.Attr("element"));
            Element = (FusionChartElementType) Enum.Parse(typeof (FusionChartElementType), elementFullName, true);
            Chart = (FusionChartType)Enum.Parse(typeof(FusionChartType), chartType, true);
        }

        public string Key { get { return Name + "_" + Element ; } }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Range { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public FusionChartElementType Element { get; set; }
        public FusionChartType Chart { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XElement ToXmlElement() {
            return new XElement("attribute",
                                new XAttribute("name",Name),
                                new XAttribute("element",Element),
                                new XAttribute("chart",Chart.ToString().Replace(", "," + ")),
                                new XAttribute("type",Type),
                                new XAttribute("range",Range),
                                new XAttribute("category",Category),
                                new XText(Description)
                
                );

        }
    }
}