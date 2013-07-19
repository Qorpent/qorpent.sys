using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="objectToRender"></param>
        protected override void SendOutput(IMvcContext context, object objectToRender) {
            var sw = new StringWriter();
            GetMainSerializer().Serialize("result", objectToRender, sw);
            
            var xwriter = new FullEndingXmlTextWriter(context.Output);
            var x = XElement.Parse(sw.ToString());
            x.WriteTo(xwriter);
            
        }
        /// <summary>
        /// 
        /// </summary>
        public class FullEndingXmlTextWriter : XmlTextWriter {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="w"></param>
            public FullEndingXmlTextWriter(TextWriter w)
                : base(w) {
            }
            /// <summary>
            /// /
            /// </summary>
            /// <param name="w"></param>
            /// <param name="encoding"></param>
            public FullEndingXmlTextWriter(Stream w, Encoding encoding)
                : base(w, encoding) {
            }
            /// <summary>
            /// /
            /// </summary>
            /// <param name="fileName"></param>
            /// <param name="encoding"></param>
            public FullEndingXmlTextWriter(string fileName, Encoding encoding)
                : base(fileName, encoding) {
            }
            /// <summary>
            /// 
            /// </summary>
            public override void WriteEndElement() {
                this.WriteFullEndElement();
            }
        }
    }
}