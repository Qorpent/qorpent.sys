using System;

namespace Qorpent.IntermediateFormat {
	/// <summary>
	///		Описание слоёв документа промежуточного формата
	/// </summary>
	[Flags]
	public enum IntermediateFormatLayer:ulong {
	
		/// <summary>
		///		Строка
		/// </summary>
		Row = 1,
		/// <summary>
		///		Колонка
		/// </summary>
		Col = 2,
		/// <summary>
		///		Особый слой (пересечение <see cref="Row"/> и <see cref="Col"/>)
		/// </summary>
		SpecialRowCol = 4,
		/// <summary>
		///		Слой данных
		/// </summary>
		Data =8 ,
		/// <summary>
		///		Слой настроек
		/// </summary>
		Settings=16,
		/// <summary>
		///		Слой информация
		/// </summary>
		Info=32,
		/// <summary>
		/// Отладочная информация
		/// </summary>
		Debug = 64,
		/// <summary>
		/// Документ
		/// </summary>
		StandaloneDocument = 128,
		
	}
}
