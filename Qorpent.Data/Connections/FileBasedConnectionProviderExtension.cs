using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data {

	/// <summary>
	/// Сохраняет строки подключения к БД в локальном каталоге /usr приложения
	/// </summary>
	[ContainerComponent(Lifestyle.Extension, "file.based.connection.registry-persister",
		ServiceType = typeof (IDatabaseConnectionProviderExtension))]
	public class FileBasedConnectionRegistryPersister : ServiceBase, IDatabaseConnectionRegistryExtension {
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

		public void Register(ConnectionDescriptor connectionDescriptor) {
			var filename = GetFileName(connectionDescriptor.Name);
			var str = string.Format("{0} '{1}' '{2}'", connectionDescriptor.Name, connectionDescriptor.ConnectionType.FullName,
			                        connectionDescriptor.ConnectionString);
			File.WriteAllText(filename,str);
		}
	}

	/// <summary>
	/// Считывает информацию из файлов в формате BXL/XML c расширением *.db-connection
	/// </summary>
	[ContainerComponent( Lifestyle.Extension,"file.based.connection.provider",ServiceType = typeof(IDatabaseConnectionProviderExtension))]
	public class FileBasedConnectionProviderExtension:ServiceBase,IDatabaseConnectionProviderExtension {
		/// <summary>
		/// Проводник файловой системы
		/// </summary>
		[Inject] public IFileNameResolver Files { get; set; }

		/// <summary>
		/// Bxl parser
		/// </summary>
		[Inject] public IBxlParser Bxl { get; set; }

		/// <summary>
		/// Считывает соединения из всех файлов с расширением *.db-connection
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ConnectionDescriptor> GetConnections() {
			var connectiFiles = Directory.GetFiles(Files.Root, "*.db-connection", SearchOption.AllDirectories);
			IList<XElement> xcons = new List<XElement>();
			foreach (var connectiFile in connectiFiles) {
				var content = File.ReadAllText(connectiFile);
				if(content.StartsWith("<")) {
					xcons.Add(XElement.Parse(content));
					continue;
				}
				if(null==Bxl) {
					throw new QorpentException("cannot use bxl due to it is not found");
				}
				xcons.Add(Bxl.Parse(content,connectiFile));
			}
			foreach (var xcon in xcons) {
				var set = new[] {xcon};
				if(xcon.Elements().Any()) {
					set = xcon.Elements().ToArray();
				}
				foreach (var e in set) {
					var desc = new ConnectionDescriptor { Name = e.Name.LocalName, ConnectionString = e.Attr("name") };
					var connectiontype = e.Attr("code");
					if (desc.ConnectionString.IsEmpty() && connectiontype.IsNotEmpty())
					{
						desc.ConnectionString = connectiontype;
						connectiontype = "auto";
					}
					if (connectiontype == "mssql" || connectiontype == "auto")
					{
						desc.ConnectionType = typeof(SqlConnection);
					}
					else
					{
						desc.ConnectionType = Type.GetType(connectiontype);
					}
					yield return desc;
				}
				
			}
		}
	}
}