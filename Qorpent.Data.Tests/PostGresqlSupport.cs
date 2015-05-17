using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Data.Connections;

namespace Qorpent.Data.Tests
{

    [TestFixture]
    [Explicit]
    public class PostGresqlSupport
    {
        const string connectionString = "ProviderName=NPGSQL;Server=127.0.0.1;Port=5432;Database=test;User Id=test;Password=123;";
        [Test]
        public void CanCreateConnection() {
            var dcp = new DatabaseConnectionProvider();
            var connection = dcp.GetConnection(connectionString);
            Assert.AreEqual("NpgsqlConnection", connection.GetType().Name);
        }

        [Test]
        public void CanOpenConnection() {
            var dcp = new DatabaseConnectionProvider();
            using (var c = dcp.GetConnection(connectionString)) {
                c.Open();
            }
        }
    }
}
