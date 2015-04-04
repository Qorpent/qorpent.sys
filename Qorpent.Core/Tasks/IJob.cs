using System.Collections.Generic;
using Qorpent.Config;
using Qorpent.Log;

namespace Qorpent.Tasks {
    public interface IJob : IConfig {
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