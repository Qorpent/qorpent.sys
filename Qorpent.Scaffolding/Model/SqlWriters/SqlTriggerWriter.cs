using System.Linq;
using System.Text;
using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// 
	/// </summary>
	public class SqlTriggerWriter : SqlCommandWriter{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlTrigger"></param>
		public SqlTriggerWriter(SqlTrigger sqlTrigger){
			this.Trigger = sqlTrigger;
			this.Parameters = sqlTrigger;
		}
		/// <summary>
		/// 
		/// </summary>
		public SqlTrigger Trigger { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetText()
		{
			if (Dialect != SqlDialect.SqlServer){
				return "-- !ВНИМАНИЕ НА ДАННЫЙ МОМЕНТ РЕАЛИЗАЦИЯ ГЕНЕРАЦИИ ТРИГЕРОВ ЕСТЬ ТОЛЬКО ДЛЯ MS SQL";
			}
			var sb = new StringBuilder();
			sb.AppendLine("IF OBJECT_ID('${Schema}.${Name}') IS NOT NULL DROP TRIGGER ${Schema}.${Name};");
			sb.AppendLine("GO");

			var body = Trigger.ResolveBody();
			if (Mode == ScriptMode.Create){
				if (Trigger.IsFullyExternal()){
					sb.Append(body);
				}
				else{
					var targets = string.Join(",",
					                          new[]{
						                          Trigger.Insert ? "INSERT" : null, Trigger.Update ? "UPDATE" : null,
						                          Trigger.Delete ? "DELETE" : null
					                          }.Where(_ => null != _));
					var mode = Trigger.Before ? " INSTEAD OF " : " FOR ";
					sb.Append("CREATE TRIGGER ${Schema}.${Name} ON ${TableName}" + mode + targets + " AS BEGIN");
					sb.AppendLine();
					sb.AppendLine(body);
					sb.Append("END;");
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			return "TRIGGER " + Trigger.Name;
		}


	}
}