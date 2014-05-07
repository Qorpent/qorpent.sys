using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.XDiff;

namespace Qorpent.Data.DataDiff
{
	/// <summary>
	/// Формирует промежуточное представление для обновления таблиц
	/// </summary>
	public class DataTableDiffGenerator
	{
		private TableDiffGeneratorContext _context;
		private DiffPair[] _diffs;

		/// <summary>
		/// 
		/// </summary>
		public DataTableDiffGenerator(TableDiffGeneratorContext context){
			_context = context;
			if (null == _context.DiffPairs){
				throw new Exception("DataTableDiffGenerator: diffpairs was not set");
			}
			_diffs = _context.DiffPairs.ToArray();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void  GetTableDiff(){
			var result = new ConcurrentDictionary<string, DataDiffTable>();
			_diffs.AsParallel().ForAll(_ => Collect(_, result));
			_context.Tables = result.Values;
		}

		private void Collect(DiffPair pair, ConcurrentDictionary<string, DataDiffTable> result){
			var basis = pair.Base ?? new XElement("stub");
			var updated = pair.Updated ?? new XElement("stub");
			var baseTableName = pair.Updated.Attr("table");
			var mixed = pair.Updated.Attr("mixed").ToBool();
			var dynamicname = mixed|| string.IsNullOrWhiteSpace(baseTableName);
			var hierarchy = pair.Updated.Attr("tree").ToBool();
			var diffconfig = new XDiffOptions{
				IsHierarchy = hierarchy,
				MergeAttributeChanges = true,
				TreatNewAttributesAsChanges = true,
				IncludeActions = XDiffAction.RenameElement | XDiffAction.MainCreateOrUpdate
			};
			var diff = new XDiffGenerator(diffconfig).GetDiff(basis,updated).ToArray();
			Collect(dynamicname, baseTableName, diff, result);
		}

		private void Collect(bool dynamicname, string baseTableName, IEnumerable<XDiffItem> diffs, ConcurrentDictionary<string, DataDiffTable> dataDiffTables){
			
			foreach (var d in diffs){
				var tableName = baseTableName;
				var src = d.NewestElement ?? d.BasisElement;
				var alt = d.BasisElement ?? d.NewestElement;
				var ename = src.Name.LocalName;
				if (dynamicname){
					tableName = ename;
				}
				var table = dataDiffTables.GetOrAdd(tableName, _ => new DataDiffTable{TableName = _});
				table.Sources.Add(d);
				var id = src.Attr("id").ToInt();
				var code = src.Attr("code");
				if (id == 0 && null!=alt) id = alt.Attr("id").ToInt();
				if (string.IsNullOrWhiteSpace(code) && null !=alt) code = alt.Attr("code");

				var item = ResolveItem(table, id, code);
				if (null != d.NewestElement){
					foreach (var a in d.NewestElement.Attributes()){
						var name = a.Name.LocalName.ToLower();
						if (name == "id" || name == "code" || name == "_file" || name == "_line"){
							continue;
						}
						if (name.StartsWith("update-")){
							name = "set_" + name.Substring(7);
						}
						if (name == "__parent"){
							name = "set_parent";
						}
						item.Fields[name] = a.Value;
					}
				}
				else if (null != d.NewestAttribute){
					item.Fields[d.NewestAttribute.Name.LocalName] = d.NewestAttribute.Value;
				}
				else{
					item.Fields["set_parent"] = d.NewValue ?? "";
				}
			}
		}

		private static DataDiffItem ResolveItem(DataDiffTable table, int id, string code){
			DataDiffItem existed;
			lock (table.Definitions){
				existed = table.Definitions.FirstOrDefault(
					_ => (0 != id && _.Id == id) || (!string.IsNullOrWhiteSpace(code) && code == _.Code));
				if (null == existed){
					existed = new DataDiffItem{Id = id, Code = code};
					table.Definitions.Add(existed);
				}
				else{
					if (0 == existed.Id){
						existed.Id = id;
					}
					if (string.IsNullOrWhiteSpace(existed.Code)){
						existed.Code = code;
					}
				}
			}
			return existed;
		}
	}
}
