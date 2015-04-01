using System;
using System.Collections.Generic;

namespace Qorpent.Updater {
    public interface IUpdateModule {
        string Name { get; set; }
        string[] Requirements { get; set; }
        UpdateModuleState State { get; }
        IList<IUpdateModule> RequiredModules { get; }
        bool IsFinished { get; }
        bool IsError { get; }
        bool IsSuccess { get; }
        Exception Error { get; set; }

        /// <summary>
        /// Обший порядок выполнения
        /// </summary>
        int Idx { get; set; }

        bool Execute();
    }
}