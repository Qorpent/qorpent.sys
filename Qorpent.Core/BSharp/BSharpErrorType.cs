 using System;

namespace Qorpent.BSharp {
	/// <summary>
	/// Перечисление типов ошибки
	/// </summary>
	public enum BSharpErrorType {
		/// <summary>
		/// Дублирующиеся имена классов
		/// </summary>
		DuplicateClassNames =1 ,
		/// <summary>
		/// Целевая тема при импорте оказалась "сиротской"
		/// </summary>
		OrphanImport =2,
		/// <summary>
		/// Ошибка неразрешенного импорта
		/// </summary>
		NotResolvedImport = 3,
	}
}