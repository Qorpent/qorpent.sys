using System.Xml.Linq;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Интерфейс построителя объектов для BSharp
	/// </summary>
	public interface IBSharpRuntimeBuilder {
		/// <summary>
		/// Порядковый номер при обходе
		/// </summary>
		int Index { get; set; }
		/// <summary>
		/// Проверяет поддержку сериализации
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		bool IsSupported<T>();
		/// <summary>
		/// Создать типизированный объект из динамического объекта BSharp
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Create<T>(XElement src);
	}
}