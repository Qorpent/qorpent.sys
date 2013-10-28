using System;
using System.Net;

namespace Qorpent.Security {
    /// <summary>
    /// Интерфейс встроенного хранилища пар имя-пароль для веб
    /// </summary>
    public interface ICredentialStorage {
        /// <summary>
        /// Получить креденции для указанного сервера и приложения
        /// </summary>
        /// <param name="node"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        ICredentials GetCredentials(string node = "127.0.0.1", string app="default");
		/// <summary>
		/// Возвращает имя-пароль в незащищенном виде
		/// </summary>
		/// <param name="node"></param>
		/// <param name="app"></param>
		/// <returns></returns>
	    Tuple<string,string> GetUnsafeCredentials(string node = "127.0.0.1", string app = "default");
        /// <summary>
        /// Установить креденции для указанного сервера и приложения
        /// </summary>
        /// <param name="node"></param>
        /// <param name="app"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        void SetCredentials(string node, string app, string username, string password);
    }
}