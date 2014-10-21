namespace Qorpent.IO{
	/// <summary>
	/// Интерфейс провайдера файлов для веб-приложения
	/// </summary>
	public interface IWebFileProvider{
		/// <summary>
		/// Найти файл с указанным именем
		/// </summary>
		/// <param name="file"></param>
		/// <param name="searchMode"></param>
		/// <returns></returns>
		IWebFileRecord Find(string file, WebFileSerachMode searchMode = WebFileSerachMode.Exact);
		/// <summary>
		/// Проверяет допустимость поиска для данного провайдера
		/// </summary>
		/// <param name="nsearch"></param>
		/// <returns></returns>
		bool IsMatch(string nsearch);
		/// <summary>
		/// Префикс имен файлов для определения локальных путей
		/// </summary>
		string Prefix { get; set; }
	}
}