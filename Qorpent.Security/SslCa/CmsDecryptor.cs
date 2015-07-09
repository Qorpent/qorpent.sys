using System;
using System.Diagnostics;
using System.IO;

namespace Qorpent.Security.SslCa {
	/// <summary>
	///		Декриптор сообщений CMS
	/// </summary>
	public class CmsDecryptor {
		/// <summary>
		///		Используемый конфиг
		/// </summary>
		public CaConfig CaConfig { get; private set; }
		/// <summary>
		///		Инициализация декриптора конфигов
		/// </summary>
		/// <param name="config">Конфиг</param>
		public void Initialize(CaConfig config) {
			CaConfig = config;
		}
		/// <summary>
		///		Дешифровка сообщения
		/// </summary>
		/// <param name="message">Сообщение фотмата CMS</param>
		/// <returns>Расшифрованная строка</returns>
		public string Descrypt(CmsMessage message) {
			if (CaConfig == null) {
				throw new Exception("Not initialized");
			}
			if (string.IsNullOrWhiteSpace(message.EncryptedMessage)) {
				throw new ArgumentException("Empty message");
			}
			var provider = new CaProvider();
			provider.Initialize(CaConfig);
			var realMsg = RefineMessage(message);
			var crtPath = provider.GetUserCertPath(message.CertificateFingerprint);
			var keyPath = provider.GetUserKeyPath(message.CertificateFingerprint);
			var cmsPath = Path.GetTempFileName();
			var argPath = Path.GetTempFileName();
			File.WriteAllText(cmsPath, realMsg);
			var arguments = string.Format("smime -decrypt -in {0} -recip {1} -inkey {2} -inform PEM", cmsPath, crtPath, keyPath);
			File.WriteAllText(argPath, arguments);
			var startInfo = new ProcessStartInfo {
				FileName = CaConst.OpenSslProcess,
				Arguments = arguments,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false
			};
			var process = new Process {StartInfo = startInfo};
			process.Start();
			var output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			return output;
		}
		/// <summary>
		///		Преобразование исходного сообщения в требуемый формат
		/// </summary>
		/// <param name="message">Сообщение</param>
		/// <returns>Обработанное сообщение</returns>
		public string RefineMessage(CmsMessage message) {
			return "-----BEGIN PKCS7-----\n" + message.EncryptedMessage + "\n-----END PKCS7-----";
		}
	}
}