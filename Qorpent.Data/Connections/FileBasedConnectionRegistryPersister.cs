using System.IO;
using Qorpent.IO;
using Qorpent.IoC;

namespace Qorpent.Data.Connections {
	/// <summary>
	/// Сохраняет строки подключения к БД в локальном каталоге /usr приложения
	/// </summary>
	[ContainerComponent(Lifestyle.Extension, "file.based.connection.registry-persister",
		ServiceType = typeof (IDatabaseConnectionRegistryExtension))]
	public class FileBasedConnectionRegistryPersister : ServiceBase, IDatabaseConnectionRegistryExtension {
		/// <summary>
		/// Снимает строку подключения с регистрации (удаляет файл)
		/// </summary>
		/// <param name="name"></param>
		public void Unregister(string name) {
			var filename = GetFileName(name);
			if(File.Exists(filename)) {
				File.Delete(filename);
			}
		}

		private string GetFileName(string name) {
			var filename =
				Container.Get<IFileNameResolver>().Resolve(FileSearchQuery.Leveled("~/usr/" + name + ".fbcrp.db-connection"));
			return filename;
		}

		/// <summary>
		/// Зарегистрировать соединение с БД (создает файл)
		/// </summary>
		public void Register(ConnectionDescriptor connectionDescriptor) {
			var filename = GetFileName(connectionDescriptor.Name);
			var str = string.Format("{0} '{1}' '{2}'", connectionDescriptor.Name, connectionDescriptor.ConnectionType.AssemblyQualifiedName,
			                        connectionDescriptor.ConnectionString);
			File.WriteAllText(filename,str);
		}
	}
}