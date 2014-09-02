namespace Qorpent.IO.Net{
	/// <summary>
	/// Интерфейс ридера протокола HTTP
	/// </summary>
	public interface IHttpReader{
		/// <summary>
		///     Считывает следую
		/// </summary>
		/// <returns></returns>
		HttpEntity Next();
	}
}