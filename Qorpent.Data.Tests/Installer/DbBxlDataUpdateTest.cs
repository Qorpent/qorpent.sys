using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Qorpent.Data.Installer;
using Qorpent.Tasks;
using Qorpent.Utils;

namespace Qorpent.Data.Tests.Installer {
    [TestFixture]
    public class DbBxlDataUpdateTest {
        private const string DbName = "DbScriptTest";
        private string dir;
        private string file;
        private string file2;
        private string file3;
        private BxlDataProjectDataUpdateTask task;

        [SetUp]
        public void Setup() {
            DbInstallFactory.InitTestDatabase(DbName);
            dir = FileSystemHelper.ResetTemporaryDirectory();
            file = Path.Combine(dir, "a.bxls");
            file2 = Path.Combine(dir, "b.bxls");
            file3 = Path.Combine(dir, "c.bxls");

            var createtable = new ScriptTextDbUpdateTask("createtable", @"
create table dbo.test (
    id int  not null primary key,
    code nvarchar(255) not null unique default '',
    name nvarchar(255) not null default '',
    version datetime not null default getdate()
)
create table dbo.test1 (
    id int  not null primary key,
    code nvarchar(255) not null unique default '',
    name nvarchar(255) not null default '',
    version datetime not null default getdate(),
    test2 int not null  default 0
)
create table dbo.test2 (
    id int  not null primary key,
    code nvarchar(255) not null unique default '',
    name nvarchar(255) not null default '',
    version datetime not null default getdate(),
    test1 int not null default 0
)
GO
alter table dbo.test1 add constraint test2_fk foreign key (test2) references dbo.test2 (id)
GO
alter table dbo.test2 add constraint test1_fk foreign key (test1) references dbo.test1 (id) 
") { Database = DbName };
            createtable.Execute();
            Assert.AreEqual(TaskState.Success, createtable.State);

            File.WriteAllText(file, @"
class a prototype = db-meta table=dbo.test
    item id=10 code=X name=XX
    item id=20 code=Y name=YY
    item id=30 code=Z name=ZZ
");

            File.WriteAllText(file2, @"
class b prototype = db-meta table=dbo.test1
    item id=10 code=X name=XX test2=10
");
            File.WriteAllText(file3, @"
class c prototype = db-meta table=dbo.test2
    item id=10 code=Y name=YY test1=10 

");
            this.task = new BxlDataProjectDataUpdateTask(dir) { Database = DbName };
        }

        [Test]
        public void CanUpdateFromFile() {
     
            task.Execute();
            if (null != task.Error) {
                Console.WriteLine(task.Error.ToString());
            }
            Assert.AreEqual(TaskState.Success, task.State);
            var data = DbCommandExecutor.Default.GetResultSync(new DbCommandWrapper {
                Notation = DbCallNotation.Reader,
                Database = DbName,
                Query = "select * from dbo.test order by id"
            }) as object[];
            Assert.AreEqual(3,data.Length);
        }



        [Test]
        public void WillApplyChanged()
        {

            task.Execute();
            
            Assert.AreEqual(TaskState.Success, task.State);
            Thread.Sleep(500);
            File.WriteAllText(file, @"
class a prototype = db-meta table=dbo.test
    item id=10 code=X name=XX2
    item id=20 code=Y name=YY
    item id=30 code=Z name=ZZ2
");
            task = new BxlDataProjectDataUpdateTask(dir) { Database = DbName };
            task.Execute();
            Assert.AreEqual(TaskState.Success, task.State);
            
        }
        [Test]
        public void WillNotUpdateNoChanged()
        {

            task.Execute();

            Assert.AreEqual(TaskState.Success, task.State);
            task = new BxlDataProjectDataUpdateTask(dir) { Database = DbName };
            task.Execute();
            Assert.AreEqual(TaskState.SuccessNotRun, task.State);

        }
    }
}