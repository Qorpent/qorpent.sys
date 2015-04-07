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
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Data;

namespace Qorpent.Utils.Extensions
{
	/// <summary>
    /// Расширения для работы с командами и соединениями БД
    /// </summary>
    public static class DatabaseExtensions
    {
		/// <summary>
		/// Определение типа целевой БД по классу DbCommand
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
		/// Определение типа целевой БД по классу DbConnection
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
	    /// Выполняет запрос без результатов
	    /// </summary>
	    /// <param name="connection"></param>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <param name="timeout"></param>
	    /// <param name="close"></param>
	    /// <param name="showCommandTextWithParams">показать в Debug</param>
	    /// <exception cref="Exception"></exception>
	    public static int ExecuteNonQuery(this IDbConnection connection, object command,
										   object parameters = null, int timeout=30, bool close = false, bool showCommandTextWithParams = false)
		{
			connection.WellOpen();
			IDbCommand cmd = connection.CreateCommand(command, parameters,timeout);
			cmd.CommandTimeout = timeout;
			int r;
			try{
                if (showCommandTextWithParams) {
                    Trace.WriteLine("Sql command: [\r\n" + cmd.CommandAsSql() + "\r\n]", "debug");
			    }
				r = cmd.ExecuteNonQuery();

			}
			catch (Exception ex){
				throw new Exception("error in query:" + cmd.CommandText, ex);
			}
			finally{
				if (close){
					connection.Close();
				}
			}
			return r;
		}


        public static String ParameterValueForSQL(this SqlParameter sp) {
            String retval = "";

            switch (sp.SqlDbType) {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.Time:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                    break;

                case SqlDbType.Bit:
                    retval = (sp.Value.ToBooleanOrDefault(false)) ? "1" : "0";
                    break;

                default:
                    retval = sp.Value.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        }


        public static Boolean ToBooleanOrDefault(this String s, Boolean Default) {
            return ToBooleanOrDefault((Object)s, Default);
        }


        public static Boolean ToBooleanOrDefault(this Object o, Boolean Default) {
            Boolean ReturnVal = Default;
            try {
                if (o != null) {
                    switch (o.ToString().ToLower()) {
                        case "yes":
                        case "true":
                        case "ok":
                        case "y":
                            ReturnVal = true;
                            break;
                        case "no":
                        case "false":
                        case "n":
                            ReturnVal = false;
                            break;
                        default:
                            ReturnVal = Boolean.Parse(o.ToString());
                            break;
                    }
                }
            } catch {
            }
            return ReturnVal;
        }

        public static String CommandAsSql(this IDbCommand sc) {
            StringBuilder sql = new StringBuilder();
            Boolean FirstParam = true;

            sql.AppendLine("use " + sc.Connection.Database + ";");
            switch (sc.CommandType) {
                case CommandType.StoredProcedure:
                    sql.AppendLine("declare @return_value int;");

                    foreach (SqlParameter sp in sc.Parameters) {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output)) {
                            sql.Append("declare " + sp.ParameterName + "\t" + sp.SqlDbType.ToString() + "\t= ");

                            sql.AppendLine(((sp.Direction == ParameterDirection.Output) ? "null" : sp.ParameterValueForSQL()) + ";");

                        }
                    }

                    sql.AppendLine("exec [" + sc.CommandText + "]");

                    foreach (SqlParameter sp in sc.Parameters) {
                        if (sp.Direction != ParameterDirection.ReturnValue) {
                            sql.Append((FirstParam) ? "\t" : "\t, ");

                            if (FirstParam) FirstParam = false;

                            if (sp.Direction == ParameterDirection.Input)
                                sql.AppendLine(sp.ParameterName + " = " + sp.ParameterValueForSQL());
                            else

                                sql.AppendLine(sp.ParameterName + " = " + sp.ParameterName + " output");
                        }
                    }
                    sql.AppendLine(";");

                    sql.AppendLine("select 'Return Value' = convert(varchar, @return_value);");

                    foreach (SqlParameter sp in sc.Parameters) {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output)) {
                            sql.AppendLine("select '" + sp.ParameterName + "' = convert(varchar, " + sp.ParameterName + ");");
                        }
                    }
                    break;
                case CommandType.Text:
                    sql.AppendLine(sc.CommandText);
                    sql.AppendLine("Parameters:");
                    // show command parameters
                    foreach (SqlParameter sp in sc.Parameters) {
                        if (sp.Direction != ParameterDirection.ReturnValue) {
                            sql.Append((FirstParam) ? "\t" : "\t, ");

                            if (FirstParam) FirstParam = false;

                            if (sp.Direction == ParameterDirection.Input)
                                sql.AppendLine(sp.ParameterName + " = " + sp.ParameterValueForSQL());
                            else

                                sql.AppendLine(sp.ParameterName + " = " + sp.ParameterName + " output");
                        }
                    }
                    sql.AppendLine(";");

