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
    }
}