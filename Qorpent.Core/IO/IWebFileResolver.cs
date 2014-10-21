namespace Qorpent.IO{
	/// <summary>
	/// Интерфейс розольвера файлов уровня приложения
	/// </summary>
	public interface IWebFileResolver{
		/// <summary>
		/// Осуществляет поиск описателя файла по условию
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		IWebFileRecord Find(string search);
		/// <summary>
		/// Очищает кэш резольвера
		/// </summary>
		void Clear();
		/// <summary>
		/// Регистрирует провайдера файлов
		/// </summary>
		/// <param name="provider"></param>
		void Register(IWebFileProvider provider);
		/// <summary>
		/// Общий префикс приложения резольвера
		/// </summary>
		string Prefix { get; set; }
	}
}