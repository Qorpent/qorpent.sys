using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.XDiff;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Стандартный мержер обновлений по объекту - формирует сводный апдейт
	/// </summary>
	public class DefaultDatabaseUpdateRecordMerger : IDatabaseUpdateRecordMerger
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public DatabaseUpdateRecord Merge(IEnumerable<DatabaseUpdateRecord> source){
			var result = new DatabaseUpdateRecord();
			var key = DatabaseUpdateRecord.GetObjectKey(source);
			var tableandschema = key.Table.Split('.');
			result.Schema = tableandschema.Length == 1 ? "dbo" : tableandschema[0];
			result.TargetTable = tableandschema.Length == 1 ? tableandschema[0] : tableandschema[1];
			result.TargetId = key.Id;
			result.TargetCode = key.Code;
			DetermineFileSource(source, result);
			var updates = source.Where(_ => 0 != (_.DiffItem.Action & XDiffAction.MainCreateOrUpdate)).ToArray();
			var deletes = source.Where(_ => _.DiffItem.Action == XDiffAction.DeleteElement).ToArray();
			if (0 == deletes.Length + updates.Length) return null;
			if (0 == updates.Length && 0 != deletes.Length){
				result.DiffItem = new XDiffItem{Action = XDiffAction.DeleteElement};
			}
			else{
				result.DiffItem = BuildUpdateDiffItem(key,updates);
			}
			return result;
		}

		private XDiffItem BuildUpdateDiffItem(ObjectKey key, DatabaseUpdateRecord[] updates){
			CheckInconsequenceCodeAndIdChanges(key, updates);
			var target = new XElement(key.Table, new XAttribute("id",key.Id),new XAttribute("code",key.Code));
			foreach (var record in updates.OrderBy(_=>_.Id)){
				if (record.DiffItem.Action == XDiffAction.CreateElement){
					foreach (var a in record.DiffItem.NewestElement.Attributes()){
						BindAttribute(target, a);
					}
				}else if (record.DiffItem.Action == XDiffAction.ChangeElement){
					target.Value = record.DiffItem.NewValue;
				}else if (record.DiffItem.Action == XDiffAction.CreateAttribute ||
				          record.DiffItem.Action == XDiffAction.ChangeAttribute){
					BindAttribute(target,record.DiffItem.NewestAttribute);
				}
				else if (record.DiffItem.Action == XDiffAction.ChangeHierarchyPosition)
				{
					BindAttribute(target, new XAttribute("__parent",record.DiffItem.NewValue));
				}
			}
			
			var result = new XDiffItem{Action = XDiffAction.CreateElement, NewestElement = target};
			result.Options = new XDiffOptions();
			foreach (var map in updates.Select(_=>_.DiffItem.Options.RefMaps)){
				if (null != map){
					result.Options.RefMaps = result.Options.RefMaps ?? new Dictionary<string, string>();
					foreach (var p in map){
						result.Options.RefMaps[p.Key] = p.Value;
					}
				}
			}
			if (updates.Any(_ => _.DiffItem.Options.ChangeIds)){
				result.Options.ChangeIds = true;
			}
			return result;
		}

		private void BindAttribute(XElement target, XAttribute a){
			var name = a.Name.LocalName;
			var val = a.Value;
			if (name == "__parent"){
				var startcode = val.IndexOf('-', val.IndexOf('-') + 1);
				val = val.Substring(startcode + 1);
				name = "Parent";
			}

			if (name.StartsWith("_")) return;
			if (name == "Id" || name == "Code" || name == "Name") name = name.ToLower();
			if (name.StartsWith("tag-")||name.StartsWith("tag.")){
				var current = target.Attr("Tag");
				val = TagHelper.SetValue(current, name.Substring(4), val);
				name = "Tag";
			}
			if((name=="code" ||name=="name") &&string.IsNullOrWhiteSpace(val))return;
			if((name=="id") &&val.ToInt()==0)return;
			target.SetAttributeValue(name,val);
		}

		private static void CheckInconsequenceCodeAndIdChanges(ObjectKey key, DatabaseUpdateRecord[] updates){
			var ids = updates.Select(_ => _.TargetId).Where(_ => _ != 0).Distinct().ToArray();
			var codes = updates.Select(_ => _.TargetCode).Where(_ => !string.IsNullOrWhiteSpace(_)).Distinct().ToArray();
			if (ids.Length > 1 || codes.Length > 1){
				throw new Exception("cannot merge with so-changed code or id");
			}

			var newids = updates.Where(
				_ =>
				0 != (_.DiffItem.Action & XDiffAction.AttributeCreateOrUpdate) && _.DiffItem.NewestAttribute.Name.LocalName == "Id")
			                    .Select(_ => _.DiffItem.NewestAttribute.Value.ToInt())
			                    .Where(_ => 0 != _ && key.Id != _).Distinct().ToArray();
			var newcodes = updates.Where(
				_ =>
				0 != (_.DiffItem.Action & XDiffAction.AttributeCreateOrUpdate) && _.DiffItem.NewestAttribute.Name.LocalName == "Code")
			                      .Select(_ => _.DiffItem.NewestAttribute.Value)
			                      .Where(_ => !string.IsNullOrWhiteSpace(_) && key.Code != _).Distinct().ToArray();

			if (newids.Length > 1 || newcodes.Length > 1){
				throw new Exception("cannot merge with multiple changes of id or codes");
			}
			if (newids.Length == 1 && newcodes.Length == 0){
				throw new Exception("cannot merge with updates on id and code both");
			}
		}

		private static void DetermineFileSource(IEnumerable<DatabaseUpdateRecord> source, DatabaseUpdateRecord result){
			var mainfilesrc = source.
				Where(_ => _.DiffItem.Action == XDiffAction.CreateElement).OrderBy(_ => _.DiffItem.NewestElement.ToString().Length)
			                        .FirstOrDefault();
			if (null != mainfilesrc){
				result.FileDelta = mainfilesrc.FileDelta;
			}
		}
	}
}