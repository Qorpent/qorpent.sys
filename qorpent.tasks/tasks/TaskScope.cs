using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qorpent.tasks.tasks
{
    [Flags]
    public enum TaskScope
    {
        None = 0,
        Local = 1,
        Parent = 2,
        Target = 4,
        Project = 8,
        Global = 16,
        Environment= 32
    }
}
