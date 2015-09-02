using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.processor {
    [ContainerComponent(Lifestyle.Transient, ServiceType = typeof (ITaskProcessor))]
    public class TaskProcessor : ServiceBase, ITaskProcessor {
        public void Execute(TaskEnvironment env) {
            env.Log.Debug("> project " + env.Config.Attr("code"));
            SetupEnvironment(env);
            ProcessSubprojects(env, "before", env.Config);
            ProcessTargets(env);
            ProcessSubprojects(env, "after", env.Config);
            env.Log.Debug("< project " + env.Config.Attr("code"));
        }

        private void ProcessTargets(TaskEnvironment env) {
            var targets = env.Config.Elements("target").ToDictionary(_ => _.Attr("code"), _ => _);
            foreach (var target in env.Targets) {
                ProcessTarget(env, targets, target, new List<string>());
            }
        }

        private void ProcessTarget(TaskEnvironment env, Dictionary<string, XElement> targets, string targetName,
            List<string> visited) {
            if (visited.Contains(targetName)) {
                return;
            }
            visited.Add(targetName);
            env.Log.Debug(">> target '" + targetName + "'");
            if (!targets.ContainsKey(targetName)) {
                throw new Exception("Cannot find target '" + targetName + "' in project '" + env.Config.Attr("code") +
                                    "'");
            }
            var target = targets[targetName];
            var pretargets = target.Attr("targets").SmartSplit(false, true, ',', ';', ' ');
            foreach (var pretarget in pretargets) {
                ProcessTarget(env, targets, pretarget, visited);
            }
            ProcessTarget(env, target);
            env.Log.Debug("<< target " + targetName);
        }

        private void ProcessTarget(TaskEnvironment env, XElement target) {
            var iocname = target.Attr("component");
            if (string.IsNullOrWhiteSpace(iocname)) {
                iocname = "qorpent.tasks.compound";
            }
            var task = env.Container.Get<ITask>(iocname);
            task.Initialize(env, null, target);
            var scope = new Scope(env.Globals);
            ProcessSubprojects(env, "before", target);
            task.Execute(scope);
            ProcessSubprojects(env, "after", target);
        }

        private void ProcessSubprojects(TaskEnvironment request, string mode, XElement root) {
            var requirements =
                request.Config.Elements("project")
                    .Where(
                        _ => _.Attr("mode") == mode || (mode == "before" && string.IsNullOrWhiteSpace(_.Attr("mode"))));
            foreach (var requirement in requirements) {
                var clsname = requirement.Attr("code");
                var cls = request.Context[clsname];
                if (null == clsname) {
                    throw new Exception("cannot find required subproject " + cls);
                }
                var targets = requirement.Attr("targets");
                var subenvironment = request.Copy();
                if (!string.IsNullOrWhiteSpace(targets)) {
                    subenvironment.Targets = targets.SmartSplit(false, true, ',', ';', ' ').ToArray();
                }
                Execute(subenvironment);
            }
        }

        private void SetupEnvironment(TaskEnvironment request) {
            request.Container = request.Container ?? Container;
            foreach (var element in request.Config.Elements("lib")) {
                var libname = element.AttrOrValue("code");
                if (!request.LibNameCache.Contains(libname)) {
                    request.Container.RegisterAssembly(Assembly.Load(libname));
                    request.LibNameCache.Add(libname);
                }
            }

            if (null == request.Targets || 0 == request.Targets.Length) {
                request.Targets = new[] {"default"};
            }
        }
    }
}