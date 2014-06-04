using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Scaffolding.Sql;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// 
	/// </summary>
	public class SqlViewWriter : SqlCommandWriter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="view"></param>
		public SqlViewWriter(SqlView view)
		{
			this.View = view;
			this.Parameters = view;
		}
		/// <summary>
		/// 
		/// </summary>
		public SqlView View { get; set; }


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetText()
		{
			var sb = new StringBuilder();
			sb.AppendLine("IF OBJECT_ID('${Schema}.${Name}') IS NOT NULL DROP VIEW ${Schema}.${Name};");
			sb.AppendLine("GO");

			var body = ParseSpecialBody( View.ResolveBody());
			if (Mode == ScriptMode.Create)
			{
				if (View.IsFullyExternal())
				{
					sb.Append(body);
				}
				else
				{
					sb.Append("CREATE VIEW ${Schema}.${Name} AS SELECT ");
					sb.AppendLine();
					sb.AppendLine(body);
					sb.AppendLine();
				}
			}
			return sb.ToString();
		}

		private string ParseSpecialBody(string body){
			if (body.Contains("--PARENT_FIELD_SET--"))
			{
				var sb = new StringBuilder();
				foreach (var fld in View.Table.Fields.Values.OrderBy(_ => _.Idx))
				{
					sb.AppendLine(fld.Name + ", --" + fld.Comment);
				}
				body = body.Replace("--PARENT_FIELD_SET--", sb.ToString());
			}
			body= Regex.Replace(body,
							   @"(?ix)--\s*PARENT_REF_SET\sFOR\s\((?<fields>[^\)]+)\)\sWITH\s\((?<outers>[^\)]+)\)\s*--",
							   match
							   =>
							   {
								   var fields = match.Groups["fields"].Value.SmartSplit();
								   var outers = match.Groups["outers"].Value.SmartSplit();
								   var sb = new StringBuilder();
								   foreach (var field in fields){
									   var table = View.Table;
									   if (!table.Fields.ContainsKey(field.ToLowerInvariant()))
									   {
										   throw new Exception("table " + table.Name + " doesn't contains field " + field);
									   }
									   var fld = table.Fields[field];
									   foreach (var outer in outers)
									   {
										   sb.AppendLine(
											   string.Format("(select x.{1} from {0} x where x.{5} = {2}.{3}.{4}) as {4}{1},",
															 fld.ReferenceClass.FullSqlName, outer, table.Schema, table.Name, fld.Name, fld.ReferenceField
												   ));
									   }
								   }

								   return sb.ToString();
							   }).Trim();
				body += Environment.NewLine+"1 as __TERMINAL FROM " + View.Table.FullSqlName;
			
			return body;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher()
		{
			return "VIEW " + View.Name;
		}
	}
}