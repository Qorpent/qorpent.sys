using System.IO;
using System.Linq;
using System.Xml.Linq;
using qorpent.tasks.processor;
using Qorpent;
using Qorpent.BSharp;
using Qorpent.IoC;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.tasks.misc {
    [ContainerComponent(Lifestyle.Transient, ServiceType = typeof(ITask), Name = "qorpent.tasks.bsharpload.task")]
    public class BSharpLoad : TaskBase {
        public string Directory { get; set; }
        public override void Initialize(TaskEnvironment environment, ITask parent, XElement config) {
            base.Initialize(environment, parent, config);
            this.Directory = config.ChooseAttr("dir", "directory");
        }

        protected override void InternalExecute(IScope scope) {
            var dir = Interpolate(Directory, scope);
            if (dir.Contains("@")) {
                dir = EnvironmentInfo.ResolvePath(dir);
            }
            if (!Path.IsPathRooted(dir)) {
                var fileDir = Path.GetDirectoryName( Config.Describe().File);
                dir = Path.GetFullPath( Path.Combine(fileDir, dir));
            }
            var context = BSharpCompiler.CompileDirectory(dir,
                new BSharpConfig { Global = new Scope(scope), KeepLexInfo = true });
            if (TaskScope == TaskScope.Environment) {
                Environment.Context.Merge(context);
            }
            else {
                var _scope = ResolveScope(TaskScope, scope);
                var scopedContext = _scope.Get<IBSharpContext>("bscontext");
                if (null != scopedContext) {
                    scopedContext.Merge(context);
                }
                else {
                    _scope["bscontext"] = context;
                }
            }
            L.Trace("B# classes from "+dir + " merged at level "+TaskScope+" : "+context.Get(BSharpContextDataType.Working).Count()+" classes total");
        }
    }
}