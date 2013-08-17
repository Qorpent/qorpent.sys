namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Базовый интерфейс активатора
	/// </summary>
	public interface IBSharpRuntimeActivator {
		/// <summary>
		/// Создать типизированный объект из динамического объекта BSharp
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Activate<T>(IBSharpRuntimeClass rtcls, BSharpActivationType acivationType = BSharpActivationType.Default);
	}
}