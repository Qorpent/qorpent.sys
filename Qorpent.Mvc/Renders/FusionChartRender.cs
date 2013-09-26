using System;
using System.Xml.Linq;
using Qorpent.Dsl;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    /// Render для отрисовки графиков Fusionchart
    /// </summary>
    [Render("fc")]
    public class FusionChartRender: RenderBase {
        /// <summary>
        /// Renders given context
        /// </summary>
        /// <param name="context"></param>
        public override void Render(IMvcContext context) {
            var render = context.Get("render");
            var type = ResolveChartType(context);
            var id = context.Get("id", "fc-graph-" + DateTime.Now.Ticks);
            var container = context.Get("__targetdiv");
            var width = context.Get("width", "400");
            var height = context.Get("height", "300");
            var debug = context.Get("debug", "0");
            var script = string.Empty;

            if (!string.IsNullOrWhiteSpace(container)) {
                script += string.Format(@"
<div class=""fusinchart-cintainer"" id=""{0}""></div>", id);
                container = id;
            }
            script += string.Format(@"
<script type=""text/javascript""><!--
    {0}
    var myChart = new FusionCharts('{1}', '{2}', '{3}', '{4}', '{5}');
    {7}
    myChart.render('{6}');
// -->
</script>", render == "javascript" ? "FusionCharts.setCurrentRenderer('"+ render + "');" : "", type, id, width, height, debug, container, RenderDataScript(context));
            context.ContentType = "text/html";

            if (!string.IsNullOrWhiteSpace(context.Get("__standalone"))) {
                script = @"
<html>
<header>
</header>
<body>
<script type=""text/javascript"" src=""/scripts/jquery.min.js""></script>
<script type=""text/javascript"" src=""/scripts/fusioncharts/FusionCharts.js""></script>
<link rel=""stylesheet"" type=""text/css"" href=""/styles/fusioncharts/presentation.css""></link>
" + script + @"
</body>
</html>";
            }

            context.Output.Write(script);
        }

        private string ResolveChartType(IMvcContext context) {
            var type = context.Get("__charttype");
            if (string.IsNullOrWhiteSpace(type)) {
                if (context.ActionResult is XElement) {
                    type = ((XElement)context.ActionResult).Attribute("charttype").Value;
                }
                if (context.ActionResult is String && context.ActionResult.ToString().IndexOf("{", StringComparison.InvariantCulture) != 0) {
                    type = ResolveService<ISpecialXmlParser>("json.xml.parser").ParseXml((string)context.ActionResult).Attribute("charttype").Value;
                }
            }
            if (string.IsNullOrWhiteSpace(type)) {
                type = "Column2D";
            }
            return type;
        }

        /// <summary>
        /// Render "set data" part of script 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string RenderDataScript(IMvcContext context) {
            var script = @"myChart.set{0}Data(""{1}"");";
            string type;
            var data = string.Empty;
            if (context.ActionResult is XElement) {
                type = "XML";
                data = context.ActionResult.ToString();
            }
            else if (context.ActionResult is String && context.ActionResult.ToString().StartsWith("{")) {
                type = "JSON";
                data = context.ActionResult.ToString();
            }
            else {
                type = "JSON";
                data = @"{""chart"": {""caption"" : ""Data is not supporting""},""data"" : []}";
            }
            script = string.Format(script, type, data);
            return script;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="context"></param>
        public override void RenderError(Exception error, IMvcContext context) {
            context.ContentType = "text/plain";
            context.Output.Write(error.ToString());
        }
    }
}
