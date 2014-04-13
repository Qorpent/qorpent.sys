using System;
using System.Globalization;
using System.Xml.Linq;
using Qorpent.Charts;
using Qorpent.IO;
using Qorpent.Uson;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    /// Render для отрисовки графиков Fusionchart
    /// </summary>
    [Render("chart")]
    public  class ChartRender: RenderBase {
        /// <summary>
        /// Renders given context
        /// </summary>
        /// <param name="context"></param>
        public override void Render(IMvcContext context) {

	        IChartConfig config = PrepareChartConfig(context);
	        var script = string.Empty;

	        if (config.State.IsNormal) {
		        script = GetNormalChartContent(context, config);
	        } else {
		        script = GetErrorChartContent(context, config);
	        }

	        context.Output.Write(script);
        }

	    private string GetErrorChartContent(IMvcContext context, IChartConfig config) {
			if (context.Get("format", context.Get("__format")) == "json")
			{
				dynamic result = new UObj();
				result.config = config;			
				result.error = config.State.Message;
				context.ContentType = MimeHelper.JSON;
				return result.ToJson();
			}
		    var id = "g"+Guid.NewGuid().ToString().Replace("-", "");
		    var script = @"<div class='chart_Error chart_Error_'"+config.State.Level+"'>";
			if (!string.IsNullOrWhiteSpace(config.State.Title)){
				script += "<h3>" + config.State.Title + "</h3>";
			} else if (null != config.State.Exception) {
				script += "<p>Произошла ошибка " + config.State.Exception.GetType().Name + "</p>"; 
			}
			if (!string.IsNullOrWhiteSpace(config.State.Message)) {
				script += "<p>" + config.State.Message.Replace("\r\n", "<br/>") + "</p>";
			} else if (null != config.State.Exception) {
				script += "<p>" + config.State.Exception.Message.Replace("\r\n", "<br/>") + "</p>";
			}
			if (null != config.State.Exception){
				script += "<button onclick='$(\"#" + id + "\").toggle()'>Показать подробности ошибки</button>";
				script += "<BR /><textarea rows='30' cols='120' id='" + id + "' style='display:none'>" + config.State.Exception + "</textarea>";
			}
		    script += "</div>";
			if (!string.IsNullOrWhiteSpace(context.Get("standalone")))
			{
				script = @"
<html>
<header>
</header>
<body>
<script type=""text/javascript"" src=""../scripts/jquery.min.js""></script>
" + script + @"
</body>
</html>";
			}
		    return script;
	    }

	    private string GetNormalChartContent(IMvcContext context, IChartConfig config) {
		    var datascript = RenderDataScript(context, config);
			if (string.IsNullOrWhiteSpace(datascript)) {
				return GetErrorChartContent(context, config);
			}
		    var script = "";
		    var error = string.Empty;

		    if (string.IsNullOrWhiteSpace(datascript)) {
			    error = "Нет данных для отображения";
		    }

		    if (context.Get("format", context.Get("__format")) == "json") {
			    dynamic result = new UObj();
			    result.config = config;
			    result.data = datascript;
			    result.error = error;
			    context.ContentType = MimeHelper.JSON;
				return result.ToJson().Replace("\\\'", "'");
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

		    if (!string.IsNullOrWhiteSpace(context.Get("standalone"))) {
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
		    return script;
	    }

	    private IChartConfig PrepareChartConfig(IMvcContext context) {
            if (context.ActionResult is IChartConfig) {
				return context.ActionResult as IChartConfig;
			}
			if (context.ActionResult is Exception) {
				return ChartConfig.Create(context.ActionResult as Exception);
			}


			var result = new ChartConfig {
                Id = context.Get("id", DateTime.Now.Ticks).ToString(CultureInfo.InvariantCulture),
                Container = context.Get("container", string.Empty),
                Width = context.Get("width", "400"),
                Height = context.Get("height", "300"),
                Debug = context.Get("debug", "0"),
                Type = context.Get("type", "Column2D"),
                Divlines = context.Get("divlines", -1)
            };
            var specAttrs = context.GetAll("fc");
            foreach (var attr in specAttrs) {
                result.Set(attr.Key, attr.Value);
            }



		    if (context.ActionResult is ChartState) {
			    result.State = context.ActionResult as ChartState;
		    } else {
			    result.NativeResult = context.ActionResult;
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
	        var source = config.NativeResult;
			if (null == source) {
				config.State.Message = "Отсутствуют данные для отрисовки";
				config.State.Level = ErrorLevel.Error;
				return null;
			}
	        if (source is XElement) {
		        var xElement =source as XElement;
		        config.DataType = "XML";
		        config.Type = xElement.Attribute("graphtype").Value;
		        return source.ToString();
	        } else if(source is string) {
		        return source as string;
	        }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="context"></param>
        public override void RenderError(Exception error, IMvcContext context) {
            var config = ChartConfig.Create(error);
	        var script = GetErrorChartContent(context, config);
	        context.StatusCode = 500;
			context.Output.Write(script);
        }
    }
}
