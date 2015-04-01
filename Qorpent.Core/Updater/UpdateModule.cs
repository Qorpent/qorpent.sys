using System;
using System.Collections.Generic;
using Qorpent.Log;
using Qorpent.Utils.IO;

namespace Qorpent.Updater
{
    [Flags]
    public enum UpdateModuleState {
        Init = 0,
        Pending = 1,
        Finished = 2,
        Error =Finished |4 ,
        CascadeError = Error | 8,
        Executing = 16, 
        Success = Finished |32,
        SuccessOnce = Success | 64,
        SuccessVersioned = Success | 128
    }
    

    /// <summary>
    /// Описывает абстракцию обновляемого модуля
    /// </summary>
    public abstract class UpdateModuleBase : IUpdateModule {
        /// <summary>
        /// Обший порядок выполнения
        /// </summary>
        public int Idx { get; set; }
        private IUserLog _log =  StubUserLog.Default;
        private readonly IList<IUpdateModule> _requiredModules = new List<IUpdateModule>();
        private UpdateModuleState _state;

        public string Name { get; set; }
        public string[] Requirements { get; set; }

        public UpdateModuleState State   {
            get { return _state; }
            protected set {
                _state = value;
                Log.Debug(Name+" : state changed -> "+value);
            }
        }

        public Exception Error { get; set; }

        public IList<IUpdateModule> RequiredModules {
            get { return _requiredModules; }
        }
        /// <summary>
        /// Признак одноразового применения модуля
        /// </summary>
        public bool UpdateOnce { get; set; }

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

        public bool IsFinished {
            get { return State.HasFlag(UpdateModuleState.Finished); }
        }

        public bool IsError {
            get { return State.HasFlag(UpdateModuleState.Error); }
        }

        public bool IsSuccess {
            get { return UpdateModuleState.Success == (State & UpdateModuleState.Success); }
        }

        public bool Execute() {
            if (IsFinished) return true;
            if (State == UpdateModuleState.Init) {
                Initialize();
                State = UpdateModuleState.Pending;
            }
            if (UpdateOnce && Target.Hash != "INIT")
            {
                State = UpdateModuleState.SuccessOnce;
                return true;
            }
            if (!IsNewer()) {
                State = UpdateModuleState.SuccessVersioned;
                return true;
            }
            foreach (var requiredModule in RequiredModules) {
                if (requiredModule.IsError)
                {
                    State = UpdateModuleState.CascadeError;
                    return true;
                }
                if (!requiredModule.IsFinished) {
                    return false;
                }
            }

            Update();
            return true;
        }

        protected void Update() {
            try {
                State = UpdateModuleState.Executing;
                PrepareUpdate();
                InternalUpdate();
                AfterUpdate();
                FixSuccess();
            }
            catch (Exception ex) {
                Error = ex;
                State = UpdateModuleState.Error;
                OnError();
            }
        }

        protected virtual void PrepareUpdate() {
            
        }

        protected abstract void InternalUpdate();

        protected virtual void AfterUpdate()
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

        protected virtual void Initialize() {
            
        }
    }
}
