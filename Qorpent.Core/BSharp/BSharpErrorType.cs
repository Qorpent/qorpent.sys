 using System;

namespace Qorpent.BSharp {
	/// <summary>
	/// Перечисление типов ошибки
	/// </summary>
	public enum BSharpErrorType {

		/// <summary>
		/// Общая ошибка
		/// </summary>
		GenericError =10,
		/// <summary>
		/// Дублирующиеся имена классов
		/// </summary>
		DuplicateClassNames =100 ,
		/// <summary>
		/// Класс-сирота
		/// </summary>
		OrphanClass = 110,
		/// <summary>
		/// Класс был создан из override-конструкции
		/// </summary>
		ClassCreatedFormOverride = 1010,
		/// <summary>
		/// Класс был создан из конструкции расширения
		/// </summary>
		ClassCreatedFormExtension =1020,

		/// <summary>
		/// Ошибка неразрешенного импорта
		/// </summary>
		NotResolvedImport = 210,
		/// <summary>
		/// Целевая тема при импорте оказалась "сиротской"
		/// </summary>
		OrphanImport =220,

		/// <summary>
		/// Циклический импорт
		/// </summary>
		RecycleImport = 230,

		/// <summary>
		/// Импорт игнорируемого объекта
		/// </summary>
		IgnoredImport = 240,


		
		/// <summary>
		/// Не найдена класс для инклуда
		/// </summary>
		NotResolvedInclude = 320,

		/// <summary>
		/// Попытка включить класс-сироту
		/// </summary>
		OrphanInclude = 330,

		/// <summary>
		/// У инклуда вообще не указан код
		/// </summary>
		FakeInclude = 3010,
		
		/// <summary>
		/// При инклуде класса в режиме body ее контент оказался пустым
		/// </summary>
		EmptyInclude = 3030,

		/// <summary>
		/// Не разрешенная ссылка на класс
		/// </summary>
		NotResolvedClassReference = 410,
		/// <summary>
		/// Неоднозначкая ссылка на класс
		/// </summary>
		AmbigousClassReference = 420,
		/// <summary>
		/// Ссылка на класс разрешена просто по имени
		/// </summary>
		NotDirectClassReference = 4010,
        /// <summary>
        /// Не найден словарь
        /// </summary>
	    NotResolvedDictionary = 430,

        /// <summary>
        /// Не найден словарь
        /// </summary>
        NotResolvedDictionaryElement = 440,

		/// <summary>
		/// У патча не указана цель
		/// </summary>
		PatchUndefinedTarget = 510,


		/// <summary>
		/// У патча указано неверное поведение
		/// </summary>
		PatchError = 520,
	}
}