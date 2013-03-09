// Copyright 2007-2010 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
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
// MODIFICATIONS HAVE BEEN MADE TO THIS FILE

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Data;

namespace Qorpent.Utils.Extensions
{
    /// <summary>
    /// Расширения для работы с командами и соединениями БД
    /// </summary>
    public static class DbExtensions
    {
		

        /// <summary>
        /// Выполняет запрос без результатов
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command"></param>
        /// <param name="provider"></param>
        public static void ExecuteNonQuery(this IDbConnection connection, string command,IParametersProvider provider) {
            var dict = null == provider ? null : new ParamMappingHelper().GetParameters(provider);
            ExecuteNonQuery(connection, command, dict);
        }
		/// <summary>
		/// Вызывает запрос на скалярное значение
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="command"></param>
		/// <param name="provider"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T ExecuteScalar<T>(this IDbConnection connection, string command, IParametersProvider provider)
		{
			var dict = null == provider ? null : new ParamMappingHelper().GetParameters(provider);
			return ExecuteScalar<T>(connection, command, dict);
		}

        /// <summary>
		/// Выполняет запрос без результатов
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public static void ExecuteNonQuery(this IDbConnection connection, string command,
                                           IDictionary<string, object> parameters)
        {
           ExecuteNonQuery(connection,command,parameters,30);
        }

		/// <summary>
		/// Выполняет запрос без результатов
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <param name="timeout"></param>
		/// <exception cref="Exception"></exception>
		public static void ExecuteNonQuery(this IDbConnection connection, string command,
										   IDictionary<string, object> parameters, int timeout)
		{
			connection.WellOpen();
			IDbCommand cmd = connection.CreateCommand(command, parameters);
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
		/// Вызывает запрос на скалярное значение
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ExecuteScalar<T>(this IDbConnection connection, string command,
                                         IDictionary<string, object> parameters)
        {
            return connection.ExecuteScalar(command, parameters, default(T));
        }

        /// <summary>
		/// Вызывает запрос на скалярное значение
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <param name="defValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T ExecuteScalar<T>(this IDbConnection connection, string command,
                                         IDictionary<string, object> parameters, T defValue)
        {
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var result = connection.CreateCommand(command, parameters).ExecuteScalar();
            if (null == result) return defValue;
            if (result is DBNull) return defValue;
            return result.To<T>();
        }
		/// <summary>
		/// Выполняет запрос на "строку" - сериализованный массив объектов
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
        public static object[] ExecuteRow(this IDbConnection connection, string command,
                                          IDictionary<string, object> parameters)
        {
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var reader = connection.CreateCommand(command, parameters).ExecuteReader(CommandBehavior.SingleRow);
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
        /// <param name="parameters_"></param>
        /// <returns></returns>
        public static IDbCommand CreateCommand(this IDbConnection connection, string command,
                                               object parameters_){
            var query = connection.CreateCommand();
	        query.CommandText = command;
			if(null!=parameters_) {
				if (parameters_ is IDictionary<string, object>) {
					PrepareParametersFromDictionary(command, parameters_, query);
				}
				else {
					PrepareParametersFromProperties(command, parameters_, query);
				}
			}
	        return query;
        }

	    private static void PrepareParametersFromProperties(string command, object parameters_, IDbCommand query) {
		    var parameters = parameters_.GetType().GetProperties();
		    if (parameters.Length != 0 && !command.Contains("@")) {
			    command = command + " " +
			              parameters.Select(x => x.Name).Select(x => x.StartsWith("@") ? x : "@" + x).Select(x => x + "=" + x).
				              ConcatString(",");
		    }
		    query.CommandText = command;
		    foreach (var pair in parameters) {
			    var parameter = query.CreateParameter();
			    parameter.ParameterName = pair.Name;
			    var val = pair.GetValue(parameters_);
			    if (val is DbType) {
				    parameter.DbType = (DbType) val;
			    }
			    else {
				    if (null == val) {
					    parameter.Value = DBNull.Value;
				    }
				    else {
					    parameter.Value = val;
					    if (parameter.Value is XElement) {
						    parameter.DbType = DbType.Xml;
						    parameter.Value = ((XElement) parameter.Value).ToString();
					    }
				    }
			    }


			    query.Parameters.Add(parameter);
		    }
	    }

	    private static void PrepareParametersFromDictionary(string command, object parameters_, IDbCommand query) {
		    var parameters = parameters_ as IDictionary<string, object>;
		    if (parameters.Count != 0 && !command.Contains("@")) {
			    command = command + " " +
			              parameters.Keys.Select(x => x.StartsWith("@") ? x : "@" + x).Select(x => x + "=" + x).ConcatString(",");
		    }
		    query.CommandText = command;
		    foreach (var pair in parameters) {
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
						    parameter.Value = ((XElement) parameter.Value).ToString();
					    }
				    }
			    }


