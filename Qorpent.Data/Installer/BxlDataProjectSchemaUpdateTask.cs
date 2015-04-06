using System;
using System.Collections.Generic;

namespace Qorpent.Data.Installer {
    public class BxlDataProjectSchemaUpdateTask : BxlDataProjectUpdateTaskBase
    {
        public const int Index = 5000;
        public BxlDataProjectSchemaUpdateTask(string projectfile, string projectname = null)
            : base(projectfile, projectname)
        {
            this.Idx = Index;
        }
        public override IEnumerable<string> GetScripts()
        {
            throw new NotImplementedException();
        }
    }
}