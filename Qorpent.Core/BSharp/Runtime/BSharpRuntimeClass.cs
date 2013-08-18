using System.Xml.Linq;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	///     Базовая реализация BSharp класса времени выполнения
	/// </summary>
	public class BSharpRuntimeClass : IBSharpRuntimeClass {
		/// <summary>
		///     Имя класса
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Пространство имен
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		///     Полное имя
		/// </summary>
		public string Fullname { get; set; }

		/// <summary>
		///     Определение
		/// </summary>
		public XElement Definition { get; set; }
	}
}