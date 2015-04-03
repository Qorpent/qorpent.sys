using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Qorpent.Data;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests.Data {
    [TestFixture]
    public class DbCommandExecutorTest {
        private DbCommandExecutor E;
        private DbCommandWrapper C;
        TraceListener L = new ConsoleTraceListener();
        private const string Connection = "Server=(local);Database=tempdb;Trusted_Connection=true;";
        private const string NoDatabaseNameConnection = "Server=(local);Trusted_Connection=true;";
        [TestFixtureSetUp]
        public void PrepareDatabase() {
            using (var c = new SqlConnection(Connection)) {
                c.Open();
                c.ExecuteNonQuery(@"
IF OBJECT_ID ('test') IS NOT NULL DROP PROCEDURE test;
");
                c.ExecuteNonQuery(@"
CREATE PROCEDURE test @id int, @str nvarchar(255), @type nvarchar(255) AS BEGIN
    SET @type = ISNULL(@type, 'SCALAR');
    IF @str is not null print @str +' hella!';
    IF @type = 'SCALAR' SELECT @id * 2;
    IF @type = 'SINGLE' SELECT @id * 2 as id, @str + ' hella' as name;
    IF @type = 'NULL' RETURN;
    IF @type = 'READER' 
        SELECT @id * 2 as id, @str + ' hella' as name UNION
        SELECT @id * 3 as id, @str + ' bella' ;
    IF @type = 'MULTIPLE' BEGIN
        SELECT @id * 2 as id, @str + ' hella' as name UNION
        SELECT @id * 3, @str + ' bella' ;

        SELECT @id * 4 as id2, @str + ' zella' as name2 UNION
        SELECT @id * 5, @str + ' vella' ;
    END;
    
END;
");
                L.TraceOutputOptions = TraceOptions.None;
                Trace.Listeners.Add(L);
            }
        }

        [TestFixtureTearDown]
        public void FixtureTearDown() {
            Trace.Listeners.Remove(L);
        }

        [SetUp]
        public void Setup() {
            E = new DbCommandExecutor();
            C = new DbCommandWrapper {ConnectionString = Connection, ObjectName = "test"};
        }

        [Test]
        public void CanDetermineMetadata() {
            C.NoExecute = true;
            E.Execute(C).Wait();
            if (null != C.Error) {
                Console.WriteLine(C.Error.ToString());
            }
            Assert.True(C.Ok);
            Assert.AreEqual(SqlDialect.SqlServer,C.Dialect);
            Assert.AreEqual(SqlObjectType.Procedure,C.ObjectType);
            Assert.AreEqual(3, C.Parameters.Length);
        }

        [Test]
        public void Scalar() {
            C.Notation = DbCallNotation.Scalar;
            C.ParametersSoruce = new {id = 2, type = "SCALAR"};
            E.Execute(C).Wait();
            Assert.AreEqual(4,C.Result);
        }

        [Test]
        public void DatabaseNameInContext() {
            C.ConnectionString = NoDatabaseNameConnection;
            C.Database = "tempdb";
            C.Notation = DbCallNotation.Scalar;
            C.ParametersSoruce = new { id = 2, type = "SCALAR" };
            E.Execute(C).Wait();
            Assert.AreEqual(4, C.Result);
        }

        [Test]
        public void ClonedPreparedVersionSupport()
        {
            C.Notation = DbCallNotation.Scalar;
            C.ParametersSoruce = new { id = 2, type = "SCALAR" };
            E.Execute(C).Wait();
            Assert.AreEqual(4, C.Result);
            C = C.Clone(new {id = 4, type = "SCALAR"});
            E.Execute(C).Wait();
            Assert.AreEqual(8,C.Result);
        }

        [Test]
        public void SingleRow()
        {
            C.Notation = DbCallNotation.SingleRow;
            C.ParametersSoruce = new { id = 2, str="x", type = "SINGLE" };
            E.Execute(C).Wait();
            Assert.IsInstanceOf<Dictionary<string,object>>(C.Result);
            var dict = (Dictionary<string, object>) C.Result;
            Assert.AreEqual(4,dict["id"]);
            Assert.AreEqual("x hella", dict["name"]);
        }

        [Test]
        public void NullSingleRow() {
            C.Notation = DbCallNotation.SingleRow;
            C.ParametersSoruce = new { id = 2, str = "x", type = "NULL" };
            E.Execute(C).Wait();
            Assert.Null(C.Error);
            Assert.True(C.Ok);
            Assert.Null(C.Result);
        }

        class A {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        class A2 {

            public int Id2 { get; set; }
            public string Name2 { get; set; }
        }
        [Test]
        public void SingleObject()
        {
            C.Notation = DbCallNotation.SingleObject;
            C.TargetType = typeof (A);
            C.ParametersSoruce = new { id = 2, str = "x", type = "SINGLE" };
            E.Execute(C).Wait();
            Assert.IsInstanceOf<A>(C.Result);
            var a = (A)C.Result;
            Assert.AreEqual(4, a.Id);
            Assert.AreEqual("x hella", a.Name);
        }

        [Test]
        public void Reader() {
            C.Notation = DbCallNotation.Reader;
            C.ParametersSoruce = new { id = 2, str = "x", type = "READER" };
            E.Execute(C).Wait();
            Assert.IsInstanceOf<object[]>(C.Result);
            var a = (object[])C.Result;
            Assert.AreEqual(2,a.Length);
            var sec = (Dictionary<string, object>) a[1];
            Assert.AreEqual(6, sec["id"]);
            Assert.AreEqual("x bella", sec["name"]);
        }

        [Test]
        public void ObjectReader()
        {
            C.Notation = DbCallNotation.ObjectReader;
            C.TargetType = typeof(A);
            C.ParametersSoruce = new { id = 2, str = "x", type = "READER" };
            E.Execute(C).Wait();
            Assert.IsInstanceOf<object[]>(C.Result);
            var a = (object[])C.Result;
            Assert.AreEqual(2, a.Length);
            var sec = (A)a[1];
            Assert.AreEqual(6, sec.Id);
            Assert.AreEqual("x bella", sec.Name);
        }


        [Test]
        public void MultipleReader()
        {
            C.Notation = DbCallNotation.MultipleReader;
            C.ParametersSoruce = new { id = 2, str = "x", type = "MULTIPLE" };
            E.Execute(C).Wait();
            Assert.IsInstanceOf<object[]>(C.Result);
            var a = (object[])C.Result;
            Assert.AreEqual(2, a.Length);
            var seca = (object[])a[1];
            var sec = (Dictionary<string, object>) seca[1];
            Assert.AreEqual(10, sec["id2"]);
            Assert.AreEqual("x vella", sec["name2"]);
        }
        [Test]
        public void MultipleObject() {
            C.Notation = DbCallNotation.MultipleObject;
            C.TargetTypes = new[] {typeof (A), typeof (A2)};
            C.ParametersSoruce = new { id = 2, str = "x", type = "MULTIPLE" };
            E.Execute(C).Wait();
            if (null != C.Error) {
                Console.WriteLine(C.Error.ToString());
            }
            Assert.IsInstanceOf<object[]>(C.Result);
            var a = (object[])C.Result;
            Assert.AreEqual(2, a.Length);
            var seca = (object[])a[1];
            var sec = (A2)seca[1];
            Assert.AreEqual(10, sec.Id2);
            Assert.AreEqual("x vella", sec.Name2);
        }
    }
}