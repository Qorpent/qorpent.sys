using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using NUnit.Framework;
using Qorpent.Data;

namespace Qorpent.Core.Tests.Data
{
    [TestFixture]
    public class NoDbQueryExecutorTest {
        protected FakeConnectionProvider Cp;
        protected DbCommandExecutor E;
        protected DbCommandWrapper C;

        [SetUp]
        public void Setup() {
            Cp = new FakeConnectionProvider();
            E = new DbCommandExecutor {ConnectionProvider = Cp};
            C = new DbCommandWrapper {Query = "/**/", NoExecute = true};
        }

        [Test]
        public void CanSetUpFromMinimumInfo() {

            E.Execute(C).Wait();
           
        }

        [Test]
        public void InitilizeDefaultConnectionString()
        {
            E.Execute(C).Wait();
            Assert.AreEqual("default", C.ConnectionString);
            Assert.NotNull(C.Connection);
            Assert.True(C.Connection.ConnectionString.Contains("default"));
        }

        [Test]
        public void InitilizeConnection() {
            C.ConnectionString = "test";
            E.Execute(C).Wait();
            Assert.NotNull(C.Connection);
            Assert.True(C.Connection.ConnectionString.Contains("test"));
        }

        [Test]
        public void DontInitilizeConnectionIfSetup()
        {
            C.Connection= new SqlConnection("Server=(local);Database=XXX;Trusted_Connection=true");
            E.Execute(C).Wait();
            Assert.NotNull(C.Connection);
            Assert.True(C.Connection.ConnectionString.Contains("XXX"));
        }

        [Test]
        public void ErrorOnNotSetConnection() {
            Cp.Registry["test"] = "";
            C.ConnectionString = "test";
            E.Execute(C).Wait();
            Assert.False(C.Ok);
            Assert.NotNull(C.Error);
        }

        [Test]
        public void GeneratesPreparedCommand() {
            E.Execute(C).Wait();
            Assert.NotNull(C.PreparedCommand);
            Assert.AreEqual(C.Query,C.PreparedCommand.CommandText);
        }

        [Test]
        public void DontRecreatePreparedCommand() {
            C.PreparedCommand = new SqlCommand("/*1*/");
            E.Execute(C).Wait();
            Assert.NotNull(C.PreparedCommand);
            Assert.AreEqual("/*1*/", C.PreparedCommand.CommandText);
        }

        [Test]
        public void DeterminesDialectFromConnection() {
            Assert.AreEqual(SqlDialect.None,C.Dialect);
            E.Execute(C).Wait();
            Assert.AreEqual(SqlDialect.SqlServer, C.Dialect);
        }

        [Test]
        public void RetrieveParametersFromQuery() {
            C.Query = "select * from x where Id = @id or Code = @id";
            E.Execute(C).Wait();
            Assert.NotNull(C.Parameters);
            Assert.AreEqual(1,C.Parameters.Length);
            
            var p = C.Parameters[0];
            Assert.AreEqual("id",p.Name);
            var sp =(SqlParameter) C.PreparedCommand.Parameters[0];
            Assert.AreEqual(default(DbType),p.DbType);
        }

        [Test]
        public void BindParametersFromSource() {
            C.Query = "select * from x where Id = @id or Code = @id";
            C.ParametersSoruce = new{Id = 23};
            E.Execute(C).Wait();
            var p = C.Parameters[0];
            Assert.AreEqual("id", p.Name);
            Assert.AreEqual(23, p.Value);
            Assert.AreEqual(default(DbType), p.DbType);
        }

        [Test]
        public void CanSetupValidProcedureCall() {
            C.Query = "";
            C.ObjectName = "test";
            C.ObjectType = SqlObjectType.Procedure;
            C.Parameters = new[] {
                new DbParameter {Name = "id"},
                new DbParameter {Name = "name"},
            };
            E.Execute(C).Wait();
            Assert.AreEqual("EXEC test @id=@id, @name=@name",C.Query);
        }

        [Test]
        public void CanSetupValidFunctionCall()
        {
            C.Query = "";
            C.ObjectName = "test";
            C.ObjectType = SqlObjectType.Function;
            C.Parameters = new[] {
                new DbParameter {Name = "id"},
                new DbParameter {Name = "name"},
            };
            E.Execute(C).Wait();
            Assert.AreEqual("SELECT test(@id, @name)", C.Query);
        }
        [Test]
        public void CanSetupValidTableFunctionCall()
        {
            C.Query = "";
            C.ObjectName = "test";
            C.ObjectType = SqlObjectType.TableFunction;
            C.Parameters = new[] {
                new DbParameter {Name = "id"},
                new DbParameter {Name = "name"},
            };
            E.Execute(C).Wait();
            Assert.AreEqual("SELECT * FROM test(@id, @name)", C.Query);
        }

        public class ParametersProxy : IDbCommandExecutor {
            public async Task<DbCommandWrapper> Execute(DbCommandWrapper info) {
                info.Result = new object[] {
                    new Dictionary<string, object> {{"name", "id"}, {"type", "int"}},
                    new Dictionary<string, object> {{"name", "name"}, {"type", "nvarchar"}},
                };
                return info;
            }
        }

        [Test]
        public void CanDetermineParametersByCallingMetadataInfoFromServer()
        {
            E.ProxyExecutor = new ParametersProxy();
            C.Query = "";
            C.ObjectName = "test";
            C.ObjectType = SqlObjectType.Procedure;
            E.Execute(C).Wait();
            Assert.AreEqual(DbType.Int64,C.Parameters[0].DbType);
            Assert.AreEqual("EXEC test @id=@id, @name=@name", C.Query);
        }


        public class ObjectTypeProxy : IDbCommandExecutor
        {
            public async Task<DbCommandWrapper> Execute(DbCommandWrapper info) {
                info.Result = "TF";
                return info;
            }
        }
        [Test]
        public void CanDetermineObjectTypeFromServer()
        {
            E.ProxyExecutor = new ObjectTypeProxy();
            C.Query = "";
            C.ObjectName = "test";
            C.Parameters = new[] {
                new DbParameter {Name = "id"},
                new DbParameter {Name = "name"},
            };
            E.Execute(C).Wait();
            Assert.AreEqual(SqlObjectType.TableFunction,C.ObjectType);
            Assert.AreEqual("SELECT * FROM test(@id, @name)", C.Query);
        }

    }
}
