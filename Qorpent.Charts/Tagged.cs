namespace Qorpent.Utils {
	/// <summary>
	///		Абстрактный клас сущности, помеченной тегом
	/// </summary>
	public abstract class Tagged {
		/// <summary>
		///		Тег
		/// </summary>
		public object Tag { get; set; }
		/// <summary>
		///		Типизированное получение тега
		/// </summary>
		/// <typeparam name="T">Типизация возвращаемого объекта</typeparam>
		/// <returns>Типизированный объект</returns>
		public T GetTag<T>() {
			return (T) Tag;
		}
		/// <summary>
		///		Определяет соответствие тега типизации
		/// </summary>
		/// <typeparam name="T">Предполагаемая типизация тега</typeparam>
		/// <returns>Признак соответствия типизации</returns>
		public bool TagIs<T>() {
			return Tag is T;
		}
	}
}
