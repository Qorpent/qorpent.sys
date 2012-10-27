using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Data.Connections;
using Qorpent.Events;
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
			File.WriteAllText(file,"con1 mssql 'Data Source=(local1);Initial Catalog=db;'\r\ncon2 'Data Source=(local2);Initial Catalog=db2;'");
			var conp = container.Get<IDatabaseConnectionProvider>();
			var c1 = conp.GetConnection("con1");
			Assert.AreEqual("Data Source=(local1);Initial Catalog=db;", c1.ConnectionString);
			var c2 = conp.GetConnection("con2");
			Assert.AreEqual("Data Source=(local2);Initial Catalog=db2;", c2.ConnectionString);
			File.Delete(file);
		}

		[Test]
		public void FilePostgresResolutionWorks()
		{
			var container = getcontainer();
			container.GetLoader().Load<FileBasedConnectionProviderExtension>();
			var files = container.Get<IFileNameResolver>();
			var file = files.Resolve(FileSearchQuery.Leveled("~/usr/mycon.db-connection"));
			Directory.CreateDirectory(Path.GetDirectoryName(file));
			File.WriteAllText(file, "con1 pgsql 'Server=127.0.0.1;Port=5432;Integrated Security=True;Database=zetatest;'");
			var conp = container.Get<IDatabaseConnectionProvider>();
			var c1 = conp.GetConnection("con1");
			c1.Open();
			Console.WriteLine(c1.ConnectionString);

			Assert.IsInstanceOf<Npgsql.NpgsqlConnection>(c1);
			File.Delete(file);
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

		[Test]
		public void NonPersistentRegister_And_Reset() {
			var container = getcontainer();
			var conp = container.Get<IDatabaseConnectionProvider>();
			Assert.Null(conp.GetConnection("con1"));
			conp.Register(new ConnectionDescriptor{Name = "con1",ConnectionString = "Data Source=(local1);Initial Catalog=db;" 
				,ConnectionType = typeof(SqlConnection)},false );
			Assert.NotNull(conp.GetConnection("con1"));
			((IResetable) conp).Reset(null);
			Assert.Null(conp.GetConnection("con1"));

		}

		[Test]
		public void PersistentRegister_And_Reset()
		{
			var container = getcontainer();
			container.GetLoader().Load<FileBasedConnectionRegistryPersister>();
			container.GetLoader().Load<FileBasedConnectionProviderExtension>();

			var conp = container.Get<IDatabaseConnectionProvider>();
			conp.UnRegister("con1",true);
			((IResetable)conp).Reset(null);
			Assert.Null(conp.GetConnection("con1"));
			conp.Register(new ConnectionDescriptor
			{
				Name = "con1",
				ConnectionString = "Data Source=(local1);Initial Catalog=db;"
				,
				ConnectionType = typeof(SqlConnection)
			}, true);
			Assert.NotNull(conp.GetConnection("con1"));
			((IResetable)conp).Reset(null);
			Assert.NotNull(conp.GetConnection("con1"));

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
