using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using qorpent.tasks.processor;
using qorpent.v2.tasks;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.tasks.database {
    [ContainerComponent(Lifestyle.Transient, "qorpent.tasks.dbscript.task", ServiceType = typeof(ITask))]
    public class DbScriptCommand : DbTaskBase
    {

        public string CommandText { get; set; }
        public override void Initialize(TaskEnvironment environment, ITask parent, XElement config)
        {
            base.Initialize(environment, parent, config);
            CommandText = config.AttrOrValue("script");
        }

        protected override void InternalExecute(IScope scope) {
            var commands = Regex.Split(CommandText, @"(?i)(^|[\r\n])\s*go\s*([\r\n]|$)")
                .Where(_=>!string.IsNullOrWhiteSpace(_) && _.Trim().ToLowerInvariant()!="go").ToArray();
            using (var c = GetConnection(null, scope))
            {
                c.WellOpen();
                L.Trace(">>>> start script " +TaskCode + " : " + TaskName);
                for (var i = 0; i < commands.Length; i++) {
                    var command = c.CreateCommand();
                    command.CommandText = commands[i];
                    try {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e) {
                        if (ErrorLevel <= LogLevel.Trace) {
                            L.Trace("<<<<< error " + TaskCode + " : " + i + " : " + e.Message);
                            continue;

                        }
                        throw;
                    }
                    L.Info(">>>> executed " + TaskCode + " : " + i);
                    L.Debug(commands[i]);
                }
                L.Trace("<<<<< end script " + TaskCode + " : " + TaskName);
            }
        }
    }
}