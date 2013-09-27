using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Dsl;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    /// Render для отрисовки графиков Fusionchart
    /// </summary>
    [Render("chart")]
    public class FusionChartRender: RenderBase {
        /// <summary>
        /// Renders given context
        /// </summary>
        /// <param name="context"></param>
        public override void Render(IMvcContext context) {
            var render = ResolveDataType(context);
            var charttype = ResolveChartType(context, render);
            var id = context.Get("__id", DateTime.Now.Ticks);
            var container = context.Get("__targetdiv");
            var width = context.Get("__width", "400");
            var height = context.Get("__height", "300");
            var debug = context.Get("__debug", "0");
            var script = string.Empty;
            var datascript = RenderDataScript(context, render);
            var error = string.Empty;

            if (string.IsNullOrWhiteSpace(datascript)) {
                error = "Нет данных для отображения";
            }

            if (string.IsNullOrWhiteSpace(container)) {
                container = "fc-graph-" + id;
                script += string.Format(@"
<div class=""fusinchart-container{0}"" id=""{1}"">{2}</div>", string.IsNullOrEmpty(error) ? " fusionchart-error" : "", container, error);
            }

            if (string.IsNullOrWhiteSpace(error)) {
                script += string.Format(@"
<script type=""text/javascript""><!--
    FusionCharts.setCurrentRenderer('javascript');
    var myChart = new FusionCharts('../charts/{0}.swf', '{1}', '{2}', '{3}', '{4}');
    {5}
    myChart.render('{6}');
// -->
</script>", charttype, "fc-chart-" + id, width, height, debug, datascript, container);
                context.ContentType = "text/html";   
            }

            if (!string.IsNullOrWhiteSpace(context.Get("__standalone"))) {
                script = @"
<html>
<header>
</header>
<body>
<script type=""text/javascript"" src=""../scripts/jquery.min.js""></script>
<script type=""text/javascript"" src=""../charts/FusionCharts.js""></script>
<script type=""text/javascript"" src=""../charts/FusionCharts.HC.js""></script>
<script type=""text/javascript"" src=""../charts/FusionCharts.HC.Charts.js""></script>
<link rel=""stylesheet"" type=""text/css"" href=""../charts/presentation.css""></link>" + script + @"
</body>
</html>";
            }

            context.Output.Write(script);
        }

        private string ResolveChartType(IMvcContext context, string datatype = null) {
            var type = context.Get("__charttype");
            if (string.IsNullOrWhiteSpace(type)) {
                if (null == datatype) datatype = ResolveDataType(context);
                XAttribute typeattr = null;
                if (datatype == "xml") {
                    typeattr = ((XElement)context.ActionResult).Attribute("charttype");
                }
                if (datatype == "xmlstring") {
                    typeattr = (XElement.Parse((string) context.ActionResult)).Attribute("charttype");
                }
                if (datatype == "json") {
                    typeattr = ResolveService<ISpecialXmlParser>("json.xml.parser").ParseXml((string)context.ActionResult).Attribute("charttype");
                }
                if (null != typeattr) {
                    type = typeattr.Value;
                }
            }
            if (string.IsNullOrWhiteSpace(type)) {
                type = "Column2D";
            }
            return type;
        }

        private string ResolveDataType(IMvcContext context) {
            string type;
            if (context.ActionResult is XElement) {
                type = "xml";
            } else if (context.ActionResult is String && context.ActionResult.ToString().Trim().StartsWith("<")) {
                type = "xmlstring";
            } else if (context.ActionResult is String && context.ActionResult.ToString().Trim().StartsWith("{")) {
                type = "json";
            } /*else if (context.ActionResult is IChartXmlSource) {
                type = "xmlsource";
            } else if (context.ActionResult is IChartSource) {
                type = "source";
            }*/ else {
                type = "unknown";
            }
            return type;
        }

        /// <summary>
        /// Render "set data" part of script 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="datatype"></param>
        /// <returns></returns>
        private string RenderDataScript(IMvcContext context, string datatype = null) {
            if (null == datatype) datatype = ResolveDataType(context);
            if (null != context.ActionResult && (datatype == "xml" || datatype == "json" || datatype == "xmlstring")) {
                var result = context.ActionResult.ToString();
                result = Regex.Replace(result, @"\s{2,}", "");
                return string.Format(@"myChart.set{0}Data(""{1}"");", datatype == "xmlstring" ? "XML" : datatype.ToUpper(), result);
            }
            return "";
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
