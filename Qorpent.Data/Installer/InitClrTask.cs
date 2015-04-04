using System.Collections.Generic;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// </summary>
    public class InitClrTask : DbUpdateTaskBase {
        public const int Index = InitMetaTableTask.Index + 10;

        /// <summary>
        /// </summary>
        public InitClrTask() {
            Name = "initclr";
            Group = "init";
            RunOnce = true;
            Idx = Index;
            MetaName = "initclr";
        }

        protected override IEnumerable<string> GetScripts() {
            yield return @"ALTER DATABASE [${database}] SET TRUSTWORTHY ON";
            yield return @"ALTER AUTHORIZATION ON database::[${database}] TO sa";
            yield return @"exec sp_configure 'show advanced options', 1";
            yield return @"exec sp_executesql N'RECONFIGURE'";
            yield return @"exec sp_configure 'clr enabled', 1";
            yield return @"exec sp_executesql N'RECONFIGURE'";
        }
    }
}