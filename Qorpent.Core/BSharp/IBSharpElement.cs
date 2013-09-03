namespace Qorpent.BSharp {
	/// <summary>
	/// Интерфейс определения элемента класса
	/// </summary>
	public interface IBSharpElement {
		/// <summary>
		/// </summary>
		string Name { get; set; }

		/// <summary>
		///     Имя цели мержинга (рут)
		/// </summary>
		string TargetName { get; set; }

		/// <summary>
		///     Тип импорта
		/// </summary>
		BSharpElementType Type { get; set; }
	}
}