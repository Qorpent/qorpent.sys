using System.Collections.Generic;
using Qorpent.Log;

namespace Qorpent.Tasks {
    public interface IJob {
        IUserLog Log { get; set; }
        IDictionary<string, ITask> Modules { get; }

        /// <summary>
        /// ������������ ����� ��������
        /// </summary>
        int MaxIteration { get; set; }

        bool Success { get; }
        bool HasError { get; }

        /// <summary>
        /// ���������� 
        /// </summary>
        void Execute();

        void Initialize(bool forced = false);
    }
}