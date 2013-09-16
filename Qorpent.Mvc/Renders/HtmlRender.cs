using System.IO;
using System.Xml.Linq;
using Qorpent.Serialization;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    /// ������ ��� ������ ����������� � HTML ������ XML, ��� �� ���� ������� ������������� �� ���������� (Q-14)
    /// </summary>
    [Render("html")]
    public class HtmlRender : XmlRender {
        /// <summary>
        /// 	����� ���� ����� ����� ��� XmlReader, �� MIME ������
        /// </summary>
        /// <returns> </returns>
        protected override string GetContentType()
        {
            return "text/html";
        }

        /// <summary>
        /// ����������� ������ � XML � ����� ������������ ���, ��������� ����� <see cref="XHtml5XmlWriter"/>
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