namespace Qorpent.Host.Utils
{
	/// <summary>
	/// Скаченный файл
	/// </summary>
	public class PostFile
	{
		/// <summary>
		/// Тип контента
		/// </summary>
		public string ContentType { get; set; }
		/// <summary>
		/// Имя на форме
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Имя файла
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// Контент
		/// </summary>
		public byte[] Content { get; set; }
	}
}