                    sql.AppendLine("select 'Return Value' = convert(varchar, @return_value);");

                    foreach (SqlParameter sp in sc.Parameters) {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output)) {
                            sql.AppendLine("select '" + sp.ParameterName + "' = convert(varchar, " + sp.ParameterName + ");");
                        }
                    }

                    break;
            }

            return sql.ToString();
        }
		


		/// <summary>
		/// Вызывает запрос на скалярное значение
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <param name="defValue"></param>
		/// <param name="timeout"></param>
		/// <param name="close"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T ExecuteScalar<T>(this IDbConnection connection, object command,
                                         object parameters=null, T defValue=default(T) , int timeout = 30, bool close = false)
        {
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var result = connection.CreateCommand(command, parameters, timeout).ExecuteScalar();
            if (null == result) return defValue;
            if (result is DBNull) return defValue;
			if (close){
				connection.Close();
			}
            return result.To<T>();
        }

	    /// <summary>
	    /// Выполняет запрос на "строку" - сериализованный массив объектов
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
        /// Корректный метод гарантированного открытия соединения
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
            // TODO: если будет Exception то не ясно откуда он взялся 
            throw new InvalidOperationException("Insuficient connection state " + connection.State);
        }


	    /// <summary>
	    /// Создает команлу с параметрами
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
			Debug.Assert(!string .IsNullOrWhiteSpace(realcommand),"1");
			    query.CommandText = realcommand;
				Debug.Assert(!string.IsNullOrWhiteSpace(query.CommandText), "2");
			    if (null != parameters) {
			        if (parameters is ISqlParametersSource) {
			            ((ISqlParametersSource) parameters).SetupSqlParameters(query);
			        }
			        else {
			            PrepareParameters(realcommand, parameters.ToDict(), query);
			        }

			    }
				Debug.Assert(!string.IsNullOrWhiteSpace(query.CommandText), "3");
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
			if (!String.IsNullOrWhiteSpace(paramlist)) {
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
		/// Делает защищенное имя идентификатора
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
		/// Возвращает оператор присвоения параметров
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
		/// Возвращает префикс для именованного внешнего параметра
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
		/// Возвращает префикс для именованного (внутреннего) параметра
		/// </summary>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		public static string GetPrefixOfInnerParameter(DatabaseEngineType dbtype) {
			switch (dbtype)
			{
				case DatabaseEngineType.SqlServer:
					return "@";
				case DatabaseEngineType.Postgres:
					return String.Empty;
				case DatabaseEngineType.MySql:
					return String.Empty;
				case DatabaseEngineType.Oracle:
					return String.Empty;
				default:
					return String.Empty;
			}
	    }

		/// <summary>
		/// Возвращает префикс всей команды до собственно имени процедуры/функции
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
		/// True - параметры запроса должны закрываться скобками
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
		/// True - если движок поддерживает именованные параметры в вызове функций
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
	    /// Формирует перечень параметров в команду по переданному источнику, в случае отсутствия параметра в строке запроса, он игнорируется
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
	    /// Сериализует результат команды как словарь - имя поле - ключ, второе - значение - одна строка
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
		///  Эмулирует работу ORM - прошивает именованными полями свойства объектов
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <param name="timeout"></param>
		/// <param name="close"></param>
		/// <param name="map"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T[] 
            ExecuteOrm<T>(this IDbConnection connection, object command,object parameters=null,int timeout = 30, bool close = true, object map = null) where T:new(){
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var result = new List<T>();
           
	        var cmd = connection.CreateCommand(command, parameters, timeout);
	        cmd.CommandTimeout = 0;
            var reader = cmd.ExecuteReader();
			IDictionary<string, string> dict = null;
			if (null != map){
				dict = map.ToDict().ToDictionary(_ => _.Key, _ => _.Value.ToStr());
			}
            try
            {
                while (reader.Read()||reader.NextResult()) {
                    var item = new T();
                    for(int i=0;i<reader.FieldCount;i++) {
                        var name = reader.GetName(i);
						if (null != dict && dict.ContainsKey(name.ToLower())){
							name = dict[name.ToLower()];
						}
                        item.SetValue(name, reader[i] is DBNull ? null : reader[i],ignoreNotFound:true);
                    }
                    result.Add(item);
                }
            }
            finally
            {
                reader.Close();
	            if (close){
		            connection.Close();
	            }
            }
            return result.ToArray();
        }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="connection"></param>
	    /// <param name="script"></param>
	    /// <param name="haltonerror">Прерывать выполнение при обнаружении ошибок</param>
	    /// <returns></returns>
	    public static string[] ExecuteScript(this IDbConnection connection, string script, bool haltonerror = false) {
            if (string.IsNullOrWhiteSpace(script)) return new string[]{};
            var messages = new List<string>();
            SqlInfoMessageEventHandler onMessage = (s, a) => {
                messages.Add(a.Message);
                messages.AddRange(from object e in a.Errors select "ERROR: " + e.ToString());
            };
            var commands =
                Regex.Split(script, @"\sGO(\s|$)")
                    .Cast<Match>()
                    .Select(_ => _.Value.Trim())
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                        .ToArray();
            if(commands.Length==0)return new string[]{};
            connection.WellOpen();
            try {
                if (connection is SqlConnection) {
                    (connection as SqlConnection).InfoMessage += onMessage;
                }
                foreach (var command in commands) {
                    try {
                        connection.ExecuteNonQuery(command);
                    }
                    catch {
                        if (haltonerror) {
                            break;
                        }
                    }
                }
            }
            finally {
                if (connection is SqlConnection)
                {
                    (connection as SqlConnection).InfoMessage -= onMessage;
                }
            }
	        return messages.ToArray();
	    } 

	    /// <summary>
	    /// Возвращает все строки запроса как словарь - первое поле- ключ, второе - значение
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
                        result[reader[0].ToStr()] = reader[1];
                    }else {
                        var subresult = new List<object>();
                        for(int i = 1; i< reader.FieldCount; i++) {
                            subresult.Add(reader[i]);
                        }
                        result[reader[0].ToStr()] = subresult.ToArray();
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
	    /// Формирует из запроса сгруппированный словарь - повторные ключи не вытесняют друг дргуа а дополняют коллекцию
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
				var result = new Dictionary<string, IList<T>>();
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
	    /// Выполняет запрос как типизированный список (используется только первое поле)
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

		/// <summary>
		/// Утилитная функция для формирования объекта соединения из строки
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static IDbConnection CreateDatabaseConnectionFromString(string name){
			var connectionString = name;
			if (connectionString.StartsWith("ProviderName")){
				var parsematch = Regex.Match(connectionString, @"^ProviderName=([^;]+);([\s\S]+)$");
				var providername = parsematch.Groups[1].Value;
				var connstring = parsematch.Groups[2].Value;
				if (providername.ToUpper() == "NPGSQL"){
					if (File.Exists(Path.Combine(EnvironmentInfo.BinDirectory, "Npgsql.dll"))){
						return GetPostGresConnection(connstring);
					}
					else{
						throw new QorpentException("cannot connect to PostGres because Npgsql not exists in application");
					}
				}
				var provider = DbProviderFactories.GetFactory(providername);
				var result = provider.CreateConnection();
				result.ConnectionString = connstring;
				return result;
			}
			else{
				return new SqlConnection(connectionString);
			}
		}

		private static Assembly _npgsqlassembly = null;
		private static Type _npgsqlconnectiontype;

		private static Assembly NpgSQLAssembly {
			get {
				if(null==_npgsqlassembly) {
					_npgsqlassembly =
						AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.ToLower().StartsWith("npgsql"));
					if(null==_npgsqlassembly) {
						_npgsqlassembly = Assembly.LoadFrom("Npgsql.dll");
					}
				}
				return _npgsqlassembly;
			}
		}

		private static Type NpgSQLConnectionType {
			get {
				if(_npgsqlconnectiontype==null) {
					_npgsqlconnectiontype = NpgSQLAssembly.GetType("Npgsql.NpgsqlConnection");
				}
				return _npgsqlconnectiontype;
			}
		}

		private static IDbConnection GetPostGresConnection(string connstring) {
			return (IDbConnection)Activator.CreateInstance(NpgSQLConnectionType, connstring);
		}
	}
}
