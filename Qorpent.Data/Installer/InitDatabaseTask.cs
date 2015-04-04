using System.Collections.Generic;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// </summary>
    public class InitDatabaseTask : DbUpdateTaskBase {
        public const int Index = -1000000;

        /// <summary>
        /// </summary>
        public InitDatabaseTask() {
            Name = "initdb";
            QueryDatabase = "master";
            Group = "init";
            RunOnce = true;
            Idx = Index;
        }

        protected override IEnumerable<string> GetScripts() {
            yield return "CREATE DATABASE ${database}";
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override bool HasUpdatedOnce() {
            var cmd = InitCommand(
                "select database_id from sys.databases where name = @database",
                DbCallNotation.Scalar);
            var id = DbExecutor.GetResultSync(cmd);
            return null != id;
        }
    }
}