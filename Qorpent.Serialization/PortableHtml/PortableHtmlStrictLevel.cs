using System;

namespace Qorpent.PortableHtml{
	/// <summary>
	///     Уровень ограничений схем при разборе
	/// </summary>
	[Flags]
	public enum PortableHtmlStrictLevel
	{
		/// <summary>
		/// Default, no any options added
		/// </summary>
		Strict = 0,
		/// <summary>
		///     Разрешает недоверенные источники картинок
		/// </summary>
		TrustAllImages = 1,

		/// <summary>
		///    Разрешает недоверенные источники ссылок
		/// </summary>
		TrustAllLinks = 2,

		/// <summary>
		/// Разрешает таблицы
		/// </summary>
		AllowTables  = 4,
		/// <summary>
		/// Разрешает списки
		/// </summary>
		AllowLists = 8,
		/// <summary>
		/// Разрешает разрывы строк
		/// </summary>
		AllowBr = 16,

		/// <summary>
		/// Обобщает подключение расширенных элементоы
		/// </summary>
		AllowExtensions = AllowTables|AllowLists|AllowBr,

		/// <summary>
		/// Разрешить атрибуты кроме обязательных href/src и phtml_*
		/// </summary>
		AllowUnknownAttributes = 32,

	}
}