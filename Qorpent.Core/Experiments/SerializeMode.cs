using System;

namespace Qorpent.Experiments {
    /// <summary>
    /// ����� ������������
    /// </summary>
    [Flags]
    public enum SerializeMode {
        /// <summary>
        /// ���
        /// </summary>
        None =0,
        /// <summary>
        /// ������ �� ������ �� �������
        /// </summary>
        OnlyNotNull=1,
        /// <summary>
        /// ������������� � ����� ������
        /// </summary>
        Serialize = 2,
        /// <summary>
        /// ����������� ���
        /// </summary>
        Unknown = 4,
        LowerCase =8
    }
}