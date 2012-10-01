using System.Collections.Generic;
using System.Data;
using System.Linq;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.Connections {
	/// <summary>
	/// —читывает информацию из контейнера по типу IDbConnection
	/// </summary>
	[ContainerComponent(Lifestyle.Extension, "container.based.connection.provider",
		ServiceType = typeof (IDatabaseConnectionProviderExtension))]
	public class ContainerBasedConnectionProviderExtension : ServiceBase, IDatabaseConnectionProviderExtension {
		/// <summary>
		/// ќбходит компоненты контейнера и находит все IDbConnection
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ConnectionDescriptor> GetConnections() {
			foreach (var source in Container.GetComponents().Where(x=>x.ServiceType==typeof(IDbConnection))) {
				if(source.Name.IsEmpty()) continue;
				var con = new ConnectionDescriptor();
				con.Name = source.Name.Split('.', '/')[0];
				con.InstantiateWithContainer = true;
				con.Container = this.Container;
				con.ContainerName = source.Name;
				yield return con;
			}
		}
	}
}