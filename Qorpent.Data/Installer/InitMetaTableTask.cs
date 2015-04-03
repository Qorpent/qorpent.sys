using System.Collections.Generic;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// 
    /// </summary>
    public class InitMetaTableTask : DbUpdateTaskBase {
        public const int Index = InitDatabaseTask.Index + 10;

        /// <summary>
        /// 
        /// </summary>
        public InitMetaTableTask()
        {
            Name = "initmeta";
            Group = "init";
            RunOnce = true;
            Idx = Index;

        }
        protected override IEnumerable<string> GetScripts() {
            yield return @"IF SCHEMA_ID('qorpent') IS NULL EXEC sp_executesql N'CREATE SCHEMA qorpent'";
            yield return @"CREATE SEQUENCE qorpent.meta_seq AS bigint start with 100000 increment by 10";
            yield return @"CREATE TABLE qorpent.meta (
    Id bigint not null primary key default (next value for qorpent.meta_seq),
    Code  nvarchar(255) not null unique default '', 
    Hash nvarchar(255) not null default '',
    Version datetime not null default '19000101'
)";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool HasUpdatedOnce() {
            var cmd = InitCommand(
                "select table_name from INFORMATION_SCHEMA.TABLES where table_name = 'meta' and table_schema='qorpent'",
                DbCallNotation.Scalar);
            var table_name = DbExecutor.GetResultSync(cmd);
            return null != table_name;
        }
    }
}