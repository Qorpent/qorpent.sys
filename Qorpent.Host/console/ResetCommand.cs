using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using qorpent.v2.console;
using qorpent.v2.reports;
using qorpent.v2.security.authorization;
using qorpent.v2.security.user.storage;
using Qorpent.Events;
using Qorpent.Experiments;
using Qorpent.IoC;

namespace Qorpent.Host.console
{
    [ContainerComponent(Lifestyle.Singleton, Name = "qorpent.sys.command.reset", ServiceType = typeof(IConsoleCommand))]
    public class ResetCommand : ConsoleCommandBase
    {

        protected override async Task InternalExecute(IConsoleContext context, ConsoleCommandResult result, string commandname, string commandstring, IScope scope) {
            var cmd = commandstring.Trim();
            if (string.IsNullOrWhiteSpace(cmd)) {
                cmd = "all";
            }
            var d = new ResetEventData(cmd=="all");
            foreach (var component in Container.GetComponents()) {
                if (component.Lifestyle == Lifestyle.Singleton && null!=component.Implementation) {
                    var type = component.Implementation.GetType();
                    var ra = type.GetCustomAttribute<RequireResetAttribute>();
                    if (null != ra && ra.IsMatch(type, cmd)) {
                        if (typeof (IResetable).IsAssignableFrom(type)) {
                            context.WriteLine(type.Name + " : " +
                                              ((IResetable) component.Implementation).Reset(d).stringify());
                        }
                    }
                }
            }
        }

    }
}
