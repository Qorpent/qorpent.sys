using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Graphs;
using Qorpent.Graphs.Dot;
using Qorpent.IoC;
using Qorpent.Mvc.Binding;
using Qorpent.Serialization;

namespace Qorpent.Mvc.Actions
{
    /// <summary>
    /// Действие отрисовки скрипта на DOT в виде SVG
    /// </summary>
    [Action("_sys.renderdot")]
    public class RenderDotAction:ActionBase
    {
        [Bind(Required = true,IsLargeText = true)]private string Script { get; set; }
        [Inject(Name = "dot.graph.provider",Required = true)]private IGraphProvider Provider { get; set; }
        /// <summary>
        /// Возвращает XML, содержащий SVG с графиком
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess() {
            if (!Script.Contains("digraph")) {
                Script = "digraph G{\r\n" + Script + "\r\n}";
            }
            Script = Escape(Script);
            return XElement.Parse((string)Provider.Generate(Script, new GraphOptions()));
        }

        private string Escape(string script) {
            return DotLanguageUtils.EscapeScript(script);
        }
    }
}
