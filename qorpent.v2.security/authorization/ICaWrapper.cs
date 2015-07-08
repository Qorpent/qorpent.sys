using qorpent.v2.security.user;

namespace qorpent.v2.security.authorization {
	/// <summary>
	///		Враппер к центру авторизации
	/// </summary>
	public interface ICaWrapper {
		/// <summary>
		///		Получение сессионной соли относительно идентификатора сертификата
		/// </summary>
		/// <param name="certId">Идентификатор сертификата</param>
		/// <returns>Сессионная соль</returns>
		string GetSalt(string certId);
		/// <summary>
		///		Проведение процесса авторизации
		/// </summary>
		/// <param name="certId">Идентификатор сертификата</param>
		/// <param name="encryptedSalt">Зашифрованная соль</param>
		/// <returns>Авторизованный пользователь</returns>
		IUser ProcessAuth(string certId, string encryptedSalt);
	}
}