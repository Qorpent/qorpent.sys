using System.Collections.Generic;

namespace Qorpent.Data {
	/// <summary>
	/// Расширение проводника соединений
	/// </summary>
	public interface IDatabaseConnectionProviderExtension {
		/// <summary>
		/// Возвращает коллекцию зарегистрированных данным расширением соединений
		/// </summary>
		/// <returns></returns>
		IEnumerable<ConnectionDescriptor> GetConnections();
		
	}
}