using System;
using System.Data;

namespace Qorpent.Data {
    /// <summary>
    /// Описатель параметра
    /// </summary>
    public class DbParameter {
        public string Name { get; set; }
        public DbType DbType { get; set; }
        public object Value { get; set; }
    }
}