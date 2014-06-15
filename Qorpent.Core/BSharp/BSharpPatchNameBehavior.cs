using System;

namespace Qorpent.BSharp{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum BSharpPatchNameBehavior
	{
		/// <summary>
		/// 
		/// </summary>
		None = 0,
		/// <summary>
		/// ошибка при попытке создания элемента
		/// </summary>
		Free = 1,
		/// <summary>
		/// Игнорирование новых элементов
		/// </summary>
		Match = 2,
		/// <summary>
		/// Значение по умолчанию для поведения - ошибка (контроль целостности целевого класса)
		/// </summary>
		Default = Free,

	}
}