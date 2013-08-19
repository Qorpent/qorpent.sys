namespace Qorpent.BSharp.Runtime {
	/// <summary>
	///     Интерфейс сервера Runtime инфраструктуры BSharp
	/// </summary>
	public interface IBSharpRuntimeService : IBSharpRuntimeProvider, IBSharpRuntimeActivator {
		/// <summary>
		///     Активирует сервис по имени класса
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="activationType"></param>
		/// <returns></returns>
		T Activate<T>(string name, BSharpActivationType activationType = BSharpActivationType.Default);
	}
}