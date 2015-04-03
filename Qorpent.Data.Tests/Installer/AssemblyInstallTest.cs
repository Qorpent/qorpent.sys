using System;
using System.Data.SqlTypes;
using System.IO;
using System.Reflection;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using Qorpent.Data.Installer;
using Qorpent.Mvc;
using Qorpent.Tasks;

namespace Qorpent.Data.Tests.Installer {

    public static class SampleExt {
        [SqlFunction]
        public static SqlInt32 Test(SqlInt32 id) {
            if (id.IsNull) return -1;
            return id.Value*4;
        }
    }

    [TestFixture]
    public class AssemblyInstallTest {
        private string dir;

        [SetUp]
        public void Setup()
        {
            var cmd = new DbCommandWrapper
            {
                ConnectionString = "Server=(local);Trusted_Connection=true",
                Database = "master",
                Query = @"
ALTER DATABASE AssemblyInstallTest SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
DROP DATABASE AssemblyInstallTest
"
            };
            DbCommandExecutor.Default.Execute(
                cmd
                ).Wait();
            DbInstallJobFactory.Create("AssemblyInstallTest").Execute();
        }

        [Test]
        public void CanInstallLibrary() {
            var mycodebase = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
            var rootdir = Path.GetDirectoryName(Path.GetDirectoryName(mycodebase));
            var pathtolib = Path.Combine(rootdir, "all", "Qorpent.Integration.SqlExtensions.dll");
            var task = new AssemblyDbUpdateTask(pathtolib);
            task.Database = "AssemblyInstallTest";
            task.Execute();
            if (null != task.Error) {
                Console.WriteLine(task.Error.ToString());
            }
            Assert.AreEqual(TaskState.Success,task.State);
        }

        [Test]
        public void InstallLibraryFromLegacyScript() {
            var path = EnvironmentInfo.ResolvePath("@repos@/qorpent.sys/Qorpent.Data/Old.Comdiv.Sql.Functions.sql");
            var task = new ScriptFileDbUpdateTask(path);
            task.Database = "AssemblyInstallTest";
            task.Execute();
            if (null != task.Error)
            {
                Console.WriteLine(task.Error.ToString());
            }
            Assert.AreEqual(TaskState.Success, task.State);
        }
    }
}