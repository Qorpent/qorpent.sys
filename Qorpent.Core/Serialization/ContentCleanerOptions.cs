using System;

namespace Qorpent.Serialization {
	/// <summary>
	/// Класс опций очистки контента
	/// </summary>
    /// <remarks>Используется в <see cref="IContentCleaner"/></remarks>
	public class ContentCleanerOptions {
		private static ContentCleanerOptions _default = new ContentCleanerOptions();
		/// <summary>
		/// Создает настройки очистки со стандартными опциями
		/// </summary>
		public ContentCleanerOptions() {
			Operations = ContentCleanerOperations.Default;
		}
		/// <summary>
		/// Набор операций очистки контента
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
		/// Базовый URL для <see cref="ContentCleanerOperations.FixHrefUrls"/> и <see cref="ContentCleanerOperations.FixImageUrls"/>
		/// </summary>
		public string BaseUrl { get; set; }
	}
}