using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Log;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.IO;

namespace Qorpent.Tasks
{
    /// <summary>
    /// Описывает абстракцию обновляемого модуля
    /// </summary>
    public abstract class TaskBase : ITask {
        /// <summary>
        /// Обший порядок выполнения
        /// </summary>
        public int Idx { get; set; }
        private IUserLog _log =  StubUserLog.Default;
        private readonly IList<ITask> _requiredModules = new List<ITask>();
        private TaskState _state;

        public string Name { get; set; }
        public string[] Requirements { get; set; }

        public TaskState State   {
            get { return _state; }
            protected set {
                _state = value;
                Log.Debug(Name+" : state changed -> "+value);
            }
        }

        public Exception Error { get; set; }

        public IList<ITask> RequiredModules {
            get { return _requiredModules; }
        }
        /// <summary>
        /// Признак одноразового применения модуля
        /// </summary>
        public bool UpdateOnce { get; set; }

        /// <summary>
        /// Признак игнорирования ошибок
        /// </summary>
        public bool IgnoreErrors { get; set; }

        /// <summary>
        /// Группировка в модуль
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Исходный дескриптор
        /// </summary>
        public IVersionedDescriptor Source { get; set; }
        /// <summary>
        /// Целевой дескриптор
        /// </summary>
        public IVersionedDescriptor Target { get; set; }

        /// <summary>
        /// Логгер
        /// </summary>
        public IUserLog Log {
            get { return _log; }
            set { _log = value; }
        }

        /// <summary>
        /// Дополнительное определение
        /// </summary>
        public XElement Definition { get; set; }

        public bool IsFinished {
            get { return State.HasFlag(TaskState.Finished); }
        }

        public bool IsError {
            get { return State.HasFlag(TaskState.Error); }
        }

        public bool IsSuccess {
            get { return TaskState.Success == (State & TaskState.Success); }
        }

        public bool Execute() {
            if (IsFinished) return true;
            
            if (UpdateOnce && Target.Hash != "INIT")
            {
                State = TaskState.SuccessOnce;
                return true;
            }
            if (!IsNewer()) {
                State = TaskState.SuccessVersioned;
                return true;
            }
            foreach (var requiredModule in RequiredModules) {
                if (requiredModule.IsError)
                {
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

        public virtual void Initialize(IJob package = null) {
            if (State == TaskState.Init) {
                this.Package = package;
                if (null == Requirements || 0 == Requirements.Length) {
                    var requirements = Source.Header.Elements("require").ToArray();
                    this.Requirements = requirements.Select(_ => _.Attr("code")).ToArray();
                }
                UpdateOnce = UpdateOnce || Source.Header.Attr("updateonce").ToBool();
                IgnoreErrors = IgnoreErrors || Source.Header.Attr("ignoreerrors").ToBool();
                State = TaskState.Pending;
                if (0 != Requirements.Length) {
                    if (null == package) {
                        throw new Exception("cannot resolve requirements without");
                    }
                    foreach (var requirement in Requirements) {
                        if (requirement.StartsWith("@")) {
                            var groupName = requirement.Substring(1);
                            foreach (var module in package.Modules.Values) {
                                if (module.Group == groupName) {
                                    if (!RequiredModules.Contains(module))
                                    {
                                        RequiredModules.Add(module);
                                    }
                                }   
                            }
                        }
                        else {
                            if (package.Modules.ContainsKey(requirement)) {
                                if (!RequiredModules.Contains(package.Modules[requirement])) {
                                    RequiredModules.Add(package.Modules[requirement]);
                                }
                            }
                            else {
                                throw new Exception("cannot resolve dependency from "+this.Name+" to "+requirement);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Обратная ссылка на пакет
        /// </summary>
        public IJob Package { get; set; }

        protected void DoWork() {
            try {
                State = TaskState.Executing;
                PrepareWork();
                InternalWork();
                AfterWork();
                FixSuccess();
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

        protected virtual void AfterWork()
        {

        }

        protected virtual void FixSuccess()
        {

        }

        protected virtual void OnError()
        {

        }

        protected bool IsNewer() {
            if (Target.Hash == "INIT") return true;
            if (Target.Hash == Source.Hash) return false;
            return Target.Version > Source.Version;
        }

        
    }
}
