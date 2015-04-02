using System.Collections.Generic;
using Qorpent.Log;

namespace Qorpent.Tasks {
    public interface IJob {
        IUserLog Log { get; set; }
        IDictionary<string, ITask> Modules { get; }

        /// <summary>
        /// Максимальное число итераций
        /// </summary>
        int MaxIteration { get; set; }

        bool Success { get; }
        bool HasError { get; }

        /// <summary>
        /// Выполнение 
        /// </summary>
        void Execute();

        void Initialize(bool forced = false);
    }
}