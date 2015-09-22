using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using qorpent.tasks.factory;
using qorpent.tasks.processor;
using qorpent.v2.tasks;
using Qorpent;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace qorpent.tasks.tasks {
    public abstract class TaskBase : ITask {
        public TaskBase() {
            Name = GetType().Name;
            Before = new List<ITask>();
            After = new List<ITask>();
            Fallback = new List<ITask>();
            PlainConfig = new Dictionary<string, string>();
            Interpolator = new StringInterpolation {AncorSymbol = '@'};
        }
        public TaskScope TaskScope { get; set; }
        public StringInterpolation Interpolator { get; set; }

        public ITaskFactory TaskFactory { get; set; }

        public string Name { get; set; }

        public LogLevel ErrorLevel { get; set; }
        public string Condition { get; set; }

        public IList<ITask> Before { get; set; }
        public IList<ITask> After { get; set; }
        public IList<ITask> Fallback { get; set; }
        public ILoggy L { get; set; }

        public XElement Config { get; set; }

        public ITask Parent { get; set; }

        public TaskEnvironment Environment { get; set; }

        public IDictionary<string,string> PlainConfig { get; set; } 

        public T ResolveParam<T>(string name, T def =default(T)) {
            if (null!=CurrentScope && CurrentScope.ContainsKey(name)) {
                return CurrentScope[name].To<T>();
            }
            if (PlainConfig.ContainsKey(name)) {
                return PlainConfig[name].To<T>();
            }
            var result = Config.AttrOrElement(name);
            if (!string.IsNullOrWhiteSpace(result)) return result.To<T>();
            return def;
        }

        public virtual void Initialize(TaskEnvironment environment, ITask parent, XElement config) {
            Environment = environment;
            TaskFactory = TaskFactory ?? environment.Container.Get<ITaskFactory>();
            Parent = parent;
            Config = config;
            L = Environment.Log;
            ErrorLevel = config.Attr("errorlevel", "Error").To<LogLevel>();
            Condition = config.Attr("condition");
            TaskScope = config.Attr("scope").To<TaskScope>();
            var name = config.Attr("name");
            if (!string.IsNullOrWhiteSpace(name)) {
                Name = name;
            }
            if (config.Attr("async").ToBool()) {
                Flags |= TaskFlags.Async;
            }
            SetupTasks(environment, Before, config.Element("before"));
            SetupTasks(environment, After, config.Element("after"));
            SetupTasks(environment, Fallback, config.Element("fallback"));

            TaskCode = config.Attr("code");
            TaskName = config.Attr("name");

            var current = config;
            while (true) {
                if(null==current)break;
                foreach (var a in current.Attributes()) {
                    if (!PlainConfig.ContainsKey(a.Name.LocalName)) {
                        PlainConfig[a.Name.LocalName] = a.Value;
                    }   
                }
                current = current.Parent;
            }
        }

        public IScope ResolveScope( TaskScope taskScope = TaskScope.None, IScope localScope = null) {
            if (taskScope == TaskScope.None) {
                taskScope = this.TaskScope;
            }
            if (TaskScope == TaskScope.Environment) {
                throw new Exception("environment level is not resolveable to IScope");
            }
            if (taskScope == TaskScope.Project) {
                taskScope= TaskScope.Global; //for now Projects share same scope by design
            }
            if (taskScope == TaskScope.Parent)
            {
                if (null == Parent) {
                    taskScope = TaskScope.Global;
                }
                else {
                    var taskbase = Parent as TaskBase;
                    if (null == taskbase) {
                        return (localScope ?? CurrentScope).GetParent();
                    }
                    return taskbase.ResolveScope(TaskScope.Local);
                }
            }
            if (taskScope == TaskScope.Target) {
                if (this is CompoundTask && null == Parent) {
                    taskScope = TaskScope.Local;
                }
                else if (null == Parent) {
                    throw new Exception("cannot find target scope - invalid task structure");
                }
                else {
                    var target = Parent as TaskBase;
                    while (true) {
                        if (null == target) {
                            throw new Exception("cannot find target scope - invalid task structure");
                        }
                        if (target is CompoundTask && null == target.Parent) {
                            return target.ResolveScope(TaskScope.Local);
                        }
                        target = target.Parent as TaskBase;
                        

                    }
                }

            }
            if (taskScope == TaskScope.Global) {
                return this.Environment.Globals;
            }
          
            if (TaskScope == TaskScope.Local) {
                return localScope ?? CurrentScope;
            }
            throw new Exception("invalid resolve scope request");
        }

        public IScope CurrentScope { get; set; }

        public void Execute(IScope scope) {
            this.CurrentScope = scope;
            if (IsMatch(scope)) {
                try {
                    L.Debug(">>> task " + Name);
                    ExecuteSubTasks(Before, scope);
                    InternalExecute(scope);
                    ExecuteSubTasks(After, scope);
                    L.Debug("<<< task " + Name);
                }
                catch (Exception e) {
                    L.Write(ErrorLevel, "<<< error in " + Name + " : " + e);
                    try {
                        ExecuteSubTasks(Fallback, scope);
                    }
                    catch (Exception fe) {
                        L.Error("ERROR IN FALLBACK OF " + Name + " : " + fe);
                    }
                    if (ErrorLevel >= LogLevel.Error) {
                        throw e;
                    }
                }
            }
            else {
                L.Debug("<==> " + Name + " ignored ");
            }
        }

        public TaskFlags Flags { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }

        protected void ExecuteSubTasks(IEnumerable<ITask> tasks, IScope scope) {
            var ts = tasks.ToArray();
            var asyncs = ts.Where(_ => _.Flags.HasFlag(TaskFlags.Async)).ToArray();
            var syncs = ts.Where(_ => !_.Flags.HasFlag(TaskFlags.Async)).ToArray();
            IList<Task> waiters = new List<Task>();
            foreach (var w in asyncs) {
                var s = new Scope(scope);
                waiters.Add(Task.Run(() => w.Execute(s)));
            }
            foreach (var sync in syncs) {
                var s = new Scope(scope);
                sync.Execute(s);
            }
            if (0 != waiters.Count) {
                Task.WaitAll(waiters.ToArray());
            }
        }

        protected virtual bool IsMatch(IScope scope) {
            if (string.IsNullOrWhiteSpace(Condition)) {
                return true;
            }
            return new LogicalExpressionEvaluator().Eval(Condition, scope);
        }

        protected abstract void InternalExecute(IScope scope);


        protected void SetupTasks(TaskEnvironment environment, IList<ITask> target, XElement config) {
            if (null == config) {
                return;
            }
            foreach (var element in config.Elements()) {
                var name = element.Name.LocalName;
                if (name == "before" || name == "after" || name == "fallback" || name == "project") {
                    continue;
                }
                if (null == TaskFactory) {
                    throw new Exception("no task factory configured");
                }
                var task = TaskFactory.Create(environment, element);
                if (null != task) {
                    target.Add(task);
                    task.Initialize(environment, this, element);
                }
            }
        }

        protected string Interpolate(string str, IScope scope) {
            var s = GetInterpolationScope(scope);
            return Interpolator.Interpolate(str, s);
        }

        protected virtual IScope GetInterpolationScope(IScope scope) {
            return scope;
        }
    }
}