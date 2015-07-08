using Qorpent.IoC;

namespace Qorpent.Security.SslCa.CaConfigProvider {
	/// <summary>
	///		Провайдер конфига CA
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton, "qorpent.ca.configprovider.default", ServiceType = typeof(ICaConfigProvider))]
	public class DefaultCaConfigProvider : ICaConfigProvider {
		/// <summary>
		///		Провайдер конфига CA
		/// </summary>
		/// <returns>Конфиг CA</returns>
		public CaConfig GetConfig() {
			var config = new CaConfig {
				BaseDir = "/srv",
				DataStorageDir = "/srv/ca-storage",
				UserCertExtension = "crt",
				UserKeyExtension = "key"
			};
			return config;
		}
	}
}
