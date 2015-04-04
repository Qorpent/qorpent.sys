using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Log;

namespace Qorpent.Tasks {
    public class Job : ConfigBase, IJob {
        private IUserLog _log = StubUserLog.Default;
        private int _maxIteration = 3;
        protected bool wasInitialized;
        private readonly IDictionary<string, ITask> _tasks = new Dictionary<string, ITask>();
        public XElement Definition { get; set; }

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
            Refresh();
            var workingQueue = new List<ITask>(Tasks.Values.OrderBy(_ => _.Idx));
            var iterations = MaxIteration;
            while (iterations > 0) {
                Log.Debug("cycle");
                iterations --;
                var restQueue = new List<ITask>();
                if (workingQueue.Count == 0) {
                    break;
                }
                foreach (var module in workingQueue) {
                    Log.Trace("call " + module.Name);
                    var executed = module.Execute();
                    if (!executed) {
                        Log.Trace("rest " + module.Name);
                        restQueue.Add(module);
                    }
                    else {
                        Log.Info("finish " + module.Name + " with " + module.State);
                    }
                }
                workingQueue = restQueue;
            }
            if (workingQueue.Count != 0) {
                throw new Exception("cannot finish due to max iteration counter reached, maybe cycle dependency");
            }
        }

        /// <summary>
        ///     Инициализирует коллекцию задач
        /// </summary>
        /// <param name="forced"></param>
        public void Initialize(bool forced = false) {
            if (!wasInitialized || forced) {
                Log.Debug("Start initialization", "Job", this);
                foreach (var module in Tasks) {
                    module.Value.SetParent(this);
                    if (string.IsNullOrWhiteSpace(module.Value.Name)) {
                        module.Value.Name = module.Key;
                    }
                }
                SetupLog();
                foreach (var module in _tasks.Values) {
                    Log.Debug("Begin initialize " + module.Name);
                    module.Initialize(this);
                    if (0 == module.Idx) {
                        module.Idx = 1000000;
                    }
                    Log.Debug("End initialize " + module.Name);
                }
                wasInitialized = true;
                Log.Debug("Finish initialization", "Job", this);
            }
        }

        public bool Success {
            get { return Tasks.Values.All(_ => _.IsFinished && !_.IsError); }
        }

        public bool HasError {
            get { return Tasks.Values.Any(_ => _.IsError); }
        }

        private void Refresh() {
            foreach (var value in Tasks.Values) {
                if (value.RunCount != 0) {
                    value.Refresh();
                }
                value.RunCount++;
            }
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