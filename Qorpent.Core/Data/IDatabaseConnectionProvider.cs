using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Qorpent.Data
{
	/// <summary>
	/// Фабрика строк подключений и самих подключений, с поддержкой настройки
	/// </summary>
	public interface IDatabaseConnectionProvider {
		/// <summary>
		/// Получить соединение по имени
		/// </summary>
		/// <param name="name">Имя соединения</param>
		/// <returns>Содениение</returns>
		IDbConnection GetConnection(string name);
		/// <summary>
		/// Получить строку подключения по имени
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string GetConnectionString(string name);

		/// <summary>
		/// Зарегистрировать новое соединение
		/// </summary>
		
		void Register(ConnectionDescriptor connectionDescriptor, bool persistent);
		/// <summary>
		/// Удалить строку из регистратора
		/// </summary>
		/// <param name="name"></param>
		/// <param name="persistent"></param>
		void UnRegister(string name, bool persistent);

		/// <summary>
		/// Проверяет наличие подключения у поставщика
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		bool Exists(string name);

		/// <summary>
		/// Возвращает список всех имеющихся конфигураций подключений
		/// </summary>
		/// <returns></returns>
		IEnumerable<ConnectionDescriptor> Enlist();
	}
}
