using System;
using System.IO;
using NUnit.Framework;
using Qorpent.Data.Installer;
using Qorpent.Log;
using Qorpent.Tasks;
using Qorpent.Utils;
using Qorpent.Utils.Git;

namespace Qorpent.Data.Tests.Installer {
    [TestFixture]
    public class DbScriptTest {
        private const string DbName = "DbScriptTest";
        private string dir;
        private string file;
        private string file2;

        [SetUp]
        public void Setup() {
            
            DbInstallFactory.InitTestDatabase(DbName);
            dir = FileSystemHelper.ResetTemporaryDirectory();
            file = Path.Combine(dir, "a.sql");
            file2 = Path.Combine(dir, "b.sql");
        }

        [Test]
        public void CanExecute() {
            File.WriteAllText(file, "select 1 print '1 selected'");
            var t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            if (null != t.Error) {
                Console.WriteLine(t.Error);
            }
            Assert.AreEqual(TaskState.Success, t.State);
            t.Refresh();
            Assert.AreEqual(t.Target.Hash, t.Source.Hash);
            var diff = Math.Floor((t.Target.Version - t.Source.Version).TotalSeconds);
            Assert.LessOrEqual(diff, 1);
        }

        [Test]
        public void CanExecuteTextScript() {
            var t = new ScriptTextDbUpdateTask("a", "select 1 print '1 selected'") { Database = DbName };
            t.Execute();
            if (null != t.Error)
            {
                Console.WriteLine(t.Error);
            }
            Assert.AreEqual(TaskState.Success, t.State);
        }

        [Test]
        public void WillNotExectuteSameTextScriptTwice()
        {
            var t = new ScriptTextDbUpdateTask("a", "select 1 print '1 selected'") { Database = DbName };
            t.Execute();
            t = new ScriptTextDbUpdateTask("a", "select 1 print '1 selected'") { Database = DbName };
            t.Execute();
            Assert.AreEqual(TaskState.SuccessNotRun,t.State);
        }

        [Test]
        public void WillExectuteUpdatedScriptTwice()
        {
            var t = new ScriptTextDbUpdateTask("a", "select 1 print '1 selected'") { Database = DbName };
            t.Execute();
            t = new ScriptTextDbUpdateTask("a", "select 1 print '1 selected' --rem") { Database = DbName };
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
        }

        [Test]
        public void CanExecuteWithInterpolationsAndParameters() {
            File.WriteAllText(file, "select @x ${command}");
            var t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t["x"] = 1;
            t["command"] = "print '1 executed'";
            t.Execute();
            if (null != t.Error) {
                Console.WriteLine(t.Error);
            }
            Assert.AreEqual(TaskState.Success, t.State);
        }

        [Test]
        public void WillNotApplySameVersion() {
            File.WriteAllText(file, "select 1 print '1 selected'");
            var t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
            t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            Assert.AreEqual(TaskState.SuccessNotRun, t.State);
        }

        [Test]
        public void WillApplyNewerVersion() {
            File.WriteAllText(file, "select 1 print '1 selected'");
            var t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
            File.WriteAllText(file, "select 2 print '2 selected'");
            t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
        }

        [Test]
        public void ExplicitOrderOfTasks() {
            var j = new Job();
            j["database"] = DbName;
            var sw = new StringWriter();
            j.Log = TextWriterLogWriter.CreateLog(sw, level: LogLevel.Info, customFormat: "${Message}");
            File.WriteAllText(file, @"
--!options idx=20
select 2 print '2 selected'
");
            File.WriteAllText(file2, @"
--!options idx=10
select 1 print '1 selected'");
            j.Tasks["t1"] = new ScriptFileDbUpdateTask(file);
            j.Tasks["t2"] = new ScriptFileDbUpdateTask(file2);
            j.Execute();
            var result = sw.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(@"finish t2 with Success
finish t1 with Success
", result);
        }

        [Test]
        public void CanUseHeader() {
            File.WriteAllText(file, @"
/*!
options runonce=1
*/
select 1 print '1 selected'
");
            var t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
            File.WriteAllText(file, @"
--!options runonce=1
select 2 print '2 selected'
");
            t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            Assert.AreEqual(TaskState.SuccessOnce, t.State);
        }

        [Test]
        public void CanUseGit() {
            GitHelper.Init(dir);
            File.WriteAllText(file, "select 1 print '1 selected'");
            GitHelper.CommitAll(dir);
            //   Console.WriteLine(GitHelper.GetLastCommit(file).Hash);
            var t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
            //  Console.WriteLine(t.GetLastLog());
            File.WriteAllText(file, "select 2 print '2 selected'");
            t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            Assert.AreEqual(TaskState.SuccessNotRun, t.State);
            GitHelper.CommitAll(dir);
            //   Console.WriteLine(GitHelper.GetLastCommit(file).Hash);
            t = new ScriptFileDbUpdateTask(file) {Database = DbName};
            t.Execute();
            Assert.AreEqual(TaskState.Success, t.State);
        }
    }
}