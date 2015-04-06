namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// Тип селектирующей комманды SQL
	/// </summary>
	public enum SqlCommandType {
		/// <summary>
		/// Вызов процедуры (SELECT func, EXEC proc)
		/// </summary>
		Call,
		/// <summary>
		/// Получение табличной функции EXEC proc, SELECT * FROM func
		/// </summary>
		Select,
	}
}