using System;

namespace Qorpent.BSharp {
	/// <summary>
	/// Перечисление типов ошибки
	/// </summary>
	[Flags]
	public enum BSharpErrorType {
		/// <summary>
		/// Дублирующиеся имена классов
		/// </summary>
		DuplicateClassNames =1 ,
	}
}