			    query.Parameters.Add(parameter);
		    }
	    }

	    /// <summary>
	    ///Сериализует результат команды как словарь - имя поле - ключ, второе - значение - одна строка
	    /// </summary>
	    /// <param name="connection"></param>
	    /// <param name="command"></param>
	    /// <param name="parameters"></param>
	    /// <returns></returns>
	    /// <exception cref="ArgumentNullException"></exception>
	    public static IDictionary<string, object> ExecuteDictionary(this IDbConnection connection, string command,
                                                                    IDictionary<string, object> parameters)
        {
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var reader = connection.CreateCommand(command, parameters).ExecuteReader(CommandBehavior.SingleRow);
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
        /// Эмулирует работу ORM - прошивает именованными полями свойства объектов
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command"></param>
        /// <param name="provider"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] ExecuteOrm<T>(this IDbConnection connection, string command, IParametersProvider provider) where T:new() {
        	connection.WellOpen();
            var dict = null == provider ? null : new ParamMappingHelper().GetParameters(provider);
            return ExecuteOrm<T>(connection, command, dict);
        }

        /// <summary>
		///  Эмулирует работу ORM - прошивает именованными полями свойства объектов
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T[] 
            ExecuteOrm<T>(this IDbConnection connection, string command,IDictionary<string, object> parameters) where T:new(){
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var result = new List<T>();
            var reader = connection.CreateCommand(command, parameters).ExecuteReader();
            try
            {
                while (reader.Read()||reader.NextResult()) {
                    var item = new T();
                    for(int i=0;i<reader.FieldCount;i++) {
                        var name = reader.GetName(i);
                        item.SetValue(name, reader[i] is DBNull ? null : reader[i]);
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
		/// Возвращает все строки запроса как словарь - первое поле- ключ, второе - значение
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
        public static IDictionary<string, object> ExecuteDictionaryReader(this IDbConnection connection, string command,
                                                                    IDictionary<string, object> parameters)
        {
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var reader = connection.CreateCommand(command, parameters).ExecuteReader(CommandBehavior.SingleResult);
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
		/// Формирует из запроса сгруппированный словарь - повторные ключи не вытесняют друг дргуа а дополняют коллекцию
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static IDictionary<string, IList<T>> ExecuteDictionaryReaderList<T>(this IDbConnection connection, string command,
																   IDictionary<string, object> parameters) where T:struct 
		{
			if (null == connection) throw new ArgumentNullException("connection");
			connection.WellOpen();
			var reader = connection.CreateCommand(command, parameters).ExecuteReader(CommandBehavior.SingleResult);
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
        /// Выполняет запрос как типизированный список (используется только первое поле)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IList<T> ExecuteList<T>(this IDbConnection connection, string command,
                                                                   IDictionary<string, object> parameters) {
            var result = new List<T>();
            if (null == connection) throw new ArgumentNullException("connection");
            connection.WellOpen();
            var reader = connection.CreateCommand(command, parameters).ExecuteReader(CommandBehavior.SingleResult);
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
