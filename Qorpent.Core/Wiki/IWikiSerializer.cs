namespace Qorpent.Wiki {
	/// <summary>
	/// Интерфейс рендера Wiki
	/// </summary>
	public interface IWikiSerializer {
		/// <summary>
		/// Отрисовывает переданный текстовой контент в виде Wiki
		/// </summary>
		/// <param name="usage"></param>
		/// <param name="page"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		string ToHTML(string usage, WikiPage page, object context);
	}
}