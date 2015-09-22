using System;
using System.Xml.Linq;
using qorpent.tasks.processor;
using qorpent.v2.tasks;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.tasks.database {
    [ContainerComponent(Lifestyle.Transient,"qorpent.tasks.dbcommand.task",ServiceType=typeof(ITask))]
    public class DbCommandTask :DbTaskBase
    {

        public string CommandText { get; set; }
        public override void Initialize(TaskEnvironment environment, ITask parent, XElement config) {
            base.Initialize(environment, parent, config);
            CommandText = config.AttrOrValue("command");
        }

        protected override void InternalExecute(IScope scope) {
            using (var c = GetConnection(null, scope)) {
                c.WellOpen();
                var command = c.CreateCommand();
                command.CommandText = CommandText;
                try {
                    command.ExecuteNonQuery();
                }
                catch (Exception e) {
                    if (ErrorLevel <= LogLevel.Trace) {
                        L.Trace("<<<<< error " + TaskCode + " : " + TaskName + " : " + e.Message);
                        return;
                        
                    }
                    throw;
                }
                L.Info(">>>> executed "+TaskCode+" : "+TaskName);
                L.Debug(CommandText);
            }
        }
    }
}