using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Qorpent.Charts.ChartsApiAnalizer {
    class Program {
        static void Main(string[] args) {
            var target = "merged_fusion_chart_api.xml";
            if (!File.Exists(target)) Console.Write("file 'merged_fusion_chart_api.xml' not found!");

            var input = XElement.Load(target);
            var attrs = new List<string>();
            foreach (var attr in input.Elements("attribute")) {
                var name = (attr.Attribute("element").Value + "_" + attr.Attribute("name").Value).ToLower();
                if (attrs.Contains(name)) {
                    Console.WriteLine("Attribute " + name + " already exists");
                }
                else {
                    attrs.Add(name);
                }
            }
            Console.ReadKey();
        }
    }
}
