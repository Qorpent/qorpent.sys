using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// Описатель унифицированного SQL запроса
	/// </summary>
	public class UniSqlQuery {

		/// <summary>
		/// default ctor
		/// </summary>
		public UniSqlQuery() {
			
		}
		/// <summary>
		/// Creates command for given stored proc or function
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="name"></param>
		/// <param name="calltype"></param>
		/// <param name="parameters"></param>
		public UniSqlQuery(string schema, string name, SqlCommandType calltype = SqlCommandType.Call, object parameters = null) {
			Schema = schema;
			Name = name;
			CommandType = calltype;
			ParameterDefinition = parameters;
		}

		/// <summary>
		/// Кэш подготовленных текстов запросов
		/// </summary>
		string[] _textcache = new string[10];
		/// <summary>
		/// Тип вызова команды 
		/// </summary>
		private SqlCommandType _commandType = SqlCommandType.Call;
		/// <summary>
		/// Схема объекта
		/// </summary>
		private string _schema = "";
		/// <summary>
		/// Имя объекта
		/// </summary>
		private string _name = "";
		/// <summary>
		/// Определение параметров
		/// </summary>
		private object _parameterDefinition = null;

		private IDictionary<string, object> _paramdict;
		private bool _invalid;
		private string[] _extparams = null; 
		/// <summary>
		/// Определение параметров
		/// </summary>
		public object ParameterDefinition {
			get { return _parameterDefinition; }
			set {
				if (_parameterDefinition != value) {
					_parameterDefinition = value;
					_paramdict = _parameterDefinition.ToDict();
					var extparams = new List<string>();
					foreach (var o in _paramdict.ToArray()) {
						if (Equals(o.Value, "~")) {
							_paramdict[o.Key] = "~" + o.Key;
						}
						var strparam = _paramdict[o.Key] as string;
						if (null!=strparam) {
							if (strparam.StartsWith("~")) {
								var extname = strparam.Substring(1).ToUpper();
								if (!extparams.Contains(extname)) {
									extparams.Add(extname);
								}
							}
						}
					}
					_extparams = extparams.ToArray();
					_invalid = true;
				}

			}
		}

		/// <summary>
		/// Имя объекта
		/// </summary>
		public string Name {
			get { return _name; }
			set {
				if (_name != value) {
					_name = value;
					_invalid = true;
				}
			}
		}

		/// <summary>
		/// Схема объекта
		/// </summary>
		public string Schema {
			get { return _schema; }
			set 
			{
				if (_schema != value) {
					_schema = value;
					_invalid = true;
				}
			}
		}

		/// <summary>
		/// Тип вызова команды 
		/// </summary>
		public SqlCommandType CommandType {
			get { return _commandType; }
			set {
				if (_commandType != value) {
					_commandType = value;
					_invalid = true;
				}
			}
		}

		/// <summary>
		/// Подготовка текста DbCommand
		/// </summary>
		/// <param name="dbtype"></param>
		/// <returns></returns>
		public string PrepareQueryText(DatabaseEngineType dbtype) {
			var idx = (int) dbtype;
			if (_invalid) {
				_textcache = new string[5];	
			}
			if( string.IsNullOrWhiteSpace(_textcache[idx])) {
				_textcache[idx] = InternalPrepareQueryText(dbtype);
			}
			return _textcache[idx];
		}

		private string InternalPrepareQueryText(DatabaseEngineType dbtype) {
			var result = new StringBuilder();
			result.Append(DbExtensions.GetCallCommandPrefix(CommandType, dbtype));
			result.Append(" ");
			AppendQuotedName(dbtype, result);
			result.Append(" ");
			
			var writeBraces = DbExtensions.IsRequireBracesToCall(dbtype);
			
			if (writeBraces) {
				result.Append("( ");
			}
			if (null != _paramdict) {
				WriteParameters(dbtype, result);
			}
			if (writeBraces) {
				result.Append(" )");
			}
			return result.ToString();
		}

		private void WriteParameters(DatabaseEngineType dbtype, StringBuilder result) {
			var writeAssigns = DbExtensions.IsSupportNamedParameters(dbtype);
			var innerPrefix = DbExtensions.GetPrefixOfInnerParameter(dbtype);
			var outerPrefix = DbExtensions.GetPrefixOfOuterParameter(dbtype);
			var assignSymbol = DbExtensions.GetAssignOperator(dbtype);
			bool first = true;
			foreach (var pair in _paramdict) {
				if (!first) {
					result.Append(", ");
				}
				first = false;
				if (writeAssigns) {
					result.Append(innerPrefix);
					result.Append(pair.Key);
					result.Append(" ");
					result.Append(assignSymbol);
					result.Append(" ");
				}
				var item = pair.Value;
				if (null==item || item is DBNull) {
					result.Append("null");
				}
				if (item is string) {
					var stritem = item as string;
					if (stritem.StartsWith("~")) {
						result.Append(outerPrefix);
						result.Append(stritem.Substring(1));
					}
					else {
						result.Append("'" + stritem.Replace("'", "''") + "'");
					}
				}
				else {
					var anyitem = item.ToString();
					if (!Regex.IsMatch(anyitem, @"^[\w\d\.]+$",RegexOptions.Compiled)) {
						anyitem = "'" + anyitem.Replace("'", "''") + "'";
					}
					result.Append(anyitem);
				}
			}
		}

		private void AppendQuotedName(DatabaseEngineType dbtype, StringBuilder result) {
			if (!string.IsNullOrWhiteSpace(Schema)) {
				if (dbtype == DatabaseEngineType.MySql) {
					result.Append(DbExtensions.QuoteIdentifier(Schema + "_" + Name, dbtype));
				}
				else {
					result.Append(DbExtensions.QuoteIdentifier(Schema, dbtype));
					result.Append(".");
					result.Append(DbExtensions.QuoteIdentifier(Name, dbtype));
				}
			}
			else {
				result.Append(DbExtensions.QuoteIdentifier(Name, dbtype));
			}
			
		}

		/// <summary>
		/// Формирует полностью комманду SQL
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IDbCommand PrepareCommand(IDbConnection connection, object parameters = null) {
			var dbtype = connection.DetermineDbType();
			var result = connection.CreateCommand();
			result.CommandText = PrepareQueryText(dbtype);
			if (null != parameters) {
				var extdict = parameters.ToDict();
				foreach (var p in extdict) {
					if (-1 != Array.IndexOf(_extparams, p.Key.ToUpper())) {
						var parameter = result.CreateParameter();
						parameter.ParameterName = p.Key;
						if (null == p.Value) {
							parameter.Value = DBNull.Value;
						}
						else {
							parameter.Value = p.Value;
						}
					}
				}
			}
			return result;
		}
	}
}