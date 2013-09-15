using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Graphs;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    ///     Render для отрисовки графиков DOT
    /// </summary>
    [Render("dot")]
    public class DotRender : RenderBase {

	    [Inject(Name = "dot.graph.provider")] private IGraphProvider Provider { get; set; }
        [Inject(Name = "dot.serializer")]private ISerializer DotSerializer { get; set; }
        
        /// <summary>
        ///     Параметр указания целевого формата, по умолчанию SVG
        /// </summary>
        public const string FORMATPARAM = "__format";

        /// <summary>
        ///     Параметр указания алгоритма по умолчанию DOT
        /// </summary>
        public const string ALGORITHMPARAM = "__algorithm";

        /// <summary>
        ///     Параметр включения тюнинга графа
        /// </summary>
        public const string TUNEPARAM = "__tune";

        /// <summary>
        ///     Префикс парамтеров перекрытия настрок графа
        /// </summary>
        public const string OVERRIDEPARAMPREFIX = "g.";



        /// <summary>
        ///     Renders given context
        /// </summary>
        /// <param name="context"> </param>
        public override void Render(IMvcContext context) {
            GraphOptions options = ExtractOptions(context);
            string dotscript = DotSerializer.Serialize(options.Context.ActionResult, options: options);
	        context.ContentType = MimeHelper.GetMimeByExtension(options.Format);
            string script = dotscript.GetUnicodeSafeXmlString();

            if (options.Format == "dot") {
                context.ContentType = "text/plain";
                context.Output.Write(script);
            }
            else if (options.Format == "rawxml") {
                WriteOutRawXml(context,options);
            }
            else {
	            GenerateGraph(context, dotscript, options);
            }
        }

	    private void GenerateGraph(IMvcContext context, string dotscript, GraphOptions options) {
		    var result = Provider.Generate(dotscript, options);
		    if (result is string) {
			    if (options.Context.ActionResult is IGraphSource) {
				    XElement xml = XElement.Parse((string) result);
				    xml = ((IGraphSource) options.Context.ActionResult).PostprocessGraphSvg(xml, options);
				    context.Output.Write(xml.ToString());
			    }
			    else {
				    context.Output.Write((string) result);
			    }
		    }
		    else {
			    context.WriteOutBytes((byte[]) result);
		    }
	    }

	    private void WriteOutRawXml(IMvcContext context, GraphOptions options) {
            var val = context.ActionResult;
            context.ContentType = MimeHelper.GetMimeByExtension("xml");
            if (val is IGraphSource) {
                val = ((IGraphSource) val).BuildGraph(options);
            }
            var xmls = Application.Container.Get<ISerializer>("xml.serializer");
            xmls.Serialize("dot.xml", val, context.Output);

        }

        private static GraphOptions ExtractOptions(IMvcContext context) {
            string format = context.Get(FORMATPARAM, GraphOptions.SVGFORMAT);
            string algorithm = context.Get(ALGORITHMPARAM, GraphOptions.DOTAGORITHM);
            bool tune = context.Get(TUNEPARAM, true);
            IEnumerable<KeyValuePair<string, string>> overrides = context.GetAll(OVERRIDEPARAMPREFIX);
            var options = new GraphOptions {
                Tune = tune,
                Algorithm = algorithm,
                Format = format,
                Dialect = GraphOptions.DOTDIALECT
            };
            foreach (var o in overrides) {
                options.OverrideGraphAttributes[o.Key] = o.Value;
            }
            options.Context = context;
            return options;
        }


	    /// <summary>
        ///     Renders error, occured in given context
        /// </summary>
        /// <param name="error"> </param>
        /// <param name="context"> </param>
        public override void RenderError(Exception error, IMvcContext context) {
            context.ContentType = "text/plain";
            context.Output.Write(error.ToString());
        }
    }
}