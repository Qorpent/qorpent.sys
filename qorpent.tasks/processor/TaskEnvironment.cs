using System.Collections.Generic;
using System.Xml.Linq;
using qorpent.tasks.console;
using Qorpent;
using Qorpent.BSharp;
using Qorpent.IoC;
using Qorpent.Log.NewLog;

namespace qorpent.tasks.processor {
    public class TaskEnvironment {
        public TaskEnvironment() {
            Globals = new Scope();
            LibNameCache = new List<string>();
        }

        public XElement Config { get; set; }
        public IBSharpContext Context { get; set; }
        public TaskConsoleParameters ConsoleCall { get; set; }
        public ILoggy Log { get; set; }
        public IScope Globals { get; set; }
        public string[] Targets { get; set; }
        public IContainer Container { get; set; }
        public IList<string> LibNameCache { get; set; }

        public TaskEnvironment Copy() {
            return MemberwiseClone() as TaskEnvironment;
        }
    }
}