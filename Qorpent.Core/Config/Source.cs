namespace Qorpent.Config {
	/// <summary>
	///		Класс абстрактного источника
	/// </summary>
	public abstract class Source {
		/// <summary>
		///		Абстрактный источник
		/// </summary>
		private object _source;
		/// <summary>
		///		Установка источника
		/// </summary>
		/// <param name="source">Источник</param>
		public void SetSource(object source) {
			_source = source;
		}
		/// <summary>
		///		Типизированное получение источника
		/// </summary>
		/// <typeparam name="T">Типизация получаемого источника</typeparam>
		/// <returns>Типизированный источник</returns>
		public T GetSource<T>() {
			return (T) _source;
		}
	}
}