using System;
using System.IO;
using NUnit.Framework;
using Qorpent.Data.Installer;
using Qorpent.Tasks;
using Qorpent.Utils;
using Qorpent.Utils.Git;

namespace Qorpent.Data.Tests.Installer {
    [TestFixture]
    public class DbSciptTest {
        private string dir;
        private string file;

        [SetUp]
        public void Setup() {
            var cmd = new DbCommandWrapper
            {
                ConnectionString = "Server=(local);Trusted_Connection=true",
                Database = "master",
                Query = @"
ALTER DATABASE DbSciptTest SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
DROP DATABASE DbSciptTest
"
            };
            DbCommandExecutor.Default.Execute(
                cmd
                ).Wait();
            var job = new Job();
            job.Tasks["initdb"] = new InitDatabaseTask();
            job.Tasks["initmeta"] = new InitMetaTableTask();
            job.Data["database"] = "DbSciptTest";
            job.Execute();
            this.dir = FileSystemHelper.ResetTemporaryDirectory();
            this.file = Path.Combine(dir, "a.sql");
        }

        [Test]
        public void CanExecute() {
            File.WriteAllText(file,"select 1 print '1 selected'");
            var t = new ScriptFileDbUpdateTask(file) { Database = "DbSciptTest" };
            t.Execute();
            if (null != t.Error) {
                Console.WriteLine(t.Error);
            }
            Assert.AreEqual(TaskState.Success,t.State);
            t.Refresh();
            Assert.AreEqual(t.Target.Hash,t.Source.Hash);
            var diff = Math.Floor((t.Target.Version - t.Source.Version).TotalSeconds);
            Assert.LessOrEqual(diff,1);
        }

        [Test]
        public void CanExecuteWithInterpolationsAndParameters()
        {
            File.WriteAllText(file, "select @x ${command}");
            var t = new ScriptFileDbUpdateTask(file) { Database = "DbSciptTest" };
            t.Data["x"] = 1;
            t.Data["command"] = "print '1 executed'";
            t.Execute();
            if (null != t.Error) {
                Console.WriteLine(t.Error);
            }
            Assert.AreEqual(TaskState.Success, t.State);
        }

        [Test]
        public void WillNotApplySameVersion()
        {
            File.WriteAllText(file, "select 1 print '1 selected'");
            var t = new ScriptFileDbUpdateTask(file) { Database = "DbSciptTest" };
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
            t = new ScriptFileDbUpdateTask(file) { Database = "DbSciptTest" };
            t.Execute();
            Assert.AreEqual(TaskState.SuccessNotRun, t.State);
            
        }

        [Test]
        public void WillApplyNewerVersion() {
            File.WriteAllText(file, "select 1 print '1 selected'");
            var t = new ScriptFileDbUpdateTask(file) { Database = "DbSciptTest" };
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
            File.WriteAllText(file, "select 2 print '2 selected'");
            t = new ScriptFileDbUpdateTask(file) { Database = "DbSciptTest" };
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
        }

        [Test]
        public void CanUseGit()
        {
            GitHelper.Init(dir);
            File.WriteAllText(file, "select 1 print '1 selected'");
            GitHelper.CommitAll(dir);
            //   Console.WriteLine(GitHelper.GetLastCommit(file).Hash);
            var t = new ScriptFileDbUpdateTask(file) { Database = "DbSciptTest" };
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
            //  Console.WriteLine(t.GetLastLog());
            File.WriteAllText(file, "select 2 print '2 selected'");
            t = new ScriptFileDbUpdateTask(file) { Database = "DbSciptTest" };
            t.Execute();
            Assert.AreEqual(TaskState.SuccessNotRun, t.State);
            GitHelper.CommitAll(dir);
            //   Console.WriteLine(GitHelper.GetLastCommit(file).Hash);
            t = new ScriptFileDbUpdateTask(file) { Database = "DbSciptTest" };
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);

        }
    }
}