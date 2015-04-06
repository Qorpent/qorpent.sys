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

        public override IEnumerable<string> GetScripts() {
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

    /// <summary>
    /// </summary>
    public class DropDatabaseTask : DbUpdateTaskBase
    {
        public const int Index = -2000000;

        /// <summary>
        /// </summary>
        public DropDatabaseTask()
        {
            Name = "dropdb";
            QueryDatabase = "master";
            Group = "init";
            Idx = Index;
            IgnoreErrors = true;
        }

        public override IEnumerable<string> GetScripts()
        {
            yield return @"ALTER DATABASE ${database} SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
DROP DATABASE ${database}";
        }

        
    }
}