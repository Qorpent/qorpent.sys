using System.Xml.Linq;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Интерфейс построителя объектов для BSharp
	/// </summary>
	public interface IBSharpRuntimeActivatorService : IBSharpRuntimeActivator {
		/// <summary>
		/// Порядковый номер при обходе
		/// </summary>
		int Index { get; set; }
		/// <summary>
		/// Проверяет поддержку сериализации
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		bool CanActivate<T>(IBSharpRuntimeClass rtcls, BSharpActivationType acivationType = BSharpActivationType.Default);
	}
}