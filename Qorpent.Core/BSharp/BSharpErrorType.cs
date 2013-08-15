 using System;

namespace Qorpent.BSharp {
	/// <summary>
	/// Перечисление типов ошибки
	/// </summary>
	public enum BSharpErrorType {
		/// <summary>
		/// Дублирующиеся имена классов
		/// </summary>
		DuplicateClassNames =100 ,
		/// <summary>
		/// Класс был создан из override-конструкции
		/// </summary>
		ClassCreatedFormOverride = 1010,
		/// <summary>
		/// Класс был создан из конструкции расширения
		/// </summary>
		ClassCreatedFormExtension =1020,
		/// <summary>
		/// Целевая тема при импорте оказалась "сиротской"
		/// </summary>
		OrphanImport =210,


		/// <summary>
		/// Ошибка неразрешенного импорта
		/// </summary>
		NotResolvedImport = 220,
		/// <summary>
		/// Циклический импорт
		/// </summary>
		RecycleImport = 230,
		/// <summary>
		/// У инклуда вообще не указан код
		/// </summary>
		FakeInclude = 3010,
		/// <summary>
		/// Не найдена тема, которая могла бы быть применена в инклуд
		/// </summary>
		NotResolvedInclude = 320,
		/// <summary>
		/// При инклуде темы в режиме body ее контент оказался пустым
		/// </summary>
		EmptyInclude = 3030,

		
	}
}