using System;
using System.IO;
using NUnit.Framework;
using Qorpent.Data.Installer;
using Qorpent.Tasks;
using Qorpent.Utils;

namespace Qorpent.Data.Tests.Installer {
    [TestFixture]
    public class DbBxlSchemaUpdateTest {
        private const string DbName = "DbScriptTest";
        private string dir;
        private string file;
        private string file2;
        private string file3;
        private BxlDataProjectDataUpdateTask task;

        [SetUp]
        public void Setup()
        {
            DbInstallFactory.InitTestDatabase(DbName);
            dir = FileSystemHelper.ResetTemporaryDirectory();
            file = Path.Combine(dir, "a.bxls");
            file2 = Path.Combine(dir, "b.bxls");
            file3 = Path.Combine(dir, "c.bxls");


            File.WriteAllText(file, @"
class a prototype = db-meta table=the.test
    item id=10 code=X name=XX
    item id=20 code=Y name=YY
    item id=30 code=Z name=ZZ
");

            File.WriteAllText(file2, @"
require data
TableBase test schema=the
    import IEntity
");
            
            
        }

        [Test]
        public void CanCreateSchema() {
            var task = new BxlDataProjectSchemaUpdateTask(dir) {Database = DbName};
            task.Execute();
            if(null!=task.Error)Console.WriteLine(task.Error);
            Assert.AreEqual(TaskState.Success,task.State);
            var data = DbCommandExecutor.Default.GetResultSync(new DbCommandWrapper {
                Notation = DbCallNotation.Reader,
                Database = DbName,
                Query = "select * from the.test order by id"
            }) as object[];
            Assert.AreEqual(2,data.Length);
        }

        [Test]
        public void CanCreateSchemaAndLoadData() {
            var dtask = new BxlDataProjectDataUpdateTask(dir) {Database = DbName};
            var stask = new BxlDataProjectSchemaUpdateTask(dir) { Database = DbName };
            var job = new Job();
            job.Tasks["dtask"] = dtask;
            job.Tasks["stask"] = stask;
            job.Execute();
            Assert.True(job.Success);
            var data = DbCommandExecutor.Default.GetResultSync(new DbCommandWrapper {
                Notation = DbCallNotation.Reader,
                Database = DbName,
                Query = "select * from the.test order by id"
            }) as object[];
            Assert.AreEqual(5,data.Length);
        }
    }
}