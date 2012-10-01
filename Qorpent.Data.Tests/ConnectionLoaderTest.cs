using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.IO;
using Qorpent.IoC;

namespace Qorpent.Data.Tests
{
	[TestFixture]
	public class ConnectionLoaderTest
	{
		[Test]
		public void FileBasedResolutionWorks() {
			var container = getcontainer();
			container.GetLoader().Load<FileBasedConnectionProviderExtension>();
			var files = container.Get<IFileNameResolver>();
			var file = files.Resolve(FileSearchQuery.Leveled("~/usr/mycon.db-connection"));
			Directory.CreateDirectory(Path.GetDirectoryName(file));
			File.WriteAllText(file,"con1 'Data Source=(local1);Initial Catalog=db;'\r\ncon2 'Data Source=(local2);Initial Catalog=db2;'");
			var conp = container.Get<IDatabaseConnectionProvider>();
			var c1 = conp.GetConnection("con1");
			Assert.AreEqual("Data Source=(local1);Initial Catalog=db;", c1.ConnectionString);
			var c2 = conp.GetConnection("con2");
			Assert.AreEqual("Data Source=(local2);Initial Catalog=db2;", c2.ConnectionString);
		}

		[Test]
		public void ContainerBasedResolutionWorks()
		{
			var container = getcontainer();
			container.GetLoader().Load<ContainerBasedConnectionProviderExtension>();
			var connection1 = new SqlConnection("Data Source=(local1);Initial Catalog=db;");
			var connection2 = new SqlConnection("Data Source=(local2);Initial Catalog=db2;");
			container.Register(container.NewExtension<IDbConnection>(connection1, "con1.connection"));
			container.Register(container.NewExtension<IDbConnection>(connection2, "con2.connection"));

			var conp = container.Get<IDatabaseConnectionProvider>();

			var c1 = conp.GetConnection("con1");
			Assert.AreEqual("Data Source=(local1);Initial Catalog=db;", c1.ConnectionString);
			var c2 = conp.GetConnection("con2");
			Assert.AreEqual("Data Source=(local2);Initial Catalog=db2;", c2.ConnectionString);
		}

		private static Container getcontainer() {
			var container = new Container();
			var loader = container.GetLoader();
			loader.Load<FileNameResolver>();
			loader.Load<DatabaseConnectionProvider>();
			loader.Load<BxlParser>();
			return container;
		}
	}
}
