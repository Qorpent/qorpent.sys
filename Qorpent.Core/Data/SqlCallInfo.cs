using System;
using System.Data;
using System.Text;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data {
    /// <summary>
    /// ��������������� ��������� ������ �������
    /// </summary>
    public class SqlCallInfo {
        /// <summary>
        /// ������ �����������
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// prepared connection
        /// </summary>
        public IDbConnection Connection { get; set; }
        /// <summary>
        /// dialect of Sql to be used
        /// </summary>
        public SqlDialect Dialect { get; set; }

        public Exception Error { get; set; }
        /// <summary>
        /// test of valid state
        /// </summary>
        public bool Ok {
            get { return null == Error; }
        }


        /// <summary>
        /// calling notation
        /// </summary>
        public SqlCallNotation Notation { get; set; }
        /// <summary>
        /// type of object to be call
        /// </summary>
        public SqlObjectType ObjectType { get; set; }
        /// <summary>
        /// definition of parameters in valid order
        /// </summary>
        public SqlCallParameter[] Parameters { get; set; }
        /// <summary>
        /// Object to get parameter values from
        /// </summary>
        public object ParametersSoruce { get; set; }
        /// <summary>
        /// Name of object to wrap
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// Full query string
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Callback for handling Exceptions
        /// </summary>
        public Action<SqlCallInfo, Exception> OnError { get; set; }
        /// <summary>
        /// Callback for log messages from connection
        /// </summary>
        public Action<SqlCallInfo, string> OnMessage { get; set; }
        /// <summary>
        /// True - caller will send trace information
        /// </summary>
        public bool Trace { get; set; }
        /// <summary>
        /// Orm type for Object,SingleObject,Scalar
        /// </summary>
        public Type TargetType { get; set; }
        /// <summary>
        /// Orm types for MultipleObject
        /// </summary>
        public Type[] TargetTypes { get; set; }
        /// <summary>
        /// Object to handle result
        /// </summary>
        public object Result { get; set; }


        public IDbCommand PreparedCommand { get; set; }
        /// <summary>
        /// ����� ������ ����������
        /// </summary>
        public bool NoExecute { get; set; }


        /// <summary>
        /// ����� ��������� ��������� �� ����������� ������ ������� � ��� ����������
        /// </summary>
        /// <returns></returns>
        public SqlCallInfo GetNoQueryCopy() {
            var result = new SqlCallInfo {
                Connection = Connection,
                ConnectionString = ConnectionString,
                Trace = Trace,
                Dialect = Dialect,
                OnError = OnError,
                OnMessage = OnMessage
            };
            return result;
        }
    }
}