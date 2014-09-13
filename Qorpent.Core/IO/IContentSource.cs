namespace Qorpent.IO.Net{
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
	}
}