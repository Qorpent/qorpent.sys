using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// </summary>
	public class SqlViewWriter : SqlCommandWriter{
		/// <summary>
		/// </summary>
		/// <param name="view"></param>
		public SqlViewWriter(SqlView view){
			View = view;
			Parameters = view;
		}

		/// <summary>
		/// </summary>
		public SqlView View { get; set; }


		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			var sb = new StringBuilder();
			sb.AppendLine("IF OBJECT_ID('${FullName}') IS NOT NULL DROP VIEW ${FullName};");
			sb.AppendLine("GO");

			string body = ParseSpecialBody(View.ResolveBody());
			if (Mode == ScriptMode.Create){
				if (View.IsFullyExternal()){
					sb.Append(body);
				}
				else{
					bool isfullydefined = null!=body && body.Trim().ToLowerInvariant().StartsWith("select");
					sb.Append("CREATE VIEW ${FullName} AS ");
					if (!isfullydefined){
						sb.Append("SELECT");
					}
					sb.AppendLine();
					if (null != View.Definition.Element("selffields")){
						sb.Append(GetSelfFieldSet());
					}
					foreach (var element in View.Definition.Elements("reffields")){
						var free = element.GetSmartValue("free").ToBool();
						string[] rnames = element.Elements("by").SelectMany(_ => _.CollectFlags()).ToArray();
						string[] fields = element.Elements("use").SelectMany(_ => _.CollectFlags()).ToArray();
						WriteRefFields(rnames, fields, sb,free);
					}
					foreach (var element in View.Definition.Elements())
					{
						if(element.Name.LocalName=="selffields")continue;
						if (element.Name.LocalName == "reffields") continue;
					    var attrOrValue = element.AttrOrValue("code");
					    if (string.IsNullOrWhiteSpace(attrOrValue)) {
                            sb.AppendLine(element.Name.LocalName + ",");
                        } else { 
					        sb.AppendLine("(" + attrOrValue + ") as " + element.Name.LocalName+",");
                        }
                    }

					sb.AppendLine(body);
					if (!isfullydefined){
					    var sourceTable = View.Table.FullSqlName;
					    if (View.Definition.Attribute("from") != null) {
					        sourceTable = View.Definition.Attr("from");
					        if (!sourceTable.Contains(".")) {
					            sourceTable = View.Table.Schema + "." + sourceTable;
					        }
					        if (sourceTable.StartsWith("this.")) {
					            var viewCode = sourceTable.Split('.')[1];
					            var refView =
					                View.Table.SqlObjects.OfType<SqlView>()
					                    .FirstOrDefault(_ => _.Name.ToLowerInvariant() == viewCode.ToLowerInvariant());
					            if (refView == null) {
					                throw new Exception("Referenced view not found " + viewCode);
					            }
					            sourceTable = refView.FullName;
					        }
					    }
                        sb.AppendLine("1 as __TERMINAL FROM " + sourceTable);
					}
					sb.AppendLine();
				}
			}
			return sb.ToString();
		}

		private void WriteRefFields(IEnumerable<string> rnames, ICollection<string> fields, StringBuilder sb,bool free){
			PersistentClass table = View.Table;
			foreach (string refname in rnames){
				if (!table.Fields.ContainsKey(refname.ToLowerInvariant())){
					if (free){
						continue;
					}
					throw new Exception("table " + table.Name + " doesn't contains field " + refname);
				}
				Field fld = table.Fields[refname.ToLowerInvariant()];
				if (fld.NoSql) continue;
				PersistentClass rtable = fld.ReferenceClass;
				foreach (string fname in fields){
					if (!rtable.Fields.ContainsKey(fname.ToLowerInvariant())){
						if (free){
							continue;
						}
						throw new Exception("referenced table " + rtable.Name + " doesn't contains field " + fname);
					}
					Field rfld = rtable.Fields[fname.ToLowerInvariant()];
					if (rfld.NoSql) continue;
					sb.AppendLine(
						string.Format("(select x.{1} from {0} x where x.{5} = {2}.{3}.{4}) as {6}{7},",
						              fld.ReferenceClass.FullSqlName, fname.SqlQuoteName(), table.Schema.SqlQuoteName(),
						              table.Name.SqlQuoteName(), fld.Name.SqlQuoteName(), fld.ReferenceField.SqlQuoteName(),
						              fld.Name, fname
							));
				}
			}
		}


		private string ParseSpecialBody(string body){
			if (body.Contains("--PARENT_FIELD_SET--")){
				StringBuilder sb = GetSelfFieldSet();
				body = body.Replace("--PARENT_FIELD_SET--", sb.ToString());
			}
			body = Regex.Replace(body,
			                     @"(?ix)--\s*PARENT_REF_SET\sFOR\s\((?<fields>[^\)]+)\)\sWITH\s\((?<outers>[^\)]+)\)\s*--",
			                     match
			                     =>{
				                     IList<string> fields = match.Groups["fields"].Value.SmartSplit();
				                     IList<string> outers = match.Groups["outers"].Value.SmartSplit();
				                     var sb = new StringBuilder();
				                     WriteRefFields(fields, outers, sb,false);

				                     return sb.ToString();
			                     }).Trim();


			return body;
		}

		private StringBuilder GetSelfFieldSet(){
			var sb = new StringBuilder();
			foreach (Field fld in View.Table.Fields.Values.Where(_ => !_.NoSql).OrderBy(_ => _.Idx)){
				sb.AppendLine(fld.Name.SqlQuoteName() + ", --" + fld.Comment);
			}
			return sb;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			return "VIEW " + View.FullName;
		}
	}
}