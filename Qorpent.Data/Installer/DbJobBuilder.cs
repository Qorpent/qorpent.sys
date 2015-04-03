using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Tasks;

namespace Qorpent.Data.Installer
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbInstallJobFactory
    {

        public static IJob Create(string database = null) {
            var result = new Job();
            Setup(result,database);
            return result;
        }

        public static void Setup(IJob job, string database = null) {
            if (!job.Tasks.ContainsKey("initdb")) {
                job.Tasks["initdb"] = new InitDatabaseTask();
            }
            if (!job.Tasks.ContainsKey("initmeta"))
            {
                job.Tasks["initmeta"] = new InitMetaTableTask();
            }
            if (!job.Tasks.ContainsKey("initclr"))
            {
                job.Tasks["initclr"] = new InitClrTask();
            }
            if (!string.IsNullOrWhiteSpace(database)) {
                job.Data["database"] = database;
            }
        }
    }
}
