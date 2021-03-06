﻿using System.Linq;
using System.Text;
using Qorpent.Data;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// </summary>
	public class SqlFunctionWriter : SqlCommandWriter{
		/// <summary>
		/// </summary>
		/// <param name="function"></param>
		public SqlFunctionWriter(SqlFunction function){
			Function = function;
			Parameters = function;
		}

		/// <summary>
		/// </summary>
		public SqlFunction Function { get; set; }

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			if (Dialect != DbDialect.SqlServer){
				return "-- ! ВНИМАНИЕ НА ДАННЫЙ МОМЕНТ РЕАЛИЗАЦИЯ ГЕНЕРАЦИИ ФУНКЦИЙ ЕСТЬ ТОЛЬКО ДЛЯ MS SQL";
			}
			var sb = new StringBuilder();
			string itemname = "FUNCTION";
			if (Function.IsProcedure){
				itemname = "PROCEDURE";
			}
			sb.AppendLine("IF OBJECT_ID('${FullName}') IS NOT NULL DROP " + itemname + " ${FullName};");
			sb.AppendLine("GO");

			string body = Function.ResolveBody();
			if (Mode == ScriptMode.Create){
				if (Function.IsFullyExternal()){
					sb.Append(body);
				}
				else{
					string arguments = string.Join(",", Function.Arguments.Values.OrderBy(_ => _.Index).Select(GetArgumentString));
					if (Function.IsProcedure){
						sb.Append("CREATE PROCEDURE ${FullName} " + arguments);
					}
					else{
						if (Function.ReturnType.IsTable){
							sb.Append("CREATE FUNCTION ${FullName} ( " + arguments + " )\r\nRETURNS @result TABLE " +
							          GetTableType(Function.ReturnType,DbDialect.SqlServer));
						}else if (Function.ReturnType.IsNative){
							var type = Function.ReturnType.SqlText; 
							if (type.Contains(",")){
								var fields = type.SmartSplit(false, true, ',');
								type = "@result TABLE (";
								var fst = true;
								foreach(var f in fields){
									if (!fst){
										type += ", ";
									}
									fst = false;
									var n = f;
									if (!n.Contains(" ")){
										n += " nvarchar(255)";
									}
									type += n;
								}
								type += ")";
							}
							sb.Append("CREATE FUNCTION ${FullName} ( " + arguments + " ) RETURNS " + type);
						}
						else{
							sb.Append("CREATE FUNCTION ${FullName} ( " + arguments + " ) RETURNS " +
							          Function.ReturnType.ResolveSqlDataType(DbDialect.SqlServer));
						}
					}
					sb.Append(" AS BEGIN");
					sb.AppendLine();
					sb.AppendLine(body);
					if (null!=Function.ReturnType && Function.ReturnType.IsTable){
						sb.AppendLine("RETURN;");
					}
					sb.Append("END;");
				}
			}
			return sb.ToString();
		}

		private string GetTableType(DataType returnType,DbDialect dialect){
			var result = new StringBuilder();
			result.Append("(");
			result.Append(string.Join(", ",
			              returnType.TargetType.GetOrderedFields()
			                        .Where(_ => !_.NoSql)
			                        .Select(_ => _.Name.SqlQuoteName()+ " " + _.DataType.ResolveSqlDataType(dialect))));
			result.Append(")");
			return result.ToString();

		}

		private object GetArgumentString(SqlFunctionArgument arg){
			var sb = new StringBuilder();
			sb.Append("@" + arg.Name);
			sb.Append(" ");
			sb.Append(arg.DataType.ResolveSqlDataType(DbDialect.SqlServer));
			if (null != arg.DefaultValue){
				sb.Append(" = ");
				string str = arg.DefaultValue.Value.ToString();
				if (str.StartsWith("(")){
					sb.Append(str);
				}
				else{
					sb.Append("'" + str.ToSqlString() + "'");
				}
			}
			else{
				sb.Append(" = null ");
			}
			if (arg.IsOutput){
				sb.Append(" OUTPUT");
			}
			return sb.ToString();
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			return (Function.IsProcedure ? "PROCEDURE" : "FUNCTION") + " " + Function.FullName;
		}
	}
}