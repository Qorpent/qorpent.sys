using System;
using System.Data;

namespace Qorpent.Data {
    /// <summary>
    /// ��������� ���������
    /// </summary>
    public class SqlCallParameter {
        public string Name { get; set; }
        public DbType DbType { get; set; }
        public object Value { get; set; }
    }
}