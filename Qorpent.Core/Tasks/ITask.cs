using System;
using System.Collections.Generic;
using Qorpent.Log;

namespace Qorpent.Tasks {
    public interface ITask {
        string Name { get; set; }
        string[] Requirements { get; set; }
        TaskState State { get;  }
        IList<ITask> RequiredModules { get; }
        bool IsFinished { get; }
        bool IsError { get; }
        bool IsSuccess { get; }
        Exception Error { get; set; }

        /// <summary>
        ///     ����� ������� ����������
        /// </summary>
        int Idx { get; set; }

        /// <summary>
        ///     ������
        /// </summary>
        IUserLog Log { get; set; }

        /// <summary>
        ///     �������� ������ �� �����
        /// </summary>
        IJob Job { get; set; }

        /// <summary>
        ///     ����������� � ������
        /// </summary>
        string Group { get; set; }

        int RunCount { get; set; }
        IDictionary<string, object> Data { get; }

        bool Execute();
        void Initialize(IJob package);
        void Refresh();
    }
}