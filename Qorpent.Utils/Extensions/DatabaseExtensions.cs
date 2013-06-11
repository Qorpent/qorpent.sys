#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Utils/DbExtensions.cs
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Qorpent.Utils.Extensions
{
	/// <summary>
    /// ���������� ��� ������ � ��������� � ������������ ��
    /// </summary>
    public static class DatabaseExtensions
    {
		/// <summary>
		/// ����������� ���� ������� �� �� ������ DbCommand
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public static DatabaseEngineType DetermineDbType(this IDbCommand command) {
			if (null != command.Connection) {
				return command.Connection.DetermineDbType();
			}
			var typename = command.GetType().Name;
			switch (typename) {
				case "SqlCommand":
					return DatabaseEngineType.SqlServer;
				case "NpgsqlCommand":
					return DatabaseEngineType.Postgres;
				case "MySqlCommand":
					return DatabaseEngineType.MySql;
				case "OracleCommand":
					return DatabaseEngineType.Oracle;
				default:
					return DatabaseEngineType.Undefined;
			}
		}
		/// <summary>
		/// ����������� ���� ������� �� �� ������ DbConnection
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public static DatabaseEngineType DetermineDbType(this IDbConnection connection) {
			var typename = connection.GetType().Name;
			switch (typename)
			{
				case "SqlConnection":
					return DatabaseEngineType.SqlServer;
				case "NpgsqlConnection":
					return DatabaseEngineType.Postgres;
				case "MySqlConnection":
					return DatabaseEngineType.MySql;
				case "OracleConnection":
					return DatabaseEngineType.Oracle;
				default:
					return DatabaseEngineType.Undefined;
			}
		}
       

		/// <summary>
		/// ��������� ������ ��� �����������
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <param name="timeout"></param>
		/// <exception cref="Exception"></exception>
		public static void ExecuteNonQuery(this IDbConnection connection, object command,
										   object parameters = null, int timeout=30)
		{
			connection.WellOpen();
			IDbCommand cmd = connection.CreateCommand(command, parameters,timeout);
			cmd.CommandTimeout = timeout;
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw new Exception("error in query:" + cmd.CommandText, ex);
			}
		}


	    /// <summary>
	    /// �������� ������ �� ��������� ��������
	    /// </summary>
	    /// <param name="connection"></param>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <param name="defValue"></param>
	    /// <param name="timeout"></param>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    /// <exception cref="ArgumentNullException"></exception>
	    public static T ExecuteScalar<T>(this IDbConnection connection, object command,
                                         object parameters=null, T defValue=default(T) , int timeout = 30)
        {
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var result = connection.CreateCommand(command, parameters, timeout).ExecuteScalar();
            if (null == result) return defValue;
            if (result is DBNull) return defValue;
            return result.To<T>();
        }

	    /// <summary>
	    /// ��������� ������ �� "������" - ��������������� ������ ��������
	    /// </summary>
	    /// <param name="connection"></param>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <param name="timeout"></param>
	    /// <returns></returns>
	    public static object[] ExecuteRow(this IDbConnection connection, object command,object parameters=null, int timeout=30)
        {
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var reader = connection.CreateCommand(command, parameters, timeout).ExecuteReader(CommandBehavior.SingleRow);
            try
            {
                if (reader.Read())
                {
                    var result = new object[reader.FieldCount];
                    reader.GetValues(result);
                    return result;
                }
            }
            finally
            {
                reader.Close();
            }
            return null;
        }

        /// <summary>
        /// ���������� ����� ���������������� �������� ����������
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDbConnection WellOpen(this IDbConnection connection){
            if (null == connection) return connection;
            if (ConnectionState.Open == connection.State) return connection;
            if (ConnectionState.Closed == connection.State){
                try {
                    connection.Open();
                    return connection;
                }catch(Exception ex) {
                    throw new Exception("cannot connect "+connection.ConnectionString,ex);
                }
            }
            // TODO: ���� ����� Exception �� �� ���� ������ �� ������ 
            throw new InvalidOperationException("Insuficient connection state " + connection.State);
        }


	    /// <summary>
	    /// ������� ������� � �����������
	    /// </summary>
	    /// <param name="connection"></param>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <param name="timeout"></param>
	    /// <returns></returns>
	    public static IDbCommand CreateCommand(this IDbConnection connection, object command, object parameters=null, int timeout =30){
            
			if (command is UniSqlQuery) {
				var query =  ((UniSqlQuery) command).PrepareCommand(connection, parameters);
				query.CommandTimeout = timeout;
				return query;
			}
		    else {
				var query = connection.CreateCommand();
				query.CommandTimeout = timeout;
			    var strcommand = command.ToString();
				var realcommand = RewriteSql(strcommand, DetermineDbType(connection));
			    query.CommandText = realcommand;
			    if (null != parameters) {
				    
						PrepareParameters(realcommand, parameters.ToDict(), query);
				    
			    }
			    return query;
		    }
	    }

		/// <summary>
		/// Rewrites unified sql to match different database notations
		/// </summary>
		/// <param name="command"></param>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		/// <remarks>
		/// source
		/// UNICALL MY.NAME | a = ~x , b = ~x , ~z  
		/// where tilda ~ marks named parameters
		/// will be converted to :
		/// exec MY.NAME @a=@x, @b=@y, @z in T-SQL
		/// select MY.NAME ( a := :x, b := :y, :z ) in PG 
		/// select MY.NAME ( a => :x, b=>:y, :z) in ORACLE
		/// CALL MY.NAME ( ?x, ?y, ?z) in MySQL (doesn't support named)
		/// </remarks>
		public static string RewriteSql(string command, DatabaseEngineType dbtype) {
			var result = "";
			if (command.StartsWith("UNI")) {
				result = RewriteSqlByUniNotation(command, dbtype);
			}
			else {
				result = RewriteWithSimpleUniNotation(command, dbtype);
			}
			//for MySql need correction due it's not support schemas
			if (dbtype == DatabaseEngineType.MySql) {
				result = Regex.Replace(result,@"(\w+)\.`([^`]+)`", "`$1_$2`", RegexOptions.Compiled);
			}
			return result;
		}

		private static string RewriteWithSimpleUniNotation(string command, DatabaseEngineType dbtype) {
			var paramchar = GetPrefixOfOuterParameter(dbtype);
			var realcommand = command;
			if (realcommand.Contains("~")) {
				realcommand = command.Replace("~", paramchar.ToString());
			}
			switch (dbtype) {
				case DatabaseEngineType.Postgres:
					goto case DatabaseEngineType.Oracle;
				case DatabaseEngineType.Oracle:
					realcommand = realcommand.Replace("[", "\"");
					realcommand = realcommand.Replace("]", "\"");
					break;
				case DatabaseEngineType.MySql:
					realcommand = realcommand.Replace("[", "`");
					realcommand = realcommand.Replace("]", "`");
					break;
			}
			return realcommand;
		}

		private static string RewriteSqlByUniNotation(string command, DatabaseEngineType dbtype) {
			var mainsplit = command.Split('|');
			var commandAndName = mainsplit[0].Split(' ');
			var paramlist = "";
			if (mainsplit.Length == 2) {
				paramlist =mainsplit[1].Trim();
			}
			var requireBraces = IsRequireBracesToCall(dbtype);
			var cmdbuilder = new StringBuilder();
			cmdbuilder.Append(GetCallCommandPrefix(DetermineCommandType(commandAndName[0].Trim()), dbtype));
			cmdbuilder.Append(" ");
			cmdbuilder.Append(GetQuotedName(commandAndName[1].Trim(), dbtype));
			if (requireBraces) {
				cmdbuilder.Append(" ( ");
			}
			else {
				cmdbuilder.Append(" ");
			}
			if (!string.IsNullOrWhiteSpace(paramlist)) {
				cmdbuilder.Append(RewriteParamList(paramlist, IsSupportNamedParameters(dbtype),
				                                   GetPrefixOfInnerParameter(dbtype),
				                                   GetPrefixOfOuterParameter(dbtype), GetAssignOperator(dbtype)));
			}
			if (requireBraces) {
				cmdbuilder.Append(" )");
			}
			return cmdbuilder.ToString();
		}

		private static string RewriteParamList(string paramlist, bool supportNamedInnerParameters, string namedParameterLeftChar, char namedParameterRightChar, string assignOperator) {
			var result = new StringBuilder();
			var parameters = paramlist.Split(',');
			bool first = true;
			foreach (var parameter in parameters) {
				if (!first) {
					result.Append(", ");
				}
				first = false;
				var pair = parameter.Split('=');

				if (pair.Length == 1) {
					var item = pair[0].Trim();
					if (item.StartsWith("~")) {
						item = namedParameterRightChar + item.Substring(1);
					}
					result.Append(item);
				}
				else {
					var invar = pair[0].Trim();
					var item = pair[1].Trim();
					if (item == "~") {
						item = "~" + invar;
					}
					if (item.StartsWith("~"))
					{
						item = namedParameterRightChar + item.Substring(1);
					}
					if (supportNamedInnerParameters) {
						result.Append(namedParameterLeftChar + invar);
						result.Append(" ");
						result.Append(assignOperator);
						result.Append(" ");
						result.Append(item);
					}
					else {
						result.Append(item);
					}
				}
			}
			return result.ToString();
		}

		private static string GetQuotedName(string name, DatabaseEngineType dbtype) {
			if (name.Contains(".")) {
				var sandn = name.Split('.');
				return sandn[0] + "." + QuoteIdentifier(sandn[1],dbtype);
			}
			else {
				return QuoteIdentifier(name,dbtype);
			}

		}
		/// <summary>
		/// ������ ���������� ��� ��������������
		/// </summary>
		/// <param name="name"></param>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		public static string QuoteIdentifier(string name, DatabaseEngineType dbtype) {
			switch (dbtype) {
					case DatabaseEngineType.SqlServer:
					return "[" + name + "]";
					case DatabaseEngineType.Postgres:
					return "\"" + name + "\"";
					case DatabaseEngineType.MySql:
					return "`" + name + "`";
					case DatabaseEngineType.Oracle:
					return "\"" + name + "\"";
					default:
					return name; //no quoting for unknown db
			}
		}

		private static SqlCommandType DetermineCommandType(string selcommand) {
			if(selcommand=="UNICALL")return SqlCommandType.Call;
			return SqlCommandType.Select;
		}
		/// <summary>
		/// ���������� �������� ���������� ����������
		/// </summary>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		public static string GetAssignOperator(DatabaseEngineType dbtype) {
			switch (dbtype)
			{
				case DatabaseEngineType.SqlServer:
					return "=";
				case DatabaseEngineType.Postgres:
					return ":=";
				case DatabaseEngineType.MySql:
					return "=";
				case DatabaseEngineType.Oracle:
					return "=>";
				default:
					return "=";
			}
	    }

		/// <summary>
		/// ���������� ������� ��� ������������ �������� ���������
		/// </summary>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		public static char GetPrefixOfOuterParameter(DatabaseEngineType dbtype) {
			switch (dbtype)
			{
				case DatabaseEngineType.SqlServer:
					return '@';
				case DatabaseEngineType.Postgres:
					return ':';
				case DatabaseEngineType.MySql:
					return '?';
				case DatabaseEngineType.Oracle:
					return ':';
				default:
					return ':';
			}
	    }
		/// <summary>
		/// ���������� ������� ��� ������������ (�����������) ���������
		/// </summary>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		public static string GetPrefixOfInnerParameter(DatabaseEngineType dbtype) {
			switch (dbtype)
			{
				case DatabaseEngineType.SqlServer:
					return "@";
				case DatabaseEngineType.Postgres:
					return string.Empty;
				case DatabaseEngineType.MySql:
					return string.Empty;
				case DatabaseEngineType.Oracle:
					return string.Empty;
				default:
					return string.Empty;
			}
	    }

		/// <summary>
		/// ���������� ������� ���� ������� �� ���������� ����� ���������/�������
		/// </summary>
		/// <param name="cmdtype"></param>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		public static string GetCallCommandPrefix(SqlCommandType cmdtype, DatabaseEngineType dbtype) {
		    if (dbtype == DatabaseEngineType.SqlServer) return "EXEC";
			if (dbtype == DatabaseEngineType.MySql) {
				if (cmdtype == SqlCommandType.Call) {
					return "CALL";	
				}
				return "CALL ALL";
			}
		    if (cmdtype == SqlCommandType.Call) {
			    return "SELECT";
		    }
		    return "SELECT * FROM";
	    }


		/// <summary>
		/// True - ��������� ������� ������ ����������� ��������
		/// </summary>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		public static bool IsRequireBracesToCall(DatabaseEngineType dbtype) {
			switch (dbtype) {
				case DatabaseEngineType.SqlServer:
					return false;
				case DatabaseEngineType.Postgres:
					return true;
				case DatabaseEngineType.MySql:
					return true;
				case DatabaseEngineType.Oracle:
					return true;
				default:
					return true;
			}
		}
		/// <summary>
		/// True - ���� ������ ������������ ����������� ��������� � ������ �������
		/// </summary>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		public static bool IsSupportNamedParameters(DatabaseEngineType dbtype) {
			switch (dbtype)
			{
				case DatabaseEngineType.SqlServer:
					return true;
				case DatabaseEngineType.Postgres:
					return true;
				case DatabaseEngineType.MySql:
					return false;
				case DatabaseEngineType.Oracle:
					return true;
				default:
					return true;
			}
		}
	    /// <summary>
	    /// ��������� �������� ���������� � ������� �� ����������� ���������, � ������ ���������� ��������� � ������ �������, �� ������������
	    /// </summary>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <param name="query"></param>
	    private static void PrepareParameters(string command, IEnumerable<KeyValuePair<string, object>> parameters, IDbCommand query) {
		    var paramchar = GetPrefixOfOuterParameter(DetermineDbType(query));  
		    foreach (var pair in parameters) {
			    var name = pair.Key;
				if (name.StartsWith("@")) {
					name = name.Substring(1);
				}
			    var paramregex = @"[^\w\d]" + paramchar + name + @"\b";
				if (!Regex.IsMatch(command, paramregex, RegexOptions.IgnoreCase)) {
					continue;
				}
			    var parameter = query.CreateParameter();
			    parameter.ParameterName = pair.Key;
			    if (pair.Value is DbType) {
				    parameter.DbType = (DbType) pair.Value;
			    }
			    else {
				    if (null == pair.Value) {
					    parameter.Value = DBNull.Value;
				    }
				    else {
					    parameter.Value = pair.Value;
					    if (parameter.Value is XElement) {
						    parameter.DbType = DbType.Xml;
						    parameter.Value = parameter.Value.ToString();
					    }
				    }
			    }
			    query.Parameters.Add(parameter);
		    }
	    }

	    

	    ///  <summary>
	    /// ����������� ��������� ������� ��� ������� - ��� ���� - ����, ������ - �������� - ���� ������
	    ///  </summary>
	    ///  <param name="connection"></param>
	    ///  <param name="command"></param>
	    ///  <param name="parameters"></param>
	    /// <param name="timeout"></param>
	    /// <returns></returns>
	    ///  <exception cref="ArgumentNullException"></exception>
	    public static IDictionary<string, object> ExecuteDictionary(this IDbConnection connection, string command,
                                                                    IDictionary<string, object> parameters, int timeout = 30)
        {
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var reader = connection.CreateCommand(command, parameters, timeout).ExecuteReader(CommandBehavior.SingleRow);
            try
            {
                if (reader.Read())
                {
                    var result = new Dictionary<string, object>();
                    for (var i = 0; i < reader.FieldCount; i++)
                        result[reader.GetName(i)] = reader[i];
                    return result;
                }
            }
            finally
            {
                reader.Close();
                connection.Close();
            }
            return null;
        }


	    /// <summary>
	    ///  ��������� ������ ORM - ��������� ������������ ������ �������� ��������
	    /// </summary>
	    /// <param name="connection"></param>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <param name="timeout"></param>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    /// <exception cref="ArgumentNullException"></exception>
	    public static T[] 
            ExecuteOrm<T>(this IDbConnection connection, object command,object parameters=null,int timeout = 30) where T:new(){
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var result = new List<T>();
	        var cmd = connection.CreateCommand(command, parameters, timeout);
	        cmd.CommandTimeout = 0;
            var reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read()||reader.NextResult()) {
                    var item = new T();
                    for(int i=0;i<reader.FieldCount;i++) {
                        var name = reader.GetName(i);
                        item.SetValue(name, reader[i] is DBNull ? null : reader[i],ignoreNotFound:true);
                    }
                    result.Add(item);
                }
            }
            finally
            {
                reader.Close();
                connection.Close();
            }
            return result.ToArray();
        }

	    /// <summary>
	    /// ���������� ��� ������ ������� ��� ������� - ������ ����- ����, ������ - ��������
	    /// </summary>
	    /// <param name="connection"></param>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <param name="timeout"></param>
	    /// <returns></returns>
	    public static IDictionary<string, object> ExecuteDictionaryReader(this IDbConnection connection, object command,
                                                                   object parameters=null,int timeout=30)
        {
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var reader = connection.CreateCommand(command, parameters, timeout).ExecuteReader(CommandBehavior.SingleResult);
            try
            {
                var result = new Dictionary<string, object>();
                while (reader.Read())
                {
                    if (reader.FieldCount == 2) {
                        result[reader[0] as string] = reader[1];
                    }else {
                        var subresult = new List<object>();
                        for(int i = 1; i< reader.FieldCount; i++) {
                            subresult.Add(reader[i]);
                        }
                        result[reader[0] as string] = subresult.ToArray();
                    }
                }
                return result;
            }
            finally
            {
                reader.Close();
                connection.Close();
            }
        }

	    /// <summary>
	    /// ��������� �� ������� ��������������� ������� - ��������� ����� �� ��������� ���� ����� � ��������� ���������
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="connection"></param>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <param name="timeout"></param>
	    /// <returns></returns>
	    public static IDictionary<string, IList<T>> ExecuteDictionaryReaderList<T>(this IDbConnection connection, string command,
																  object parameters,int timeout=30) where T:struct 
		{
			if (null == connection) throw new ArgumentNullException("connection");
			connection.WellOpen();
			var reader = connection.CreateCommand(command, parameters, timeout).ExecuteReader(CommandBehavior.SingleResult);
			try
			{
				var result = new Dictionary<string,  IList<T>>();
				while (reader.Read()) {
					var key = reader[0] as string;
					var value = reader[1].To<T>();
					if(!result.ContainsKey(key)) {
						result[key] = new List<T>();
					}
					if(!result[key].Contains(value)) {
						result[key].Add(value);
					}
					
				}
				return result;
			}
			finally
			{
				reader.Close();
				connection.Close();
			}

		}

	    /// <summary>
	    /// ��������� ������ ��� �������������� ������ (������������ ������ ������ ����)
	    /// </summary>
	    /// <param name="connection"></param>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <param name="timeout"></param>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    /// <exception cref="ArgumentNullException"></exception>
	    public static IList<T> ExecuteList<T>(this IDbConnection connection, string command,
                                                                   object parameters,int timeout = 30) {
            var result = new List<T>();
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var reader = connection.CreateCommand(command, parameters, timeout).ExecuteReader(CommandBehavior.SingleResult);
            try
            {
                while (reader.Read()) {
                    result.Add(reader[0].To<T>());
                }
                return result;
            }
            finally
            {
                reader.Close();
                connection.Close();
            }
        }
    }
}