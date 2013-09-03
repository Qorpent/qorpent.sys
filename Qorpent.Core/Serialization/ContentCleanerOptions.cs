using System;

namespace Qorpent.Serialization {
	/// <summary>
	/// Класс опций очистки контента
	/// </summary>
	public class ContentCleanerOptions {
		private static ContentCleanerOptions _default = new ContentCleanerOptions();
		/// <summary>
		/// 
		/// </summary>
		public ContentCleanerOptions() {
			Operations = ContentCleanerOperations.Default;
		}
		/// <summary>
		/// 
		/// </summary>
		public ContentCleanerOperations Operations { get; set; }
		/// <summary>
		/// Опции по умолчанию
		/// </summary>
		public static ContentCleanerOptions Default {
			get { return _default; }
			set { _default = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string BaseUrl { get; set; }
	}
}