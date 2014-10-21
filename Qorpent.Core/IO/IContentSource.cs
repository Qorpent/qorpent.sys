namespace Qorpent.IO{
	/// <summary>
	/// Обобщенный интерфейс источника контента
	/// </summary>
	public interface IContentSource{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		string GetString(string url);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		byte[] GetData(string url);
		/// <summary>
		///		Разрешает результирующий адрес документа
		/// </summary>
		/// <param name="uri">Исходный адрес документа</param>
		/// <returns>Результирующий адрес документа</returns>
		string ResolveUri(string uri);
	}
}