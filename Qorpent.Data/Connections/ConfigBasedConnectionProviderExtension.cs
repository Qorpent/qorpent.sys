using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Qorpent.IoC;

namespace Qorpent.Data.Connections {
	/// <summary>
	/// —читывает информацию из штатного конфига
	/// </summary>
	[ContainerComponent(Lifestyle.Extension, "config.based.connection.provider",ServiceType = typeof (IDatabaseConnectionProviderExtension))]
	public class ConfigBasedConnectionProviderExtension : ServiceBase, IDatabaseConnectionProviderExtension {
		/// <summary>
		/// —читывает соединени€ из штатного конфига
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ConnectionDescriptor> GetConnections() {
			var connections = ConfigurationManager.ConnectionStrings;
			foreach (ConnectionStringSettings connection in connections) {
				var con = new ConnectionDescriptor
					{Name = connection.Name, ConnectionString = connection.ConnectionString, ConnectionType = typeof (SqlConnection)};
				con.Evidence = "config:" + connection.ElementInformation.Source + ":" + connection.ElementInformation.LineNumber;
				yield return con;
			}
		}
	}
}