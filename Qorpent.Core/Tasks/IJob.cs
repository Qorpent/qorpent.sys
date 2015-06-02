using System.Collections.Generic;
using Qorpent.Log;

namespace Qorpent.Tasks {
    public interface IJob : IScope {
        IUserLog Log { get; set; }
        IDictionary<string, ITask> Tasks { get; }

        /// <summary>
        ///     ������������ ����� ��������
        /// </summary>
        int MaxIteration { get; set; }

        bool Success { get; }
        bool HasError { get; }

        /// <summary>
        ///     ����������
        /// </summary>
        void Execute();

        void Initialize(bool forced = false);
    }
}