using System;

namespace Qorpent.Data {
	/// <summary>
	/// Расширение проводника соединений
	/// </summary>
	public interface IDatabaseConnectionRegistryExtension {
		
		/// <summary>
		/// Снимает строку подключения с регистрации
		/// </summary>
		/// <param name="name"></param>
		void Unregister(string name );
		/// <summary>
		/// Зарегистрировать соединение с БД
		/// </summary>
		
		void Register(ConnectionDescriptor connectionDescriptor);
	}
}