namespace Qorpent.Serialization {
	/// <summary>
	///     Интерфейс для пользовательских языков селектора
	/// </summary>
	public interface ICustomSelectorImpl : ISelectorImpl {
		/// <summary>
		///     Возвращает true, если переданный образец запроса относится к данному типу пользовательского селектора
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		bool IsSupported(string query);
	}
}