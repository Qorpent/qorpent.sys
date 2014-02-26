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
		/// <param name="fullMatch">Проверка на полное соответствие (IsAssignable если false)</param>
		/// <returns>Признак соответствия типизации</returns>
		public bool TagIs<T>(bool fullMatch = false) {
			if (Tag == null) {
				return false;
			}
			if (fullMatch) {
				return Tag.GetType() == typeof (T);
			}
			return Tag.GetType().IsAssignableFrom(typeof (T));
		}
	}
}
