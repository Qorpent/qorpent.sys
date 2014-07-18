using System;

namespace Qorpent.IntermediateFormat {
	/// <summary>
	///		Описание слоёв документа промежуточного формата
	/// </summary>
	[Flags]
	public enum IntermediateLayerFormat {
		/// <summary>
		///		Неопределённый слой
		/// </summary>
		Undefined = 0,
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
		Special = 4,
		/// <summary>
		///		Слой данных
		/// </summary>
		Data,
		/// <summary>
		///		Слой настроек
		/// </summary>
		Settings,
		/// <summary>
		///		Слой информация
		/// </summary>
		Info,
		/// <summary>
		///		По умолчанию <see cref="Undefined"/>
		/// </summary>
		Default = Undefined
	}
}
