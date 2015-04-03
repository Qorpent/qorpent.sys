using System;
using System.Data;

namespace Qorpent.Data {
    /// <summary>
    /// Унифицированный описатель вызова объекта
    /// </summary>
    public class DbCommandWrapper {
        /// <summary>
        /// строка подключения
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Название базы данных
        /// </summary>
        public string Database { get; set; }
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
        public DbCallNotation Notation { get; set; }
        /// <summary>
        /// type of object to be call
        /// </summary>
        public SqlObjectType ObjectType { get; set; }
        /// <summary>
        /// definition of parameters in valid order
        /// </summary>
        public DbParameter[] Parameters { get; set; }
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
        public Action<DbCommandWrapper, Exception> OnError { get; set; }
        /// <summary>
        /// Callback for log messages from connection
        /// </summary>
        public Action<DbCommandWrapper, string> OnMessage { get; set; }
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
        /// режим только подготовки
        /// </summary>
        public bool NoExecute { get; set; }


        /// <summary>
        /// Копия исходного контекста за исключением самого запроса и его результата
        /// </summary>
        /// <returns></returns>
        public DbCommandWrapper CloneNoQuery() {
            var result = new DbCommandWrapper {
                Connection = Connection,
                ConnectionString = ConnectionString,
                Database = Database,
                Trace = Trace,
                Dialect = Dialect,
                OnError = OnError,
                OnMessage = OnMessage
            };
            return result;
        }

        public bool IsPrepared {
            get { return null != PreparedCommand; }
        }
        /// <summary>
        /// флаг, что парамтеры из внешнего источника уже привязаны
        /// </summary>
        public bool ParametersBinded { get; set; }

        

        /// <summary>
        /// Формирует копию запроса с применением новых параметров
        /// </summary>
        /// <param name="parametersSource"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public DbCommandWrapper Clone(object parametersSource = null, IDbConnection connection = null)  {
            var noexecute = NoExecute;
            if (null != connection && null==this.Connection)
            {
                Connection = connection;
            }
            if (!IsPrepared) {
                NoExecute = true;
                DbCommandExecutor.Default.Execute(this).Wait();
            }
            NoExecute = noexecute;
            var result = (DbCommandWrapper)MemberwiseClone();
            result.Result = null;
            result.PreparedCommand = null;
            if (null != parametersSource) {
                result.ParametersSoruce = parametersSource;
                result.ParametersBinded = false;
            }
            
            return result;
        }
    }
}