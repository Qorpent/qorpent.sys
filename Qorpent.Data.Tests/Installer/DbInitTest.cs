using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Qorpent.Data.Installer;
using Qorpent.Tasks;

namespace Qorpent.Data.Tests.Installer
{
    [TestFixture]
    public class DbInitTest
    {
        [SetUp]
        public void Setup() {
            var cmd = new DbCommandWrapper {
                ConnectionString = "Server=(local);Trusted_Connection=true",
                Database = "master",
                Query = @"
ALTER DATABASE DbInitTest SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
DROP DATABASE DbInitTest
"
            };
                DbCommandExecutor.Default.Execute(
                    cmd      
                    ).Wait();
         
           
        }

        
        [Test]
        public void CanInitClr()
        {
            new InitDatabaseTask { Database = "DbInitTest" }.Execute();
            new InitMetaTableTask { Database = "DbInitTest" }.Execute();
            var task = new InitClrTask { Database = "DbInitTest" };
            task.Execute();
            if (null != task.Error)
            {
                Console.WriteLine(task.Error.ToString());
            }
            Assert.AreEqual(TaskState.Success, task.State);
        }


        [Test]
        public void WillNotInitClrTwice()
        {
            new InitDatabaseTask { Database = "DbInitTest" }.Execute();
            new InitMetaTableTask { Database = "DbInitTest" }.Execute();
            var task = new InitClrTask { Database = "DbInitTest" };
            task.Execute();
            Assert.AreEqual(TaskState.Success, task.State);
            task = new InitClrTask { Database = "DbInitTest" };
            task.Execute();
            Assert.AreEqual(TaskState.SuccessOnce, task.State);
        }

        [Test]
        public void CanCreateMetaTable()
        {
            new InitDatabaseTask { Database = "DbInitTest" }.Execute();
            var task = new InitMetaTableTask { Database = "DbInitTest" };
            task.Execute();
            if (null != task.Error)
            {
                Console.WriteLine(task.Error.ToString());
            }
            Assert.AreEqual(TaskState.Success, task.State);
        }


        [Test]
        public void WillNotCreateMetaTableTwice()
        {
            new InitDatabaseTask { Database = "DbInitTest" }.Execute();
            var task = new InitMetaTableTask { Database = "DbInitTest" };
            task.Execute();
            Assert.AreEqual(TaskState.Success, task.State);
            task = new InitMetaTableTask { Database = "DbInitTest" };
            task.Execute();
            if (null != task.Error) {
                Console.WriteLine(task.Error);
            }
            Assert.AreEqual(TaskState.SuccessOnce, task.State);
        }


        [Test]
        public void CanCreateDatabase() {
            var task = new InitDatabaseTask {Database = "DbInitTest"};
            task.Execute();
            Assert.AreEqual(TaskState.Success,task.State);
            var id = DbCommandExecutor.Default.GetResultSync(
                   new DbCommandWrapper
                   {
                       ConnectionString = "Server=(local);Trusted_Connection=true",
                       Database = "master",
                       Notation = DbCallNotation.Scalar,
                       Query = @"
select database_id from sys.databases where name= 'DbInitTest'
"
                   }
                   );
            Console.WriteLine(id);
            Assert.NotNull(id);
        }

        [Test]
        public void WillNotExecuteTwice()
        {
            var task = new InitDatabaseTask { Database = "DbInitTest" };
            task.Execute();
            Assert.AreEqual(TaskState.Success, task.State);
            task = new InitDatabaseTask { Database = "DbInitTest" };
            task.Execute();
            Assert.AreEqual(TaskState.SuccessOnce,task.State);
        }
    }
}
