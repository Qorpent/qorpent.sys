namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Запрос на класс
	/// </summary>
	public class ClassQuery {
		/// <summary>
		/// Ограничение на Namespace
		/// </summary>
		public string Namespace { get; set; }
		/// <summary>
		/// Ограничение на конкретный класс
		/// </summary>
		public string ClassName { get; set; }
		/// <summary>
		/// Ограничение на прототип
		/// </summary>
		public string Prototype { get; set; }

		/// <summary>
		/// Ограничение на базовый класс
		/// </summary>
		public string BaseClass { get; set; }
	}
}