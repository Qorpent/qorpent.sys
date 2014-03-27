using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.XDiff;

namespace Qorpent.Data.MetaDataBase{

	/// <summary>
	/// 
	/// </summary>
	public interface IDatabaseUpdateRecordMerger{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		DatabaseUpdateRecord Merge(IEnumerable<DatabaseUpdateRecord> source);
	}


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
				
			}
			return result;
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

	/// <summary>
	/// Процессор по умолчанию
	/// </summary>
	public class DefaultMetaFileProcessor : IMetaFileProcessor{
		private string connection;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> alldeltas;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> working;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> error;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> ignored;
		
		/// <summary>
		/// 
		/// </summary>
		protected DatabaseUpdateRecord[] commands;

		private LinkedList<DatabaseUpdateRecord> grouped;
		private IDatabaseUpdateRecordMerger _merger;

		/// <summary>
		/// 
		/// </summary>
		public IDatabaseUpdateRecordMerger Merger{
			get { return _merger ?? (_merger=new DefaultDatabaseUpdateRecordMerger()); }
			set { _merger = value; }
		}


		bool Online{
			get { return !string.IsNullOrWhiteSpace(connection); }
		}

		/// <summary>
		/// Встраивает выявленное изменение в общий лог
		/// </summary>
		public void Prepare(string sqlconnection, IEnumerable<DatabaseUpdateRecord> givendelta, LinkedList<DatabaseUpdateRecord> workinglist, LinkedList<DatabaseUpdateRecord> errorlist){
			lock (this){
				connection = sqlconnection;
				alldeltas = new LinkedList<DatabaseUpdateRecord>(givendelta);
				grouped = new LinkedList<DatabaseUpdateRecord>(DatabaseUpdateRecord.Group(alldeltas).Values.Select(_=>Merger.Merge(_)));
					
				working = workinglist;
				error = errorlist;
				CheckTables();
			//	CheckCrossFileMovements();
			//	CheckCreates();
			//	CheckDeletes();
				
			}
			
		}
		/*
		private void CheckDeletes(){
			if (!Online) return;
			var query = string.Join("\r\nunion\r\n",
			                        creates.Union(deletes).Select(
				                        _ =>
				                        string.Format(
					                        "select {0},(selct top 1 id from {3} where (0= {1} or Id={1}) and (''='{2}' or Code='{2}') ) ")));
			var dict = getc().ExecuteDictionaryReader(query);
			var excludes = dict.Where(_ => _.Value == null).Select(_ => _.Key.ToInt()).ToArray();
			var _deletes = deletes.ToArray();
			foreach (var exclude in excludes)
			{
				var ex = _deletes.First(_ => _.Id == exclude);
				deletes.Remove(ex);
				alldeltas.Remove(ex);
				ignored.AddFirst(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void CheckCreates(){
			if (!Online) return;
			var query = string.Join("\r\nunion\r\n",
			                        creates.Union(deletes).Select(
				                        _ =>
				                        string.Format(
					                        "select {0},(selct top 1 id from {3} where (0= {1} or Id={1}) and (''='{2}' or Code='{2}') ) ")));
			var dict = getc().ExecuteDictionaryReader(query);
			var excludes = dict.Where(_ => _.Value != null).Select(_ => _.Key.ToInt()).ToArray();
			var _creates = creates.ToArray();
			foreach (var exclude in excludes){
				var ex = _creates.First(_ => _.Id == exclude);
				creates.Remove(ex);
				ex.ErrorCode = "DBR";
				ex.Error = "Данный элемент уже существует в БД";
				alldeltas.Remove(ex);
				error.AddFirst(ex);
			}
		}

		private void CheckCrossFileMovements(){
			var crossfilemoves = creates.Join(deletes, _ => _.FullCode, _ => _.FullCode, (f, s) => new{f, s}).ToArray();
			foreach (var crossfilemove in crossfilemoves){
				deletes.Remove(crossfilemove.s);
				creates.Remove(crossfilemove.f);
				var x1 = new XElement("a", crossfilemove.s.DiffItem.BasisElement);
				var x2 = new XElement("a", crossfilemove.f.DiffItem.NewestElement);
				var differ = new XDiffGenerator();
				var diff = differ.GetDiff(x1, x2);
				foreach (var diffItem in diff){
					var cpy = crossfilemove.f.Copy();
					cpy.DiffItem = diffItem;
					alldeltas.AddFirst(cpy);
				}
			}
		}
		 */

		IDbConnection getc(){
			return Applications.Application.Current.DatabaseConnections.GetConnection(connection);
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual  void CheckTables(){
			if (!Online) return;
			var tables = grouped.GroupBy(_ => _.FullTableName, _ => _);
			var query = string.Join("\r\nunion\r\n", tables.Select(_ =>string.Format("select '{0}' as code,object_id('{0}') as id ")));
			var dict = getc().ExecuteDictionaryReader(query);
			var excludes = dict.Where(_ => _.Value == null).Select(_ => _.Key).ToArray();
			foreach (var exclude in excludes){
				foreach (var i in tables.First(_=>_.Key==exclude)){
					i.ErrorCode = "EXT";
					i.Error = "таблица отсутствует в БД (" + i.FullTableName + ")";
					alldeltas.Remove(i);
					error.AddFirst(i);
				}
			}
		}

		/// <summary>
		/// Выполняет обновления
		/// </summary>
		/// <param name="sqlconnection"></param>
		/// <param name="records"></param>
		public void Execute(string sqlconnection, IEnumerable<DatabaseUpdateRecord> records){
			lock (this){
				var script = GetSql(records);
				using (var c = getc()){
					c.Open();
					var t = c.BeginTransaction();
					try{
						var com = c.CreateCommand(script);
						com.ExecuteNonQuery();
						t.Commit();
						
					}
					catch {
						t.Rollback();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Сформировать SQL по указанным командам
		/// </summary>
		/// <param name="records"></param>
		/// <param name="comments"></param>
		/// <returns></returns>
		public string GetSql(IEnumerable<DatabaseUpdateRecord> records, bool comments =false){
			commands = records.ToArray();
			CheckSql(comments);
			var script = CollectSql(comments);
			return script;
		}

		private string CollectSql(bool comments){
			var sb = new StringBuilder();
			foreach (var databaseUpdateRecord in commands){
				if (comments){
					sb.AppendLine();
					sb.AppendLine("/* " + databaseUpdateRecord + " */");
				}
				sb.AppendLine(databaseUpdateRecord.SqlCommand);
			}
			return sb.ToString();
		}

		private void CheckSql(bool comments){
			foreach (var command in commands){
				if (string.IsNullOrWhiteSpace(command.SqlCommand)){
					command.SqlCommand = BuildSql(command);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		protected virtual  string BuildSql(DatabaseUpdateRecord command){
			if (command.DiffItem.Action == XDiffAction.CreateElement){
				return BuildCreateNewSql(command);
			}else if (command.DiffItem.Action == XDiffAction.CreateAttribute ||
			          command.DiffItem.Action == XDiffAction.ChangeAttribute){
				return BuildUpdateSql(command);
			}
			return "-- it will be nice";
		}

		private string BuildUpdateSql(DatabaseUpdateRecord command){
			var xe = new XElement("a", command.DiffItem.NewestAttribute);
			var updict = CreateFieldDictionary(xe).ToArray();
			var trgdict = CreateFieldDictionary(command.DiffItem.BasisElement);
			var where = "";
			if (trgdict.ContainsKey("Id")){
				where = string.Format("Id = {0}", trgdict["Id"]);
			}
			else{
				where = string.Format("Code = '{0}'", trgdict["Code"].ToSqlString());
			}
			var cmd = string.Format("update {0} set {1} = '{2}', Acitve = 1, Version = (getdate())  where {3}", command.FullTableName, updict[0].Key, updict[0].Value.ToSqlString(),
			                        where);
			return cmd;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		protected virtual string BuildCreateNewSql(DatabaseUpdateRecord command){
			var dict_ = CreateFieldDictionary(command.DiffItem.NewestElement);
			if (!dict_.ContainsKey("Active")){
				dict_["Active"] = "1";
			}
			if (!dict_.ContainsKey("Version")){
				dict_["Version"] = "(getdate())";
			}
			var dict = dict_.ToArray();
			var fieldlist = string.Join(", ", dict.Select(_ => _.Key));
			var valuelist = string.Join(", ", dict.Select(_ => GetSqlVar(_.Value)));
			
			var where = "";
			var update = "";
			if (dict_.ContainsKey("Id")){
				where = "Id = " + dict_["Id"].ToInt();
				update = CollectUpdates(dict.Where(_ => _.Key != "Id"));
			}
			else{
				where = "Code = '" + dict_["Code"].ToSqlString() + "'";
				update = CollectUpdates(dict.Where(_ => _.Key != "Code"));
			}
			return string.Format(@"
if exists (select top 1 id from {0} where {3}) 
uupdate {0} set {4} where {3}
else insert {0} ({1}) values ({2})", command.FullTableName, fieldlist, valuelist,where,update);
		}

		private string GetSqlVar(string value){
			if (value.ToInt() != 0) return value;
			if (value.StartsWith("(") && value.EndsWith(")")) return value;
			return "'" + value.ToSqlString() + "'";
		}

		private string CollectUpdates(IEnumerable<KeyValuePair<string, string>> set){
			return string.Join(", ", set.Select(_ => _.Key + " = " + GetSqlVar(_.Value)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual  IDictionary<string,string> CreateFieldDictionary(XElement e){
			var result = new Dictionary<string, string>();
			var tag = "";
			foreach (var a in e.Attributes()){
				if (a.Name.LocalName.ToLower() == "id"){
					if (a.Value.ToInt() != 0){
						result["Id"] = a.Value;
					}
					continue;
					
				}
				if (a.Name.LocalName.ToLower() == "code")
				{
					if (!string.IsNullOrWhiteSpace(a.Value))
					{
						result["Code"] = a.Value;
					}
					continue;
				}
				if (a.Name.LocalName.ToLower() == "name")
				{
					if (!string.IsNullOrWhiteSpace(a.Value))
					{
						result["Name"] = a.Value;
					}
					continue;
				}
				
				if (a.Name.LocalName.StartsWith("tag.")){
					var tagname = a.Name.LocalName.Substring(4);
					tag = TagHelper.SetValue(tag, tagname, a.Value);
					continue;
				}

				result[a.Name.LocalName] = a.Value;
			}
			if (!string.IsNullOrWhiteSpace(e.Value)){
				result["Data"] =e.Value;
			}
			if (!string.IsNullOrWhiteSpace(tag)){
				result["Tag"] = tag;
			}
			return result;
		}
	}
}