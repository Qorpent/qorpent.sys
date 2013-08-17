namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Интерфейс сервера Runtime инфраструктуры BSharp
	/// </summary>
	public interface IBSharpRuntimeProviderService : IBSharpRuntimeProvider
	{
		/// <summary>
		/// Возвращает типизированный сериализованный объект IBSharp
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		T Get<T>(string name) where T:class;


	}
}