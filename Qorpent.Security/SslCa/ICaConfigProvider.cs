namespace Qorpent.Security.SslCa {
	/// <summary>
	///		Провайдер конфига CA
	/// </summary>
	public interface ICaConfigProvider {
		/// <summary>
		///		Провайдер конфига CA
		/// </summary>
		/// <returns>Конфиг CA</returns>
		CaConfig GetConfig();
	}
}