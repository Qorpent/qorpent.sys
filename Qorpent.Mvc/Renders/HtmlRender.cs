using System.IO;
using System.Xml.Linq;
using Qorpent.Serialization;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    /// Рендер для выдачи результатов в HTML вместо XML, сам по себе никаких трансформаций не производит (Q-14)
    /// </summary>
    [Render("html")]
    public class HtmlRender : XmlRender {
        /// <summary>
        /// 	Ведет себя точно также как XmlReader, но MIME другой
        /// </summary>
        /// <returns> </returns>
        protected override string GetContentType()
        {
            return "text/html";
        }

        /// <summary>
        /// Преобразует объект в XML и затем отрисовывает его, пропуская через <see cref="XHtml5XmlWriter"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="objectToRender"></param>
        protected override void SendOutput(IMvcContext context, object objectToRender) {
            var xml = GetMainSerializer().Serialize( objectToRender, "result");
            var x = XElement.Parse(xml);
            var xwriter = new XHtml5XmlWriter(context.Output);   
            x.WriteTo(xwriter);
        }
    }
}