using System;
using System.Xml.Linq;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    /// Render для отрисовки графиков Fusionchart
    /// </summary>
    [Render("fc")]
    public class FusionChartRender: RenderBase {
        /// <summary>
        /// 	Renders given context
        /// </summary>
        /// <param name="context"></param>
        public override void Render(IMvcContext context) {
            var render = context.Get("render", "javascript");
            var type = context.Get("graph", "Column2D");
            var id = context.Get("id", "fc-graph-" + DateTime.Now.Ticks);
            var container = context.Get("container", "");
            var width = context.Get("width", "400");
            var height = context.Get("height", "300");
            var debug = context.Get("debug", "0");
            var script = string.Format(@"<script type=""text/javascript""><!--
    FusionCharts.setCurrentRenderer('{0}');
    var myChart = new FusionCharts('{1}', '{2}', '{3}', '{4}', '{5}');
    {7};
    myChart.render('{6}');
// -->
</script>", render, type, id, width, height, debug, container, RenderDataScript(context));
            context.ContentType = "text/html";
            context.Output.Write(script);
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
            throw new NotImplementedException();
        }
    }
}
