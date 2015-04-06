using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using Qorpent.Data.DataDiff;

namespace Qorpent.Data.Installer {
    public class BxlDataProjectDataUpdateTask: BxlDataProjectUpdateTaskBase {
        public const int Index = 10000;
        public BxlDataProjectDataUpdateTask(string projectfile, string projectname = null):base(projectfile,projectname) {
            this.Idx = Index;
        }
        public override IEnumerable<string> GetScripts()
        {
            using (var c = new SqlConnection(ConnectionString)) {
                c.Open();
                if (!string.IsNullOrWhiteSpace(Database)) {
                    c.ChangeDatabase(Database);
                }
                var xml = new XElement("batch");
                foreach (var cls in Context.ResolveAll("db-meta")) {
                    xml.Add(cls.Compiled);
                }
                var ctx = MyDataDiff.GenerateContext(c, xml);
                foreach (var sqlScript in ctx.SqlScripts)
                {
                    yield return sqlScript;
                }
            }
        }

        
    }
}