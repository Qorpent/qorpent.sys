namespace Qorpent.Security.SslCa {
	/// <summary>
	///		Сообщение CMS
	/// </summary>
	public class CmsMessage {
		/// <summary>
		///		Закриптованное сообщение
		/// </summary>
		public string EncryptedMessage { get; set; }
		/// <summary>
		///		Отпечаток сертификата получателя
		/// </summary>
		public string CertificateFingerprint { get; set; }
	}
}