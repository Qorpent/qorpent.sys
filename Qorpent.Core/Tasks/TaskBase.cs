using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Log;

namespace Qorpent.Tasks {
    /// <summary>
    ///     Описывает абстракцию обновляемого модуля
    /// </summary>
    public abstract class TaskBase : ConfigBase, ITask {
        private IUserLog _log = StubUserLog.Default;
        private TaskState _state;
        private readonly IList<ITask> _requiredModules = new List<ITask>();

        /// <summary>
        ///     Признак одноразового применения модуля
        /// </summary>
        public bool RunOnce { get; set; }

        /// <summary>
        ///     Признак игнорирования ошибок
        /// </summary>
        public bool IgnoreErrors { get; set; }

        /// <summary>
        ///     Дополнительное определение
        /// </summary>
        public XElement Definition { get; set; }

        /// <summary>
        ///     Обший порядок выполнения
        /// </summary>
        public int Idx { get; set; }

        public string Name { get; set; }
        public string[] Requirements { get; set; }

        public TaskState State {
            get { return _state; }
            protected set {
                _state = value;
                Log.Debug(Name + " : state changed -> " + value);
                OnStateChange();
            }
        }

        public Exception Error { get; set; }

        public IList<ITask> RequiredModules {
            get { return _requiredModules; }
        }

        /// <summary>
        ///     Группировка в модуль
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        ///     Логгер
        /// </summary>
        public IUserLog Log {
            get { return _log; }
            set { _log = value; }
        }

        public bool IsFinished {
            get { return State.HasFlag(TaskState.Finished); }
        }

        public bool IsError {
            get { return State.HasFlag(TaskState.Error); }
        }

        public bool IsSuccess {
            get { return TaskState.Success == (State & TaskState.Success); }
        }

        public  string ResolvePath(string src) {
            return EnvironmentInfo.ResolvePath(src, false, ResolvePathOverrides);
        }

        public IDictionary<string, string> ResolvePathOverrides {
            get { return Get<IDictionary<string, string>>("resolveoverrides", null); }
            set {
                Set("resolveoverrides",value);
            }
        } 

        public int RunCount { get; set; }

        public bool Execute() {
            if (IsFinished) {
                return true;
            }
            Initialize();
            if (RunOnce && HasUpdatedOnce()) {
                State = TaskState.SuccessOnce;
                return true;
            }
            if (!RequireExecution()) {
                State = TaskState.SuccessNotRun;
                return true;
            }
            foreach (var requiredModule in RequiredModules) {
                if (requiredModule.IsError) {
                    State = TaskState.CascadeError;
                    return true;
                }
                if (!requiredModule.IsFinished) {
                    return false;
                }
            }

            DoWork();
            return true;
        }

        public virtual void Refresh() {
            State = TaskState.Pending;
        }

        public virtual void Initialize(IJob package = null) {
            if (State == TaskState.Init) {
                Job = package;
                CheckoutParameters();

                State = TaskState.Pending;
                if (null != Requirements && 0 != Requirements.Length) {
                    if (null == package) {
                        throw new Exception("cannot resolve requirements without");
                    }
                    foreach (var requirement in Requirements) {
                        if (requirement.StartsWith("@")) {
                            var groupName = requirement.Substring(1);
                            foreach (var module in package.Tasks.Values) {
                                if (module.Group == groupName) {
                                    if (!RequiredModules.Contains(module)) {
                                        RequiredModules.Add(module);
                                    }
                                }
                            }
                        }
                        else {
                            if (package.Tasks.ContainsKey(requirement)) {
                                if (!RequiredModules.Contains(package.Tasks[requirement])) {
                                    RequiredModules.Add(package.Tasks[requirement]);
                                }
                            }
                            else {
                                throw new Exception("cannot resolve dependency from " + Name + " to " + requirement);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Обратная ссылка на пакет
        /// </summary>
        public IJob Job { get; set; }

        protected virtual void OnStateChange() {
        }

        protected virtual bool HasUpdatedOnce() {
            return false;
        }

        protected virtual void CheckoutParameters() {
        }

        protected void DoWork() {
            try {
                State = TaskState.Executing;
                PrepareWork();
                InternalWork();
                AfterWork();
                FixSuccess();
                State = TaskState.Success;
            }
            catch (Exception ex) {
                Error = ex;
                if (IgnoreErrors) {
                    State = TaskState.SuccessIgnoreErrors;
                }
                else {
                    State = TaskState.Error;
                    OnError();
                }
            }
        }

        protected virtual void PrepareWork() {
        }

        protected abstract void InternalWork();

        protected virtual void AfterWork() {
        }

        protected virtual void FixSuccess() {
        }

        protected virtual void OnError() {
        }

        protected virtual bool RequireExecution() {
            return true;
        }
    }
}