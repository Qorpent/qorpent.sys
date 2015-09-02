using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using qorpent.tasks.processor;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Utils.Extensions;
using Qorpent.Log.NewLog;

namespace qorpent.tasks.tasks.misc
{
    [ContainerComponent(Lifestyle.Transient, ServiceType = typeof(ITask), Name = "qorpent.tasks.message.task")]
    public class MessageTask:TaskBase
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public override void Initialize(TaskEnvironment environment, ITask parent, XElement config) {
            base.Initialize(environment, parent, config);
            Message = config.AttrOrValue("message");
            Level = config.Attr("level", "warn").To<LogLevel>();
        }

        protected override void InternalExecute(IScope scope) {
            var message = Interpolate(Message, scope);
            L.Write(Level,message);
        }
    }
}
