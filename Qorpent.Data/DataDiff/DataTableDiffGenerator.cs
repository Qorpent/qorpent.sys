using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
		public void  Generate(){
			var result = new ConcurrentDictionary<string, DataDiffTable>();
			foreach (var diffPair in _diffs){
				Collect(diffPair,result);
			}
			_context.Tables = result.Values;
			foreach (var table in _context.Tables){
				var maps = _context.Mappings.Where(_ =>_.FromTable=="*" || _.FromTable.ToLowerInvariant() == table.TableName.ToLowerInvariant());
				bool wasnoaliascodes = false;
				bool wasnorevisions = false;
				foreach (var map in maps){
					if (map.FromField.ToLowerInvariant() == "aliascodes"){
						if (!wasnoaliascodes){
							table.UseAliasCodes = true;
						}
					}else if (map.FromField.ToLowerInvariant() == "no-aliascodes"){
						table.UseAliasCodes = false;
						wasnoaliascodes = true;
					}
					else if (map.FromField.ToLowerInvariant() == "revision"){
						if (!wasnorevisions){
							table.UseRevisions = true;
						}
					}else if (map.FromField.ToLowerInvariant() == "no-revision"){
						table.UseRevisions = false;
						wasnorevisions = true;
					}
					else if (map.FromField.ToLowerInvariant() == "no-code")
					{
						table.NoCode = true;
					}
					else{
						table.Mappings[map.FromField.ToLowerInvariant()] = map.ToTable;
					}
				}
				var indexes = _context.Indexes.Where(_ => _.FromTable == "*" || _.FromTable.ToLowerInvariant() == table.TableName.ToLowerInvariant());
				foreach (var tableMap in indexes){
					table.DisableIndexes.Add(tableMap.FromField);
				}
			}

			foreach (var table in _context.Tables){
				if (table.UseRevisions && !string.IsNullOrWhiteSpace(_context.ResolvedUpdateRevision)){
					foreach (var item in table.Definitions){
						item.Fields["metafile"] = _context.ProjectName + ".project";
						item.Fields["revision"] = _context.ResolvedUpdateRevision.Substring(0,7);
					}
				}
			}
		}

		private void Collect(DiffPair pair, ConcurrentDictionary<string, DataDiffTable> result){
			var basis = pair.Base ?? new XElement("stub");
			var updated = pair.Updated ?? new XElement("stub");
			var baseTableName = pair.Updated.Attr("table");
			var mixed = pair.Updated.Attr("mixed").ToBool();
			var dynamicname = mixed|| string.IsNullOrWhiteSpace(baseTableName);
			var hierarchy = pair.Updated.Attr("tree").ToBool();
			var script = pair.Updated.Attr("script");
			if (string.IsNullOrWhiteSpace(script)){
				script = "10_Main";
			}
			var diffconfig = new XDiffOptions{
				IsHierarchy = hierarchy,
				MergeAttributeChanges = true,
				TreatNewAttributesAsChanges = true,
				TreatDeleteAttributesAsChanges = _context.EmptyAttributesAsUpdates,
				IncludeActions = XDiffAction.RenameElement | XDiffAction.MainCreateOrUpdate
			};
			var sw = Stopwatch.StartNew();
			var diff = new XDiffGenerator(diffconfig).GetDiff(basis,updated).ToArray();
			
			Collect(dynamicname, baseTableName, diff, result,script);
			sw.Stop();
			_context.Log.Debug("diff of " + baseTableName + " : " + sw.Elapsed);
		}

		private void Collect(bool dynamicname, string baseTableName, IEnumerable<XDiffItem> diffs, ConcurrentDictionary<string, DataDiffTable> dataDiffTables, string script){
			
			foreach (var d in diffs){
				var tableName = baseTableName;
				var src = d.NewestElement ?? d.BasisElement;
				var alt = d.BasisElement ?? d.NewestElement;
				var ename = src.Name.LocalName;
				if (dynamicname){
					tableName = ename;
				}
				if (_context.ExcludeTables.Contains(tableName)){
					continue;
				}
				if (0 != _context.IncludeTables.Count && !_context.IncludeTables.Contains(tableName))
				{
					continue;
				}
				var table = dataDiffTables.GetOrAdd(tableName, _ => new DataDiffTable{TableName = _});
				if (string.IsNullOrWhiteSpace(table.ScriptFile)){
					table.ScriptFile = script;
				}
				table.Sources.Add(d);
				var id = src.Attr("id").ToInt();
				var code = src.Attr("code");
				if (id == 0 && null!=alt) id = alt.Attr("id").ToInt();
				if (string.IsNullOrWhiteSpace(code) && null !=alt) code = alt.Attr("code");

				var item = ResolveItem(table, id, code);
				if (null != d.NewestElement){
					foreach (var a in d.NewestElement.Attributes()){
						var name = AdaptName(a);
						if (name == "id") continue;
						if (name == "code") continue;
						if (name == "_file") continue;
						if (name == "_line") continue;
						if (_context.IgnoreFields.Contains(name)) continue;
						item.Fields[name] = a.Value;
					}
				}
				else if (null != d.NewestAttribute){
					var name = AdaptName(d.NewestAttribute);
					if (_context.IgnoreFields.Contains(name)) continue;
					item.Fields[name] = d.NewestAttribute.Value;
				}
				else{
					item.Fields["set_parent"] = d.NewValue ?? "";
				}
			}
		}

		private static string AdaptName(XAttribute a){
			var name = a.Name.LocalName.ToLower();
			if (name == "id" || name == "code" || name == "_file" || name == "_line"){
				return name;
			}
			if (name.StartsWith("update-")){
				name = "set_" + name.Substring(7);
			}
			if (name == "__parent"){
				name = "set_parent";
			}
			return name;
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
