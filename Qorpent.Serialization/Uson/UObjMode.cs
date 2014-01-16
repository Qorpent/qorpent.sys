using System;

namespace Qorpent.Uson
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum UObjMode
	{
		/// <summary>
		/// Обычный
		/// </summary>
		Default = 0,
		/// <summary>
		/// Массив
		/// </summary>
		Array = 1,
		/// <summary>
		/// Информация об исходном типе
		/// </summary>
		Fake = 2,
	}
}