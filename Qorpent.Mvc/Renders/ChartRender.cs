using System;
using System.Globalization;
using System.Xml.Linq;
using Qorpent.Charts;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    /// Render для отрисовки графиков Fusionchart
    /// </summary>
    [Render("chart")]
    public class ChartRender: RenderBase {
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

            var id = config.Id;
            var container = config.Container;

            if (string.IsNullOrWhiteSpace(container)) {
                container = "fc-container-" + id;
                script += string.Format(@"
<div class=""fusinchart-container{0}"" id=""{1}"">{2}</div>", string.IsNullOrEmpty(error) ? " fusionchart-error" : "", container, error);
            }

            if (string.IsNullOrWhiteSpace(error)) {
                script += string.Format(@"
<div style=""display:none"" id=""fc-data-{1}"">{5}</div>
<script type=""text/javascript""><!--
    FusionCharts.setCurrentRenderer('javascript');
    var myChart = new FusionCharts('../charts/{0}.swf', 'fc-chart-{1}', '{2}', '{3}', '{4}');
    myChart.set{7}Data($('#fc-data-{1}').text());
    myChart.render('{6}');
// -->
</script>", config.Type, id, config.Width, config.Height, config.Debug, datascript.Replace("<", "&lt;"), container, config.DataType);
                context.ContentType = "text/html";   
            }

            if (!string.IsNullOrWhiteSpace(context.Get("__standalone"))) {
                script = @"
<html>
<header>
</header>
<body>
<script type=""text/javascript"" src=""../scripts/jquery.min.js""></script>
<script type=""text/javascript"" src=""../scripts/FusionCharts.js""></script>
<script type=""text/javascript"" src=""../scripts/FusionCharts.HC.js""></script>
<script type=""text/javascript"" src=""../scripts/FusionCharts.HC.Charts.js""></script>
" + script + @"
</body>
</html>";
            }

            context.Output.Write(script);
        }

        private IChartConfig PrepareChartConfig(IMvcContext context) {
            var result = new ChartConfig {
                Id = context.Get("__id", DateTime.Now.Ticks).ToString(CultureInfo.InvariantCulture),
                Container = context.Get("__container", string.Empty),
                Width = context.Get("__width", "400"),
                Height = context.Get("__height", "300"),
                Debug = context.Get("__debug", "0"),
                Type = context.Get("__type", "Column2D"),
                Divlines = context.Get("__divlines", -1)
            };
            var specAttrs = context.GetAll("fc");
            foreach (var attr in specAttrs) {
                result.Set(attr.Key, attr.Value);
            }
            return result;
        }

        /// <summary>
        /// Render "set data" part of script 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private string RenderDataScript(IMvcContext context, IChartConfig config) {
            if (context.ActionResult is XElement) {
                var xElement = context.ActionResult as XElement;
                config.DataType = "XML";
                config.Type = xElement.Attribute("graphtype").Value;
                return (context.ActionResult as XElement).ToString();
            }

            return string.Empty;
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
