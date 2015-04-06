using System;
using System.Diagnostics;
using Qorpent.Tasks;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// </summary>
    public static class DbInstallFactory {
        public static IJob Create(string database = null, bool dropdatabase  = false) {
            var result = new Job();
            Setup(result, database, dropdatabase);
            return result;
        }

        public static void Setup(IJob job, string database = null, bool dropdatabase = false) {
            if (dropdatabase) {
                if (!job.Tasks.ContainsKey("dropdb")) {
                    job.Tasks["dropdb"] = new DropDatabaseTask();
                }
            }
            if (!job.Tasks.ContainsKey("initdb")) {
                job.Tasks["initdb"] = new InitDatabaseTask();
            }
            if (!job.Tasks.ContainsKey("initmeta")) {
                job.Tasks["initmeta"] = new InitMetaTableTask();
            }
            if (!job.Tasks.ContainsKey("initclr")) {
                job.Tasks["initclr"] = new InitClrTask();
            }
            if (!string.IsNullOrWhiteSpace(database)) {
                job["database"] = database;
            }
        }

        public static void InitTestDatabase(string name = null) {
            if (String.IsNullOrWhiteSpace(name))
            {
                var method = new StackFrame(1, true).GetMethod();
                name = method.DeclaringType.Name + "_" + method.Name;
            }
            Create(name,true).Execute();
        }
    }
}