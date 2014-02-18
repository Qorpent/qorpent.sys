namespace Qorpent{
	/// <summary>
	/// Описывает состояние фильтра на выходе
	/// </summary>
	public enum FilterState{
		/// <summary>
		/// Не определенный
		/// </summary>
		None,

		/// <summary>
		/// Игнор
		/// </summary>
		Ignored,
		/// <summary>
		/// Выполнено
		/// </summary>
		Processed,
		/// <summary>
		/// Финиш - дальнейшие фильтры отбрасываются
		/// </summary>
		Finished
	}
}