using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Log;

namespace Qorpent.Tasks {
    public class Job : IJob {
        readonly IDictionary<string, ITask> _modules = new Dictionary<string, ITask>();
        private IUserLog _log = StubUserLog.Default;
        private int _maxIteration = 3;

        public IUserLog Log {
            get { return _log; }
            set { _log = value; }
        }

        public IDictionary<string, ITask> Modules {
            get { return _modules; }
        }
        /// <summary>
        /// Максимальное число итераций
        /// </summary>
        public int MaxIteration {
            get { return _maxIteration; }
            set { _maxIteration = value; }
        }

        protected bool wasInitialized = false;
    
        /// <summary>
        /// Выполнение 
        /// </summary>
        public void Execute() {
            Initialize();
            var workingQueue = new List<ITask>(Modules.Values.OrderBy(_=>_.Idx));          
            var iterations = MaxIteration;
            while (iterations > 0) {
                iterations --;
                var restQueue = new List<ITask>();
                if(workingQueue.Count==0)break;
                foreach (var module in workingQueue) {
                    var executed = module.Execute();
                    if (!executed) {
                        restQueue.Add(module);
                    }
                }
                workingQueue = restQueue;
            }
            if (workingQueue.Count != 0) {
                throw new Exception("cannot finish due to max iteration counter reached, maybe cycle dependency");
            }
        }

        public void Initialize(bool forced = false) {
            if (!wasInitialized || forced) {
                SetupLog();
                foreach (var module in _modules.Values) {
                    module.Initialize(this);
                }
                wasInitialized = true;
            }
           
        }

        public bool Success {
            get { return Modules.Values.All(_ => _.IsFinished && !_.IsError); }
        }

        public bool HasError {
            get { return Modules.Values.Any(_ => _.IsError); }
        }

       

        private void SetupLog() {
            if (StubUserLog.Default != Log) {
                foreach (var module in _modules.Values) {
                    if (null != module.Log && !module.Log.SubLoggers.Contains(Log)) {
                        module.Log.SubLoggers.Add(Log);
                    }
                    else {
                        module.Log = Log;
                    }
                }
            }
        }
    }
}