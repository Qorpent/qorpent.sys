using System;

namespace Qorpent.BSharp{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum BSharpPatchBehavior{
		/// <summary>
		/// 
		/// </summary>
		None = 0,
		/// <summary>
		/// ошибка при попытке создания элемента
		/// </summary>
		ErrorOnNew = 1,
		/// <summary>
		/// Игнорирование новых элементов
		/// </summary>
		NoneOnNew = 2,
		/// <summary>
		/// Создание элементов при патче
		/// </summary>
		CreateOnNew = 4,

		/// <summary>
		/// Признак некорректного типа
		/// </summary>
		Invalid = 8,
		/// <summary>
		/// Значение по умолчанию для поведения - ошибка (контроль целостности целевого класса)
		/// </summary>
		Default = ErrorOnNew,

	}
}