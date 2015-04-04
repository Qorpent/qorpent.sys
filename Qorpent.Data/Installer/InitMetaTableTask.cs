using System.Collections.Generic;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// </summary>
    public class InitMetaTableTask : DbUpdateTaskBase {
        public const int Index = InitDatabaseTask.Index + 10;

        /// <summary>
        /// </summary>
        public InitMetaTableTask() {
            Name = "initmeta";
            Group = "init";
            RunOnce = true;
            Idx = Index;
        }

        public override IEnumerable<string> GetScripts() {
            yield return @"IF SCHEMA_ID('qorpent') IS NULL EXEC sp_executesql N'CREATE SCHEMA qorpent'";
            yield return @"CREATE SEQUENCE qorpent.meta_seq AS bigint start with 100000 increment by 10";
            yield return @"CREATE TABLE qorpent.meta (
    Id bigint not null primary key default (next value for qorpent.meta_seq),
    Code  nvarchar(255) not null unique default '', 
    Hash nvarchar(255) not null default '',
    Version datetime not null default '19000101'
)";
            yield return @"IF OBJECT_ID('qorpent.metaupdate') IS NOT NULL DROP PROC qorpent.metaupdate";
            yield return
                @"CREATE PROCEDURE qorpent.metaupdate @code nvarchar(255), @hash nvarchar(255), @version datetime as begin
    if not exists (select top 1 id from qorpent.meta where code = @code) 
        insert qorpent.meta (code) values (@code)
    update qorpent.meta set hash = @hash, version = @version where code = @code
end";
        }

        protected override void FixSuccess() {
            base.FixSuccess();
            SaveMeta(GetSelfMetaDesc("initmeta"));
        }

        /// <summary>
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