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
    }
}