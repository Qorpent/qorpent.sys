using System;

namespace Qorpent.Serialization {
	/// <summary>
	/// Допустимые операции XML - очистки HTML
	/// </summary>
	/// <remarks>Используется в <see cref="IContentCleaner"/></remarks>
	[Flags]
	public enum ContentCleanerOperations {
		/// <summary>
		/// Без очистки
		/// </summary>
		None = 1,
		/// <summary>
		/// Удаление опасных тегов
		/// </summary>
		RemoveBadTags = 1<<1,
		/// <summary>
		/// Удаление лишних атрибутов
		/// </summary>
		RemoveBadAttributes = 1<<2,
		/// <summary>
		/// Замена таблиц дивами
		/// </summary>
		RewriteTables = 1<<3,
		/// <summary>
		/// Замена адресов картинок на абсолютные
		/// </summary>
		FixImageUrls = 1<<4,
		/// <summary>
		/// Сделать обычные ссылки абсолютными
		/// </summary>
		FixHrefUrls = 1<<5,
		/// <summary>
		/// Установить дополнительные классы 
		/// </summary>
		SetupPositionClasses = 1<<6,
		/// <summary>
		/// Все операции
		/// </summary>
		All = RemoveBadTags | RemoveBadAttributes | RewriteTables |FixImageUrls |FixHrefUrls |SetupPositionClasses,
		/// <summary>
		/// Опции по умолчанию
		/// </summary>
		Default = All,
		/// <summary>
		/// Неопределенные опции
		/// </summary>
		Undefined = 1<<32,
	}
}