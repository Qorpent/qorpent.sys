using System;

namespace Qorpent.Host.Exe.Security{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum AuthProtocolResponseType:byte{
		/// <summary>
		/// Пустой
		/// </summary>
		None =0,
		/// <summary>
		/// Положительный результат
		/// </summary>
		True = 1,
		/// <summary>
		/// Отрицательный результат
		/// </summary>
		False =2 ,
		/// <summary>
		/// Ошибка
		/// </summary>
		Error = 4,
		/// <summary>
		/// Токен (при аутентификации)
		/// </summary>
		Token = 8,
		/// <summary>
		/// Сигнатура 
		/// </summary>
		Sygnature = 16,
		/// <summary>
		/// Статусная информация
		/// </summary>
		State = 32,
	}
}