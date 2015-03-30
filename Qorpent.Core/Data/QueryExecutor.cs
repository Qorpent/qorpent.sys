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
using Qorpent.Utils.Extensions;

namespace Qorpent.Data {
	/// <summary>
	///     Asynchronous wrapper for Executing Sql Queries
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof (ISqlQueryExecutor))]
	public class SqlQueryExecutor : ServiceBase, ISqlQueryExecutor {
		private const string PrepareParametersQueryTemplate = @"
 select PARAMETER_NAME as name, DATA_TYPE as type from information_schema.parameters
where specific_name='{1}' and specific_schema = '{0}'
order by ORDINAL_POSITION
";

		private const string GetObjectTypeInfoQueryTemplate =
			@"select type from sys.objects where schema_id=SCHEMA_ID('{0}') and name = '{1}'";

		[Inject] public IDatabaseConnectionProvider ConnectionProvider;

		/// <summary>
		///     Setup for custom override - test or filtering propose
		/// </summary>
		public ISqlQueryExecutor ProxyExecutor { get; set; }

		/// <summary>
		///     Производит асинхронный вызов Sql
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public async Task<SqlCallInfo> Execute(SqlCallInfo info) {
			try {
				if (null == info.PreparedCommand) {
					PrepareConnection(info);
					PrepareParameters(info);
					PrepareQuery(info);
				}
				else if (info.Trace) {
					Trace.TraceInformation("not required for prepare: " + info);
				}

				if (info.NoExecute) {
					if (info.Trace) {
						Trace.TraceInformation("no execution required: " + info);
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
					Trace.TraceError("error in " + info + ": " + str);
				}
				info.Error = ex;
				if (null != info.OnError) {
					info.OnError(info, ex);
				}
			}
			return info;
		}

		private Task<SqlCallInfo> InternalExecute(SqlCallInfo info) {
			if (null != ProxyExecutor) {
				if (info.Trace) {
					Trace.TraceInformation("redirect execution to proxy: " + info);
				}
				return ProxyExecutor.Execute(info);
			}
			return Task.Run(() => InternalExecuteSync(info));
		}

		private SqlCallInfo InternalExecuteSync(SqlCallInfo info) {
			if (info.Trace) {
				Trace.TraceInformation("begin execution: " + info);
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
				Trace.TraceInformation("end execution: " + info);
			}
			return info;
		}

		private void ExecuteCommand(SqlCallInfo info) {
			if (info.Notation == SqlCallNotation.None) {
				info.PreparedCommand.ExecuteNonQuery();
			}
			else if (info.Notation == SqlCallNotation.Scalar) {
				var result = info.PreparedCommand.ExecuteScalar();
				info.Result = result;
			}
			else if (0 != (info.Notation & SqlCallNotation.ReaderBased)) {
				using (var reader = info.PreparedCommand.ExecuteReader()) {
					if (info.Notation == SqlCallNotation.SingleRow || info.Notation == SqlCallNotation.SingleObject) {
						if (reader.Read()) {
							info.Result = SetupSingleResult(info, reader);
						}
					}
					else if (info.Notation == SqlCallNotation.Reader || info.Notation == SqlCallNotation.ObjectReader) {
						var result = new List<object>();
						while (reader.Read()) {
							result.Add(SetupSingleResult(info,reader));
						}
						info.Result = result.ToArray();
					}
					else if (info.Notation == SqlCallNotation.MultipleReader ||
							 info.Notation == SqlCallNotation.MultipleObject) {
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
				}
			}
		}

		private object SetupSingleResult(SqlCallInfo info, IDataReader reader, int resultNumber = 0) {
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
			if (info.Notation == SqlCallNotation.ObjectReader || info.Notation == SqlCallNotation.SingleObject ||
				info.Notation == SqlCallNotation.MultipleObject) {
				var type = info.TargetType;
				if (info.Notation == SqlCallNotation.MultipleObject) {
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

		private void PrepareQuery(SqlCallInfo info) {
			if (info.Trace) {
				Trace.TraceInformation("begin prepare query: " + info);
			}

			if (string.IsNullOrWhiteSpace(info.Query)) {
				if (string.IsNullOrWhiteSpace(info.ObjectName)) {
					throw new Exception("cannot setup query - no query or objectname");
				}
				if (info.Dialect != SqlDialect.SqlServer) {
					throw new Exception("cannot setup query for non-TSql");
				}
				PrepareTSqlObjectQuery(info);
			}

			var cmd = info.PreparedCommand = info.Connection.CreateCommand();
			cmd.CommandText = info.Query;
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
				Trace.TraceInformation("end prepare query: " + info);
			}
		}

		private void PrepareTSqlObjectQuery(SqlCallInfo info) {
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

		private void DetectObjectType(SqlCallInfo info) {
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
				var callProxy = info.GetNoQueryCopy();
				callProxy.Query = query;
				callProxy.Notation = SqlCallNotation.Scalar;
				ProxyExecutor.Execute(callProxy).Wait();
				SetupObjectTypeValue(info, callProxy.Result);
			}
		}

		private void PrepareParameters(SqlCallInfo info) {
			if (info.Trace) {
				Trace.TraceInformation("begin prepare parameters: " + info);
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
				Trace.TraceInformation("end prepare parameters: " + info);
			}
		}

		private void StoreParametersFromSource(SqlCallInfo info) {
		    if (null == info.ParametersSoruce) return;
			var dictionary = info.ParametersSoruce.ToDict();
			foreach (var o in dictionary.ToArray()) {
				dictionary[o.Key.ToLowerInvariant()] = o.Value;
			}
			foreach (var pd in info.Parameters) {
				if (dictionary.ContainsKey(pd.Name.ToLowerInvariant())) {
					pd.Value = dictionary[pd.Name.ToLowerInvariant()];
				}
			}
		}

		private void PrepareParametersByObject(SqlCallInfo info) {
			if (info.Dialect != SqlDialect.SqlServer) {
				throw new Exception("cannot setup parameters for non-TSql query");
			}
			var schema = "dbo";
			var name = info.ObjectName;
			if (name.Contains(".")) {
				schema = name.Split('.')[0];
				name = name.Split('.')[0];
			}

			var query = string.Format(PrepareParametersQueryTemplate, schema, name);
			if (null == ProxyExecutor) {
				SetupByOwnConnection(info, query);
			}
			else {
				SetupByProxy(info, query);
			}
		}

		private void SetupByProxy(SqlCallInfo info, string query) {
			var proxyCall = info.GetNoQueryCopy();
			proxyCall.Query = query;
			proxyCall.Notation = SqlCallNotation.Reader;

			ProxyExecutor.Execute(proxyCall).Wait();
			if (proxyCall.Ok) {
				var result = (object[]) proxyCall.Result;
				var parameters = new List<SqlCallParameter>();
				foreach (IDictionary<string, object> record in result) {
					parameters.Add(SetupParameter(record["name"].ToStr(), record["type"].ToStr()));
				}
				info.Parameters = parameters.ToArray();
			}
			else {
				throw proxyCall.Error;
			}
		}

		private static void DetectTypeByOwnConnection(SqlCallInfo info, string query) {
			var connecitonIsOpened = info.Connection.State == ConnectionState.Open;
			try {
				if (!connecitonIsOpened) {
					info.Connection.Open();
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

		private static void SetupObjectTypeValue(SqlCallInfo info, object result) {
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

		private static void SetupByOwnConnection(SqlCallInfo info, string query) {
			var connecitonIsOpened = info.Connection.State == ConnectionState.Open;
			try {
				if (!connecitonIsOpened) {
					info.Connection.Open();
				}
				var cmd = info.Connection.CreateCommand();
				cmd.CommandText = query;
				var parameters = new List<SqlCallParameter>();
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

		private static SqlCallParameter SetupParameter(string name, string type) {
			var parameter = new SqlCallParameter {Name = name};
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

		private void PrepareParametersByQuery(SqlCallInfo info) {
			if (info.Dialect != SqlDialect.SqlServer) {
				throw new Exception("cannot setup parameters for non-TSql query");
			}

			var references = Regex.Matches(info.Query, @"@[\w\d]+").OfType<Match>().Select(_ => _.Value.Substring(1))
				.Distinct();
			info.Parameters = references.Select(_ => new SqlCallParameter {Name = _}).ToArray();
		}

		private void PrepareConnection(SqlCallInfo info) {
			if (info.Trace) {
				Trace.TraceInformation("begin prepare connection: " + info);
			}
			if (null == info.Connection) {
				if (string.IsNullOrWhiteSpace(info.ConnectionString)) {
					info.ConnectionString = "default";
				}
			    if (null == ConnectionProvider) {
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
			if (info.Dialect == SqlDialect.None) {
				if (info.Connection is SqlConnection) {
					info.Dialect = SqlDialect.SqlServer;
				}
				else if (info.Connection.GetType().Name.ToLowerInvariant().Contains("pg")) {
					info.Dialect = SqlDialect.PostGres;
				}
				else {
					info.Dialect = SqlDialect.Ansi;
				}
			}
			if (info.Trace) {
				Trace.TraceInformation("end prepare connection: " + info);
			}
		}
	}
}