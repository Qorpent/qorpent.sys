namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Описывает класс, совместимый с BSharp runtime
	/// </summary>
	public interface IBSharpRuntimeBound {
		/// <summary>
		/// Инициализирует объект на биндинг с рантайм-классом
		/// </summary>
		/// <param name="cls"></param>
		void Initialize(IBSharpRuntimeClass cls);
	}
}