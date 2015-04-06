using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.Scaffolding.Model;

namespace Qorpent.Data.Installer {
    public class BxlDataProjectSchemaUpdateTask : BxlDataProjectUpdateTaskBase
    {
        public const int Index = 5000;
        public BxlDataProjectSchemaUpdateTask(string projectfile, string projectname = null)
            : base(projectfile, projectname)
        {
            this.Idx = Index;
            Suffix = "Schema";
            IgnoreErrors = true;
        }
        public override IEnumerable<string> GetScripts() {
            var writers = Model.GetWriters(SqlDialect.SqlServer, ScriptMode.Create);
            foreach (var writer in writers) {
                var script = writer.ToString();
                var queries = Regex.Split(script, @"(?i)[\r\n]+\s*GO\s*?[\r\n]+");
                foreach (var q in queries.Select(query => query.Trim()).Where(q => !string.IsNullOrWhiteSpace(q))) {
                    if(q.Trim().ToLowerInvariant()=="go")continue;
                    yield return q;
                }
                ;
            }
        }

        protected override bool RequireExecution() {
            if (!base.RequireExecution()) return false;
            return Model.Classes.Count != 0;
        }
    }
}