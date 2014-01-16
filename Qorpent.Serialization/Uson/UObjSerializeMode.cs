using System;

namespace Qorpent.Uson
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum UObjSerializeMode
	{
		/// <summary>
		/// Отсутствуют
		/// </summary>
		None = 0,
		
		/// <summary>
		/// Информация об исходном типе
		/// </summary>
		KeepType =1,

		
	}
}