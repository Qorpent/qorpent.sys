using System.IO;
using System.Xml.Linq;

namespace fcapimerge
{
    class Program
    {
        static void Main(string[] args) {
            var src = XElement.Load("clean_fusion_chart_api.xml");
            var result = new FusionChartAttributeMerger().Merge(src);
            File.WriteAllText("merged_fusion_chart_api.xml",result.ToString());
        }
    }
}
