using System;

namespace Qorpent.IntermediateFormat {
	/// <summary>
	///		Формат вывода ZIF
	/// </summary>
	[Flags]
	public enum IntermediateFormatOutputType {
		/// <summary>
		///		Неопределённое состояние
		/// </summary>
		Undefined = 0,
		/// <summary>
		///		XML
		/// </summary>
		Xml = 1,
		/// <summary>
		///		BXL
		/// </summary>
		Bxl = 2,
		/// <summary>
		///		JSON
		/// </summary>
		Json = 4,
		/// <summary>
		///		Значение по умолчанию <see cref="Xml"/>
		/// </summary>
		Default = Xml
	}
}