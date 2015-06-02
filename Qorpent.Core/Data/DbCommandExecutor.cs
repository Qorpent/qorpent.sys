using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Qorpent.IoC;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data {
	/// <summary>
	///     Asynchronous wrapper for Executing Sql Queries
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, Name="main.dbexecutor", ServiceType = typeof (IDbCommandExecutor))]
	public class DbCommandExecutor : ServiceBase, IDbCommandExecutor {
		private const string PrepareParametersQueryTemplate = @"
 select PARAMETER_NAME as name, DATA_TYPE as type from information_schema.parameters
where specific_name='{1}' and specific_schema = '{0}'
order by ORDINAL_POSITION
";

		private const string GetObjectTypeInfoQueryTemplate =
			@"select type from sys.objects where schema_id=SCHEMA_ID('{0}') and name = '{1}'";

		private static IDbCommandExecutor _default;
		public static IDbCommandExecutor Default {
			get {
				if (null == _default) {
					if (Applications.Application.HasCurrent) {
						_default = Applications.Application.Current.Container.Get<IDbCommandExecutor>("main.dbexecutor");
					}
					else {
						_default = new DbCommandExecutor();
						
					}
				}
				return _default;
				
			}
		}

		[Inject] public IDatabaseConnectionProvider ConnectionProvider;
	    private ILoggy _logger;

	    /// <summary>
		///     Setup for custom override - test or filtering propose
		/// </summary>
		public IDbCommandExecutor ProxyExecutor { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Inject(Name = "dbcommandexecutor.logger")]
        public ILoggy Logger {
            get {
                if (null == _logger) {
                    _logger = Loggy.Get("dbcommandexecutor.logger");
                }
                return _logger;
            }
            set { _logger = value; }
        }



		/// <summary>
		///     Производит асинхронный вызов Sql
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public async Task<DbCommandWrapper> Execute(DbCommandWrapper info) {
			try {
				if (null == info.PreparedCommand) {
					PrepareConnection(info);
					PrepareParameters(info);
					PrepareQuery(info);
				}
				else if (info.Trace) {
					Logger.Info("not required for prepare: ");
				}

				if (info.NoExecute) {
					if (info.Trace) {
						Logger.Info("no execution required: ");
					}
					return info;
				}
				return await InternalExecute(info);
			}
			catch (Exception ex) {
				if (info.Trace) {
					var str = ex.ToString();
					var len = str.Length;
					if (len > 500) {
						str = str.Substring(500);
					}
					Logger.Error("error in " + info + ": " + str, ex);
				}
				info.Error = ex;
				if (null != info.OnError) {
					info.OnError(info, ex);
				}
			}
			return info;
		}

		private Task<DbCommandWrapper> InternalExecute(DbCommandWrapper info) {
			if (null != ProxyExecutor) {
				if (info.Trace) {
					Logger.Info("redirect execution to proxy: " + info);
				}
				return ProxyExecutor.Execute(info);
			}
			return Task.Run(() => InternalExecuteSync(info));
		}

		private DbCommandWrapper InternalExecuteSync(DbCommandWrapper info) {
			if (info.Trace) {
				Logger.Info("begin execution: " + info);
			}
			var wasOpened = info.Connection.State == ConnectionState.Open;
			SqlInfoMessageEventHandler onMessage = null;
			try {
				if (null != info.OnMessage && info.Connection is SqlConnection) {
					onMessage = (o, a) => { info.OnMessage(info, a.Message); };
					((SqlConnection) info.Connection).InfoMessage += onMessage;
				}
				if (!wasOpened) {
					info.Connection.Open();
				}
			    if (!string.IsNullOrWhiteSpace(info.Database)) {
			        info.Connection.ChangeDatabase(info.Database);
			    }
				ExecuteCommand(info);
			}
			finally {
				if (!wasOpened) {
					info.Connection.Close();
				}
				if (null != onMessage) {
					((SqlConnection) info.Connection).InfoMessage -= onMessage;
				}
			}
			if (info.Trace) {
				Logger.Info("end execution: " + info);
                
			}
			return info;
		}

		private void ExecuteCommand(DbCommandWrapper info) {
			if (info.Notation == DbCallNotation.None) {
				info.PreparedCommand.ExecuteNonQuery();
			}
			else if (info.Notation == DbCallNotation.Scalar) {
				var result = info.PreparedCommand.ExecuteScalar();
				info.Result = result;
			}
			else if (info.Notation.HasFlag(DbCallNotation.Reader)) {
				using (var reader = info.PreparedCommand.ExecuteReader()) {
					if (info.Notation.HasFlag(DbCallNotation.Single)) {
						if (reader.Read()) {
							info.Result = SetupSingleResult(info, reader);
						}
					}
					
					else if (info.Notation.HasFlag(DbCallNotation.Multiple)) {
						var resultNumber = 0;
						var resultAdvanced = false;
						var result = new List<object>();
						var current = new List<object>();
						
						while ((reader.Read())||(resultAdvanced  = reader.NextResult())) {
							if (resultAdvanced) {
								resultAdvanced = false;
								resultNumber++;
								result.Add(current.ToArray());
								current = new List<object>();
								continue;
							}
							current.Add(SetupSingleResult(info,reader,resultNumber));
						}
						result.Add(current.ToArray());
						info.Result = result.ToArray();
					}
					else
					{
						var result = new List<object>();
						while (reader.Read())
						{
							result.Add(SetupSingleResult(info, reader));
						}
						info.Result = result.ToArray();
					}
				}
			}
		}

		private object SetupSingleResult(DbCommandWrapper info, IDataReader reader, int resultNumber = 0) {
			var result = new Dictionary<string, object>();
			for (var i = 0; i < reader.FieldCount; i++)
			{
				var name = reader.GetName(i);
				var value = reader.GetValue(i);
				if (DBNull.Value == value)
				{
					value = null;
				}
				result[name] = value;
			}
			if (info.Notation == DbCallNotation.ObjectReader || info.Notation == DbCallNotation.SingleObject ||
				info.Notation == DbCallNotation.MultipleObject) {
				var type = info.TargetType;
				if (info.Notation == DbCallNotation.MultipleObject) {
					if (null != info.TargetTypes && 0 != info.TargetTypes.Length) {
						if (resultNumber < info.TargetTypes.Length) {
							type = info.TargetTypes[resultNumber];
						}
						else {
							throw new Exception("invalid types count for multiple ORM");
						}
					}
				}
				if (null == type) {
					throw new Exception("no type was given for ORM call notation");
				}
				foreach (var source in result.ToArray()) {
					result[source.Key.ToLowerInvariant()] = source.Value;
				   
				}
				var ormresult = Activator.CreateInstance(type);
				var properties = type.GetProperties();
				foreach (var propertyInfo in properties) {
					if (propertyInfo.CanWrite) {
						if (result.ContainsKey(propertyInfo.Name.ToLowerInvariant())) {
							var srcvalue = result[propertyInfo.Name.ToLowerInvariant()];
							var resultvalue = srcvalue.ToTargetType(propertyInfo.PropertyType, true);
							propertyInfo.SetValue(ormresult, resultvalue);
						}
					}
				}
				return ormresult;
			}
			return result;
		}

		private void PrepareQuery(DbCommandWrapper info) {
			if (info.Trace) {
				Logger.Info("begin prepare query: " + info);
			}

			if (string.IsNullOrWhiteSpace(info.Query)) {
				if (string.IsNullOrWhiteSpace(info.ObjectName)) {
					throw new Exception("cannot setup query - no query or objectname");
				}
				if (info.Dialect != DbDialect.SqlServer) {
					throw new Exception("cannot setup query for non-TSql");
				}
				PrepareTSqlObjectQuery(info);
			}

			var cmd = info.PreparedCommand = info.Connection.CreateCommand();
			cmd.CommandText = info.Query;

			StoreParametersFromSource(info);
			foreach (var parameter in info.Parameters) {
				var p = cmd.CreateParameter();
				
				var name = parameter.Name;
				if (p is SqlParameter) {
					name = "@" + parameter.Name;
				}
				p.ParameterName = name;
				if (parameter.DbType != default(DbType)) {
					p.DbType = parameter.DbType;
				}
				if (p.DbType == DbType.AnsiString || p.DbType == DbType.String) {
					p.Size = -1;
				}
				if (p.DbType == DbType.Decimal) {
					p.Size = 18;
					p.Precision = 6;
				}
				p.Value = parameter.Value ?? DBNull.Value;
				cmd.Parameters.Add(p);
			}


			if (info.Trace) {
				Logger.Info("end prepare query: " + info);
			}
		}

		private void PrepareTSqlObjectQuery(DbCommandWrapper info) {
			if (info.ObjectType == SqlObjectType.None) {
				DetectObjectType(info);
			}
			var q = new StringBuilder();
			if (info.ObjectType == SqlObjectType.Function || info.ObjectType == SqlObjectType.TableFunction) {
				if (info.ObjectType == SqlObjectType.TableFunction) {
					q.Append("SELECT * FROM ");
				}
				else {
					q.Append("SELECT ");
				}
				q.Append(info.ObjectName);
				q.Append("(");
				var first = true;
				foreach (var parameter in info.Parameters) {
					if (!first) {
						q.Append(", ");
					}
					first = false;
					q.Append("@");
					q.Append(parameter.Name);
				}
				q.Append(")");
			}
			else {
				q.Append("EXEC ");
				q.Append(info.ObjectName);
				q.Append(" ");
				var first = true;
				foreach (var parameter in info.Parameters) {
					if (!first) {
						q.Append(", ");
					}
					first = false;
					q.Append("@");
					q.Append(parameter.Name);
					q.Append("=@");
					q.Append(parameter.Name);
				}
			}
			info.Query = q.ToString();
		}

		private void DetectObjectType(DbCommandWrapper info) {
			var schema = "dbo";
			var name = info.ObjectName;
			if (name.Contains(".")) {
				schema = name.Split('.')[0];
				name = name.Split('.')[1];
			}
			var query = string.Format(GetObjectTypeInfoQueryTemplate, schema, name);
			if (null == ProxyExecutor) {
				DetectTypeByOwnConnection(info, query);
			}
			else {
				var callProxy = info.CloneNoQuery();
				callProxy.Query = query;
				callProxy.Notation = DbCallNotation.Scalar;
				ProxyExecutor.Execute(callProxy).Wait();
				SetupObjectTypeValue(info, callProxy.Result);
			}
		}

		private void PrepareParameters(DbCommandWrapper info) {
			if (info.Trace) {
				Logger.Info("begin prepare parameters: " + info);
			}
			//если набор парамтеров определен при вызове - не требуется подготавливать параметры
			if (null != info.Parameters) {
				return;
			}
			if (!string.IsNullOrWhiteSpace(info.Query)) {
				PrepareParametersByQuery(info);
			}
			else if (!string.IsNullOrWhiteSpace(info.ObjectName)) {
				PrepareParametersByObject(info);
			}

			StoreParametersFromSource(info);

			if (info.Trace) {
				Logger.Info("end prepare parameters: " + info);
			}
		}

		private void StoreParametersFromSource(DbCommandWrapper info) {
			if (info.ParametersBinded) return;
			if (null == info.ParametersSoruce) return;
			var dictionary = info.ParametersSoruce.ToDict();
			foreach (var o in dictionary.ToArray()) {
				dictionary[o.Key.ToLowerInvariant()] = o.Value;
			}
			foreach (var pd in info.Parameters) {
				pd.Value = null;
				if (dictionary.ContainsKey(pd.Name.ToLowerInvariant())) {
					pd.Value = dictionary[pd.Name.ToLowerInvariant()];
				}
			}
			info.ParametersBinded = true;
		}

		private void PrepareParametersByObject(DbCommandWrapper info) {
		 
			
			if (info.Dialect != DbDialect.SqlServer) {
				throw new Exception("cannot setup parameters for non-TSql query");
			}
			var schema = "dbo";
			var name = info.ObjectName;
			if (name.Contains(".")) {
				schema = name.Split('.')[0];
				name = name.Split('.')[1];
			}

			var query = string.Format(PrepareParametersQueryTemplate, schema, name);
			if (null == ProxyExecutor) {
				SetupByOwnConnection(info, query);
			}
			else {
				SetupByProxy(info, query);
			}

		}

		private void SetupByProxy(DbCommandWrapper info, string query) {
			var proxyCall = info.CloneNoQuery();
			proxyCall.Query = query;
			proxyCall.Notation = DbCallNotation.Reader;

			ProxyExecutor.Execute(proxyCall).Wait();
			if (proxyCall.Ok) {
				var result = (object[]) proxyCall.Result;
				var parameters = new List<DbParameter>();
				foreach (IDictionary<string, object> record in result) {
					parameters.Add(SetupParameter(record["name"].ToStr(), record["type"].ToStr()));
				}
				info.Parameters = parameters.ToArray();
			}
			else {
				throw proxyCall.Error;
			}
		}

		private static void DetectTypeByOwnConnection(DbCommandWrapper info, string query) {
			var connecitonIsOpened = info.Connection.State == ConnectionState.Open;
			try {
				if (!connecitonIsOpened) {
					info.Connection.Open();
				}
			    if (!string.IsNullOrWhiteSpace(info.Database)) {
			        info.Connection.ChangeDatabase(info.Database);
			    }
				var cmd = info.Connection.CreateCommand();
				cmd.CommandText = query;
				var result = cmd.ExecuteScalar();
				SetupObjectTypeValue(info, result);
			}
			finally {
				if (!connecitonIsOpened) {
					info.Connection.Close();
				}
			}
		}

		private static void SetupObjectTypeValue(DbCommandWrapper info, object result) {
			if (null == result || DBNull.Value == result) {
				throw new Exception("cannot detect object type for " + info.ObjectName);
			}
			var type = result.ToStr().Trim();
			if (type == "P") {
				info.ObjectType = SqlObjectType.Procedure;
			}
			else if (type == "F") {
				info.ObjectType = SqlObjectType.Function;
			}
			else if (type == "TF") {
				info.ObjectType = SqlObjectType.TableFunction;
			}
		}

		private static void SetupByOwnConnection(DbCommandWrapper info, string query) {
			var connecitonIsOpened = info.Connection.State == ConnectionState.Open;
			try {
				if (!connecitonIsOpened) {
					info.Connection.Open();
				}
			    if (!string.IsNullOrWhiteSpace(info.Database)) {
			        info.Connection.ChangeDatabase(info.Database);
			    }
				var cmd = info.Connection.CreateCommand();
				cmd.CommandText = query;
				var parameters = new List<DbParameter>();
				using (var reader = cmd.ExecuteReader()) {
					while (reader.Read()) {
						var name = reader.GetString(0);
						var type = reader.GetString(1);
						parameters.Add(SetupParameter(name, type));
					}
				}
				info.Parameters = parameters.ToArray();
			}
			finally {
				if (!connecitonIsOpened) {
					info.Connection.Close();
				}
			}
		}

		private static DbParameter SetupParameter(string name, string type) {
			var parameter = new DbParameter {Name = name};
			if (parameter.Name.StartsWith("@")) {
				parameter.Name = parameter.Name.Substring(1);
			}
			if (type == "int") {
				parameter.DbType = DbType.Int64;
			}
			else if (type.EndsWith("varchar")) {
				parameter.DbType = DbType.String;
			}
			else if (type == "bit") {
				parameter.DbType = DbType.Boolean;
			}
			else if (type == "datetime") {
				parameter.DbType = DbType.DateTime;
			}
			return parameter;
		}

		private void PrepareParametersByQuery(DbCommandWrapper info) {

		    
		    if (Regex.IsMatch(info.Query, @"(?i)create\s+((proc)|(func))")) {
		        info.Parameters = new DbParameter[] {};
                return;
		        
		    }

			if (info.Dialect != DbDialect.SqlServer) {
				throw new Exception("cannot setup parameters for non-TSql query");
			}

			var references = Regex.Matches(info.Query, @"@[\w\d]+").OfType<Match>().Select(_ => _.Value.Substring(1))
				.Distinct().ToList();
		    foreach (var r in references.ToArray()) {
		        if (Regex.IsMatch(info.Query, @"(?i)declare\s+@" + r + @"\s+")) {
		            references.Remove(r);
		        }
		    }
			info.Parameters = references.Select(_ => new DbParameter {Name = _}).ToArray();

		}

		private void PrepareConnection(DbCommandWrapper info) {
			if (info.Trace) {
				Logger.Info("begin prepare connection: " + info);
			}
			if (null == info.Connection) {
				if (string.IsNullOrWhiteSpace(info.ConnectionString)) {
					info.ConnectionString = "default";
				}
				if (null == ConnectionProvider) {
				    if (info.ConnectionString == "default") {
				        info.ConnectionString = "Server=(local);Trusted_Connection=true";
				    }
					if (info.ConnectionString.Contains(";")) {
						info.Connection = new SqlConnection(info.ConnectionString);
					}
					else {
						throw new Exception("no connection provider setup");
					}
				}
				else {
					info.Connection = ConnectionProvider.GetConnection(info.ConnectionString);
				}
				if (null == info.Connection)
				{
					throw new Exception("cannot retrieve connection for " + info.ConnectionString);
				}
			}
			if (info.Dialect == DbDialect.None) {
				if (info.Connection is SqlConnection) {
					info.Dialect = DbDialect.SqlServer;
				}
				else if (info.Connection.GetType().Name.ToLowerInvariant().Contains("pg")) {
					info.Dialect = DbDialect.PostGres;
				}
				else {
					info.Dialect = DbDialect.Ansi;
				}
			}
			if (info.Trace) {
				Logger.Info("end prepare connection: " + info);
			}
		}
	}
}