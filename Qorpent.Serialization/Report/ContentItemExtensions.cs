using System.IO;

namespace Qorpent.Report{
	/// <summary>
	/// 
	/// </summary>
	public static class ContentItemExtensions {
		/// <summary>
		///		Получение контентной строки из переданного <see cref="IContentItem"/>
		/// </summary>
		/// <param name="contentItem">Имплементация <see cref="IContentItem"/></param>
		/// <returns>Контентная строка</returns>
		public static string GetContentString(this IContentItem contentItem){
			var sw = new StringWriter();
			contentItem.Render(sw);
			return sw.ToString();
		}
	}
}