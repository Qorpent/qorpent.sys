using System;
using System.IO;
using System.Linq;

namespace Qorpent.Security.SslCa {
	/// <summary>
	///		Провайдер CA
	/// </summary>
	public class CaProvider {
		/// <summary>
		///		Используемый конфиг
		/// </summary>
		public CaConfig CaConfig { get; private set; }
		/// <summary>
		///		Инициализация провайдера конфигом
		/// </summary>
		/// <param name="config">Конфиг</param>
		public void Initialize(CaConfig config) {
			CaConfig = config;
		}
		/// <summary>
		///		Определяет путь к данным пользователя по отпечатку сертефиката
		/// </summary>
		/// <param name="certFingerprint">Отпечаток сертификата</param>
		/// <returns>Результирующий путь</returns>
		public string GetUserDataPath(string certFingerprint) {
			if (CaConfig == null) {
				throw new Exception("Not initialized");
			}
			var dataDir = CaConfig.DataStorageDir;
			var fpParts = certFingerprint.Split(':');
			if (fpParts.Length == 1) {
				throw new ArgumentException("Incorrect certificate fingerprint");
			}
			var fisrt = fpParts[0];
			var second = fpParts[1];
			var result = Path.Combine(dataDir, fisrt, second, certFingerprint.Replace(":", ""));
			return result;
		}
		/// <summary>
		///		Определяет полный путь до сертификата пользователя по отпечатку сертификата
		/// </summary>
		/// <param name="certFingerprint">Отпечаток сертификата</param>
		/// <returns>Полный путь к пользовательскому сертификату</returns>
		public string GetUserCertPath(string certFingerprint) {
			if (CaConfig == null) {
				throw new Exception("Not initialized");
			}
			var dataPath = GetUserDataPath(certFingerprint);
			if (!Directory.Exists(dataPath)) {
				return string.Empty;
			}
			var cert = Directory.GetFiles(dataPath, "*." + CaConfig.UserCertExtension).FirstOrDefault();
			if (string.IsNullOrWhiteSpace(cert)) {
				return string.Empty;
			}
			return cert;
		}
		/// <summary>
		///		Определяет полный путь до секретного ключа пользователя по отпечатку сертификата
		/// </summary>
		/// <param name="certFingerprint">Отпечаток сертификата</param>
		/// <returns>Полный путь до приватного ключа пользователя</returns>
		public string GetUserKeyPath(string certFingerprint) {
			if (CaConfig == null) {
				throw new Exception("Not initialized");
			}
			var dataPath = GetUserDataPath(certFingerprint);
			if (!Directory.Exists(dataPath)) {
				return string.Empty;
			}
			var key = Directory.GetFiles(dataPath, "*." + CaConfig.UserKeyExtension).FirstOrDefault();
			if (string.IsNullOrWhiteSpace(key)) {
				return string.Empty;
			}
			return key;
		}
	}
}
