namespace Qorpent.Host.Exe.Security{
	/// <summary>
	/// 
	/// </summary>
	public enum AuthProtocolRequestType:byte{
		/// <summary>
		/// Нет запроса
		/// </summary>
		None = 0,
		/// <summary>
		/// Считывает статус сервера
		/// </summary>
		Status = 1,
		/// <summary>
		/// Запрос сигнатуры для защищенного соедин
		/// </summary>
		Sygnature = 2,
		/// <summary>
		/// Аутентификация с открытым именем-паролем
		/// </summary>
		AuthBasic = 4,
		/// <summary>
		/// Аутентификация с именем и хэшем пароля
		/// </summary>
		AuthDigest = 8,
		/// <summary>
		/// Проверка ранее выданного токена
		/// </summary>
		CheckToken = 16,
		/// <summary>
		/// Пролонгация действия токена
		/// </summary>
		UpgradeToken = 32,
		/// <summary>
		/// Сброс токена
		/// </summary>
		DropToken = 64,
		/// <summary>
		/// Аутентификация
		/// </summary>
		Auth = AuthBasic|AuthDigest,
		/// <summary>
		/// Запрос состояния
		/// </summary>
		State = Status|Sygnature,
		/// <summary>
		/// Операции с токенами
		/// </summary>
		Token = UpgradeToken|CheckToken|DropToken,
	}
}