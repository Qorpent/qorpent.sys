using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.XDiff;

namespace Qorpent.Data.MetaDataBase{
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
		protected LinkedList<DatabaseUpdateRecord> creates;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> ignored;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> deletes;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> renames;
		/// <summary>
		/// 
		/// </summary>
		protected DatabaseUpdateRecord[] commands;

		private LinkedList<DatabaseUpdateRecord> changes;


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
				working = workinglist;
				error = errorlist;
				creates = new LinkedList<DatabaseUpdateRecord>(alldeltas.Where(_ => _.DiffItem.Action == XDiffAction.CreateElement));
				deletes = new LinkedList<DatabaseUpdateRecord>(alldeltas.Where(_ => _.DiffItem.Action == XDiffAction.CreateElement));
				renames = new LinkedList<DatabaseUpdateRecord>(alldeltas.Where(_ => _.DiffItem.Action == XDiffAction.RenameElement));
				changes = new LinkedList<DatabaseUpdateRecord>(alldeltas.Where(_ => _.DiffItem.Action == XDiffAction.ChangeAttribute|| _.DiffItem.Action==XDiffAction.CreateAttribute|| _.DiffItem.Action==XDiffAction.ChangeHierarchyPosition));
				foreach (var rename in renames){
					//by default renames are not ignored but removed from default agile
					alldeltas.Remove(rename);
				}
				CheckTables();
				CheckCrossFileMovements();
				CheckCreates();
				CheckDeletes();
				foreach (var c in creates){
					working.AddLast(c);
				}
				foreach (var c in changes)
				{
					working.AddLast(c);
				}
				foreach (var c in deletes)
				{
					working.AddLast(c);
				}
			}
			
		}

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

		IDbConnection getc(){
			return Applications.Application.Current.DatabaseConnections.GetConnection(connection);
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual  void CheckTables(){
			if (!Online) return;
			var tables = alldeltas.GroupBy(_ => _.FullTableName, _ => _);
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
		/// <returns></returns>
		public string GetSql(IEnumerable<DatabaseUpdateRecord> records){
			commands = records.ToArray();
			CheckSql();
			var script = CollectSql();
			return script;
		}

		private string CollectSql(){
			var sb = new StringBuilder();
			foreach (var databaseUpdateRecord in commands){
				sb.AppendLine();
				sb.AppendLine("-- " + databaseUpdateRecord);
				sb.AppendLine(databaseUpdateRecord.SqlCommand);
			}
			return sb.ToString();
		}

		private void CheckSql(){
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
			}
			return "-- it will be nice";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		protected virtual string BuildCreateNewSql(DatabaseUpdateRecord command){
			var dict = CreateFieldDictionary(command).ToArray();
			var fieldlist = string.Join(",", dict.Select(_ => _.Key));
			var valuelist = string.Join(",", dict.Select(_ => " '" + _.Value + "'"));
			return string.Format("insert {0} ({1}) values ({2})", command.FullTableName, fieldlist, valuelist);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		protected virtual  IDictionary<string,string> CreateFieldDictionary(DatabaseUpdateRecord command){
			var result = new Dictionary<string, string>();
			var tag = "";
			foreach (var a in command.DiffItem.NewestElement.Attributes()){
				if (a.Name.LocalName.ToLower() == "id"){
					if (a.Value.ToInt() != 0){
						result["Id"] = a.Value;
					}
					else{
						continue;
					}
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
					TagHelper.SetValue(tag, tagname, a.Value);
					continue;
				}

				result[a.Name.LocalName] = a.Value;
			}
			if (!string.IsNullOrWhiteSpace(command.DiffItem.NewestElement.Value)){
				result["Data"] = command.DiffItem.NewestElement.Value;
			}
			if (!string.IsNullOrWhiteSpace(tag)){
				result["Tag"] = tag;
			}
			return result;
		}
	}
}