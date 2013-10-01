using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Charts;
using Qorpent.Serialization.Charts.Implementation;
using Qorpent.Dsl;
using Qorpent.IoC;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    /// Render для отрисовки графиков Fusionchart
    /// </summary>
    [Render("chart")]
    public class FusionChartRender: RenderBase {

        [Inject]
        private IChartRender InternalRender { get; set; }

        /// <summary>
        /// Renders given context
        /// </summary>
        /// <param name="context"></param>
        public override void Render(IMvcContext context) {
            
            var config = PrepareChartConfig(context);
            var script = string.Empty;

            var datascript = RenderDataScript(context, config);
            var error = string.Empty;

            if (string.IsNullOrWhiteSpace(datascript)) {
                error = "Нет данных для отображения";
            }

            var id = config.Get<string>("Id");
            var container = config.Get<string>("Container");

            if (string.IsNullOrWhiteSpace(container)) {
                container = "fc-container-" + id;
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
</script>", config.Get<string>("Type"), id, config.Get<string>("Width"), config.Get<string>("Height"), config.Get<string>("Debug"), datascript, container);
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

        private IChartConfig PrepareChartConfig(IMvcContext context) {
            var result = new ChartConfig();
            result.Set("Id", context.Get("__id", "fc-chart-" + DateTime.Now.Ticks));
            result.Set("Container", context.Get("__container", string.Empty));
            result.Set("Width", context.Get("__width", "400"));
            result.Set("Height", context.Get("__height", "300"));
            result.Set("Debug", context.Get("__debug", "0"));
            result.Set("Type", context.Get("__type", "Column2D"));

            var specAttrs = context.GetAll("fc");
            foreach (var attr in specAttrs) {
                result.Set(attr.Key, attr.Value);
            }
            return result;
        }

        /// <summary>
        /// Определяет тип графика (по-умолчанию Column2D)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="datatype"></param>
        /// <returns></returns>
        private string ResolveChartType(IMvcContext context, string datatype = null) {
            var type = context.Get("__charttype");

            if (string.IsNullOrWhiteSpace(type)) {
                if (null == datatype) datatype = ResolveRenderType(context);
                XAttribute typeattr = null;
                if (datatype == "xml") {
                    typeattr = ((XElement)context.ActionResult).Attribute("charttype");
                }
                else if (datatype == "xmlstring") {
                    typeattr = (XElement.Parse((string) context.ActionResult)).Attribute("charttype");
                }
                else if (datatype == "json") {
                    typeattr = ResolveService<ISpecialXmlParser>("json.xml.parser").ParseXml((string)context.ActionResult).Attribute("charttype");
                }
                else if (datatype == "") {
                    
                }
                else if (null != typeattr) {
                    type = typeattr.Value;
                }
                else {
                    
                }

            }
            if (string.IsNullOrWhiteSpace(type)) {
                type = "Column2D";
            }
            return type;
        }

        /// <summary>
        /// Определяет тип рендера графика
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string ResolveRenderType(IMvcContext context) {
            return
                (context.ActionResult is String && context.ActionResult.ToString().Trim().StartsWith("{")) ? "javascript" : "xml";
        }

        /// <summary>
        /// Render "set data" part of script 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private string RenderDataScript(IMvcContext context, IChartConfig config) {
            var data = string.Empty;
            if (context.ActionResult is XElement) {
                
            } else if (context.ActionResult is String && context.ActionResult.ToString().Trim().StartsWith("<")) {
                
            } else if (context.ActionResult is String && context.ActionResult.ToString().Trim().StartsWith("{")) {
                
            } else if (context.ActionResult is IChartXmlSource) {
                
            } else if (context.ActionResult is IChartSource) {
                
            } else if (context.ActionResult is IChart) {

            } else {
                
            }
            return data;
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
