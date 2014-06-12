using System.Linq;
using System.Text;
using Qorpent.Serialization;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// 
	/// </summary>
	public class SqlFunctionWriter : SqlCommandWriter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="function"></param>
		public SqlFunctionWriter(SqlFunction function)
		{
			this.Function = function;
			this.Parameters = function;
		}

		/// <summary>
		/// 
		/// </summary>
		public SqlFunction Function { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetText()
		{
			if (Dialect != SqlDialect.SqlServer)
			{
				return "-- ! ВНИМАНИЕ НА ДАННЫЙ МОМЕНТ РЕАЛИЗАЦИЯ ГЕНЕРАЦИИ ФУНКЦИЙ ЕСТЬ ТОЛЬКО ДЛЯ MS SQL";
			}
			var sb = new StringBuilder();
			var itemname = "FUNCTION";
			if (Function.IsProcedure){
				itemname = "PROCEDURE";
			}
			sb.AppendLine("IF OBJECT_ID('${FullName}') IS NOT NULL DROP "+itemname+" ${FullName};");
			sb.AppendLine("GO");

			var body = Function.ResolveBody();
			if (Mode == ScriptMode.Create)
			{
				if (Function.IsFullyExternal())
				{
					sb.Append(body);
				}
				else
				{
					var arguments = string.Join(",",Function.Arguments.Values.OrderBy(_=>_.Index).Select(GetArgumentString));
					if (Function.IsProcedure){
						sb.Append("CREATE PROCEDURE ${FullName} " + arguments);
					}
					else{
						sb.Append("CREATE FUNCTION ${FullName} ( " + arguments + " ) RETURNS " +
						          Function.ReturnType.ResolveSqlDataType(SqlDialect.SqlServer));
					}
					sb.Append(" AS BEGIN");
					sb.AppendLine();
					sb.AppendLine(body);
					sb.Append("END;");
				}
			}
			return sb.ToString();
		}

		private object GetArgumentString(SqlFunctionArgument arg){
			var sb = new StringBuilder();
			sb.Append("@" + arg.Name);
			sb.Append(" ");
			sb.Append(arg.DataType.ResolveSqlDataType(SqlDialect.SqlServer));
			if (null != arg.DefaultValue){
				sb.Append(" = ");
				var str = arg.DefaultValue.Value.ToString();
				if (str.StartsWith("(")){
					sb.Append(str);
				}
				else{
					sb.Append("'" + str.ToSqlString() + "'");
				}
			}
			if (arg.IsOutput){
				sb.Append(" OUTPUT");
			}
			return sb.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher()
		{
			return (Function.IsProcedure? "PROCEDURE":"FUNCTION")+" " + Function.FullName;
		}
	}
}