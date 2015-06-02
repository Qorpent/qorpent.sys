using System.Linq;
using System.Text;
using Qorpent.Data;
using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// </summary>
	public class SqlTriggerWriter : SqlCommandWriter{
		/// <summary>
		/// </summary>
		/// <param name="sqlTrigger"></param>
		public SqlTriggerWriter(SqlTrigger sqlTrigger){
			Trigger = sqlTrigger;
			Parameters = sqlTrigger;
		}

		/// <summary>
		/// </summary>
		public SqlTrigger Trigger { get; set; }

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			if (Dialect != DbDialect.SqlServer){
				return "-- !ВНИМАНИЕ НА ДАННЫЙ МОМЕНТ РЕАЛИЗАЦИЯ ГЕНЕРАЦИИ ТРИГЕРОВ ЕСТЬ ТОЛЬКО ДЛЯ MS SQL";
			}
			var sb = new StringBuilder();
			sb.AppendLine("IF OBJECT_ID('${FullName}') IS NOT NULL DROP TRIGGER ${FullName};");
			sb.AppendLine("GO");

			string body = Trigger.ResolveBody();
			if (Mode == ScriptMode.Create){
				if (Trigger.IsFullyExternal()){
					sb.Append(body);
				}
				else{
					string targets = string.Join(",",
					                             new[]{
						                             Trigger.Insert ? "INSERT" : null, Trigger.Update ? "UPDATE" : null,
						                             Trigger.Delete ? "DELETE" : null
					                             }.Where(_ => null != _));
					string mode = Trigger.Before ? " INSTEAD OF " : " FOR ";
					sb.Append("CREATE TRIGGER ${FullName} ON ${TableName}" + mode + targets + " AS BEGIN");
					sb.AppendLine();
					sb.AppendLine(body);
					sb.Append("END;");
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			return "TRIGGER " + Trigger.FullName;
		}
	}
}