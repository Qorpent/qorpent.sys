using System;
using System.Collections.Generic;
using Qorpent;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.console {
    public class TaskConsoleParameters : ConsoleApplicationParameters {
        public string ScriptClass { get; set; }
        public IList<string> Targets { get; set; }
        public IDictionary<string, object> Global { get; set; }
        public string ScriptDirectory { get; set; }

        protected override void InternalInitialize(string[] args) {
            base.InternalInitialize(args);
            Targets = new List<string>();
            Global = new Dictionary<string, object>();
            SetupScriptDirectory();
            var scriptClass = this.ResolveBestString("arg1", "class");
            if (string.IsNullOrWhiteSpace(scriptClass)) {
                scriptClass = "default";
            }
            ScriptClass = scriptClass;
            foreach (var p in this) {
                if (p.Key.StartsWith("set")) {
                    Global[p.Key.Substring(3)] = p.Value;
                }
                else if (p.Key.StartsWith("arg") && p.Key != "arg1") {
                    Targets.Add(p.Value.ToStr());
                }
            }
            if (!Global.ContainsKey(ScriptClass)) {
                Global[ScriptClass] = ScriptClass;
            }
        }

        private void SetupScriptDirectory() {
            var scriptDirectory = WorkingDirectory;
            if (string.IsNullOrWhiteSpace(scriptDirectory)) {
                scriptDirectory = Environment.CurrentDirectory;
            }
            ScriptDirectory = scriptDirectory;
        }
    }
}