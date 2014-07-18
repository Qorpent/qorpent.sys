using System;
using Qorpent.Log;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	///		Расширения для <see cref="IUserLog"/>
	/// </summary>
	public static class LogExtensions {
		/// <summary>
		///		Запись сообщения о фатальной ошибке
		/// </summary>
		/// <param name="log">Экземпляр лога</param>
		/// <param name="message">Сообщение</param>
		/// <param name="format">Данные для форматированного вывода</param>
		public static void WriteFatal(this IUserLog log, string message, params string[] format) {
			log.Write(LogLevel.Fatal, message, format: format);
		}
		/// <summary>
		///		Запись сообщения
		/// </summary>
		/// <param name="log">Экземпляр лога</param>
		/// <param name="message">Сообщение</param>
		/// <param name="format">Данные для форматированного вывода</param>
		public static void WriteInfo(this IUserLog log, string message, params string[] format) {
			log.Write(LogLevel.Info, message, format:format);
		}
		/// <summary>
		///		Запись сообщения об ошибке
		/// </summary>
		/// <param name="log">Экземпляр лога</param>
		/// <param name="message">Сообщение</param>
		/// <param name="format">Данные для форматированного вывода</param>
		public static void WriteError(this IUserLog log, string message, params string[] format) {
			log.Write(LogLevel.Error, message, format: format);
		}
		/// <summary>
		///		Запись форматированного сообщения в лог
		/// </summary>
		/// <param name="log">Экземпляр лога</param>
		/// <param name="level">Уровень логгирования</param>
		/// <param name="message">Сообщение</param>
		/// <param name="host">Хост-объект</param>
		/// <param name="setup">Действие для дополнительной донастройки</param>
		/// <param name="substitute">Замещение для интерполяции</param>
		/// <param name="format">Замещение для <see cref="string.Format(string,object)"/></param>
		public static void Write(this IUserLog log, LogLevel level, string message, object host = null, Action<LogMessage> setup = null, object substitute = null, string[] format = null) {
			var logMessage = new LogMessage {HostObject = host, Level = level};
			var realMessage = message;
			if (null != substitute) {
				var si = new StringInterpolation();
				realMessage = si.Interpolate(realMessage, substitute);
			} else if (null != format) {
				realMessage = string.Format(realMessage, (string[]) format);
			}
			logMessage.Message = realMessage;
			if (null != setup) {
				setup(logMessage);
			}
			log.Write(logMessage);
		}
	}
}
