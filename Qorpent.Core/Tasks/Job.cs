using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Log;

namespace Qorpent.Tasks {
    public class Job : IJob {
        private IUserLog _log = StubUserLog.Default;
        private int _maxIteration = 3;
        protected bool wasInitialized;
        private readonly IDictionary<string, ITask> _tasks = new Dictionary<string, ITask>();

        public IUserLog Log {
            get { return _log; }
            set { _log = value; }
        }

        public IDictionary<string, ITask> Tasks {
            get { return _tasks; }
        }

        /// <summary>
        ///     Максимальное число итераций
        /// </summary>
        public int MaxIteration {
            get { return _maxIteration; }
            set { _maxIteration = value; }
        }

        /// <summary>
        ///     Выполнение
        /// </summary>
        public void Execute() {
            Initialize();
            var workingQueue = new List<ITask>(Tasks.Values.OrderBy(_ => _.Idx));
            var iterations = MaxIteration;
            while (iterations > 0) {
                iterations --;
                var restQueue = new List<ITask>();
                if (workingQueue.Count == 0) {
                    break;
                }
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
        /// <summary>
        /// Инициализирует коллекцию задач
        /// </summary>
        /// <param name="forced"></param>
        public void Initialize(bool forced = false) {
            if (!wasInitialized || forced) {
                foreach (var module in Tasks) {
                    if (string.IsNullOrWhiteSpace(module.Value.Name)) {
                        module.Value.Name = module.Key;
                    }
                }
                SetupLog();
                foreach (var module in _tasks.Values) {
                    module.Initialize(this);
                    if (0 == module.Idx)
                    {
                        module.Idx = 1000000;
                    }
                }
                wasInitialized = true;
            }
        }

        public bool Success {
            get { return Tasks.Values.All(_ => _.IsFinished && !_.IsError); }
        }

        public bool HasError {
            get { return Tasks.Values.Any(_ => _.IsError); }
        }

        private void SetupLog() {
            if (StubUserLog.Default != Log) {
                foreach (var module in _tasks.Values) {
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